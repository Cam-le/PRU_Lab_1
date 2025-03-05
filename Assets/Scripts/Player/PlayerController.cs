using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float bounceHeight = 0.3f;
    [SerializeField] private float bounceSpeed = 4f;

    [Header("References")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private DiceRoller diceRoller;
    [SerializeField] private AudioSource moveSoundEffect;

    [Header("Debug")]
    [SerializeField] private bool allowManualMovement = true;

    // Private variables
    private Vector2 currentPosition = Vector2.zero;
    private int currentTileIndex = 0;
    private bool isMoving = false;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Property to check if player is currently moving
    public bool IsMoving => isMoving;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Ensure sprite is rendered above tiles
        spriteRenderer.sortingOrder = 10;
    }

    void Start()
    {
        // Initialize position
        InitializePosition();

        // Subscribe to dice roller events if available
        if (diceRoller != null)
        {
            diceRoller.OnDiceRolled.AddListener(MovePlayer);
        }
        else
        {
            Debug.LogError("No DiceRoller assigned to PlayerController!");
        }
    }

    void Update()
    {
        // For testing - allow manual movement with space bar
        if (allowManualMovement && Input.GetKeyDown(KeyCode.Space) && !isMoving)
        {
            int steps = Random.Range(1, 7); // Simulate a dice roll
            StartCoroutine(MovePlayerSteps(steps));
        }
    }

    private void InitializePosition()
    {
        // Check if we're returning from another scene
        if (PlayerState.ReturningFromMinigame)
        {
            transform.position = PlayerState.LastPosition;
            currentPosition = PlayerState.CurrentPosition;
            currentTileIndex = PlayerState.CurrentTileIndex;
            PlayerState.ReturningFromMinigame = false;
        }
        else
        {
            // Start at the first tile
            currentTileIndex = 0;
            var startTile = gridManager.GetTileAtIndex(currentTileIndex);
            if (startTile != null)
            {
                currentPosition = gridManager.GetTilePosition(startTile);
                transform.position = new Vector3(currentPosition.x, currentPosition.y, 0);
            }
            else
            {
                Debug.LogError("Could not find start tile!");
            }
        }
    }

    // Called by dice roller
    public void MovePlayer(int steps)
    {
        if (!isMoving)
        {
            Debug.Log($"Moving player {steps} steps based on dice roll");
            StartCoroutine(MovePlayerSteps(steps));
        }
    }


    IEnumerator MovePlayerSteps(int steps)
    {
        if (isMoving) yield break;
        isMoving = true;

        // Play animation if available
        if (animator != null)
        {
            animator.SetBool("IsMoving", true);
        }

        // Move one step at a time
        for (int i = 0; i < steps; i++)
        {
            // Get the next tile
            int nextTileIndex = currentTileIndex + 1;

            // Check if we've reached the end of the path
            if (nextTileIndex >= gridManager.PathLength)
            {
                // Either loop back or stop at the end
                // For this implementation, we'll stop at the end
                Debug.Log("Reached the end of the path!");
                break;
            }

            Tile nextTile = gridManager.GetTileAtIndex(nextTileIndex);
            if (nextTile == null)
            {
                Debug.LogError($"Could not find tile at index {nextTileIndex}");
                break;
            }

            Vector2 nextPosition = gridManager.GetTilePosition(nextTile);

            // Move to the next position with bezier curve
            yield return StartCoroutine(MoveAlongPath(transform.position, nextPosition));

            // Update current position
            currentPosition = nextPosition;
            currentTileIndex = nextTileIndex;

            // Play move sound if available
            if (moveSoundEffect != null)
            {
                moveSoundEffect.Play();
            }

            // Small pause between steps
            yield return new WaitForSeconds(0.2f);
        }

        // Stop animation
        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
        }

        isMoving = false;

        // Check if we landed on a special tile
        Tile currentTile = gridManager.GetTileAtIndex(currentTileIndex);
        if (currentTile != null && currentTile.tileType != Tile.TileType.Normal)
        {
            yield return StartCoroutine(HandleSpecialTile(currentTile));
        }
    }

    IEnumerator MoveAlongPath(Vector3 startPos, Vector2 endPos)
    {
        Vector3 endPosition = new Vector3(endPos.x, endPos.y, 0);

        // Calculate control point for bezier curve (elevated midpoint)
        Vector3 controlPoint = MovementUtils.GetControlPoint(startPos, endPosition);

        float elapsedTime = 0;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * moveSpeed;
            float t = Mathf.Clamp01(elapsedTime); // Ensure t is between 0 and 1

            // Calculate base position on curve
            Vector3 position = MovementUtils.QuadraticBezier(startPos, controlPoint, endPosition, t);

            // Add bouncing effect
            float bounce = Mathf.Sin(t * bounceSpeed * Mathf.PI) * bounceHeight;
            position.y += bounce;

            // Update player position
            transform.position = position;

            // Determine sprite direction based on movement
            if (endPosition.x > startPos.x)
                spriteRenderer.flipX = false;
            else if (endPosition.x < startPos.x)
                spriteRenderer.flipX = true;

            yield return null;
        }

        // Ensure we end up exactly at the target position
        transform.position = endPosition;
    }

    IEnumerator HandleSpecialTile(Tile tile)
    {
        // Trigger the tile's event
        tile.OnTileEntered?.Invoke();

        // Handle different tile types
        switch (tile.tileType)
        {
            case Tile.TileType.Checkpoint:
                Debug.Log("Landed on checkpoint");
                // Save checkpoint for respawn
                PlayerState.LastCheckpointIndex = currentTileIndex;
                break;

            case Tile.TileType.Event:
                Debug.Log("Landed on event tile");
                // Find and trigger the event manager
                EventManager eventManager = FindObjectOfType<EventManager>();
                if (eventManager != null)
                {
                    eventManager.TriggerRandomEvent(EventType.Neutral);
                }
                break;

            case Tile.TileType.Special:
                Debug.Log("Landed on special tile");

                // Save current state
                PlayerState.LastPosition = transform.position;
                PlayerState.CurrentPosition = currentPosition;
                PlayerState.CurrentTileIndex = currentTileIndex;
                PlayerState.ReturningFromMinigame = true;

                // Determine which minigame to load based on position or random selection
                string minigameScene = ChooseMinigame();

                // Load minigame scene
                SceneManager.LoadScene(minigameScene);
                break;
        }

        yield return null;
    }
    private string ChooseMinigame()
    {
        // Option 1: Random selection among available minigames
        string[] minigames = new string[] {
        "QuizScene",
        "MemoryGameScene",
        "ObjectFallingGame"
        };

        int index = Random.Range(0, minigames.Length);
        return minigames[index];
    }
    // For external scripts to force player movement
    public void TeleportToTile(int tileIndex)
    {
        if (isMoving) return;

        Tile targetTile = gridManager.GetTileAtIndex(tileIndex);
        if (targetTile != null)
        {
            Vector2 targetPos = gridManager.GetTilePosition(targetTile);
            transform.position = new Vector3(targetPos.x, targetPos.y, 0);
            currentPosition = targetPos;
            currentTileIndex = tileIndex;
        }
    }
}