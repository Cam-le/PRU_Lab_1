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

    [Header("Sound Effects")]
    //[SerializeField] private string moveStartSound = "playerMove";
    [SerializeField] private string checkpointSound = "checkpoint";
    [SerializeField] private string eventTileSound = "notification";
    [SerializeField] private string specialTileSound = "notification";

    [Header("Rewards and Penalties")]
    [SerializeField] private int checkpointPoints = 1000;
    [SerializeField] private int minEventSteps = -10;
    [SerializeField] private int maxEventSteps = 10;

    [Header("Debug")]
    [SerializeField] private bool allowManualMovement = true;

    [SerializeField] private GameManager gameManager;
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

        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
            if (gameManager == null)
            {
                Debug.LogWarning("GameManager not found! Score updates will not work.");
            }
        }
    }

    void Update()
    {
        // For testing - allow manual movement with space bar
        if (allowManualMovement && Input.GetKeyDown(KeyCode.M) && !isMoving)
        {
            int steps = Random.Range(3, 3); // Simulate a dice roll
            StartCoroutine(MovePlayerSteps(steps));
        }

        // New debug teleport to near end of path
        if (Input.GetKeyDown(KeyCode.E) && gridManager != null)
        {
            int nearEndIndex = gridManager.PathLength - 3; // 3 tiles before the end
            TeleportToTile(nearEndIndex);
            Debug.Log($"Teleporting to near end tile index: {nearEndIndex}");
        }
    }

    private void InitializePosition()
    {
        // Check if we're returning from another scene
        if (PlayerState.ReturningFromMinigame)
        {
            // First restore original position
            transform.position = PlayerState.LastPosition;
            currentPosition = PlayerState.CurrentPosition;
            currentTileIndex = PlayerState.CurrentTileIndex;
            PlayerState.ReturningFromMinigame = false;

            // Then check if we need to apply position adjustment from minigame
            if (PlayerState.TileMovementAdjustment != 0)
            {
                StartCoroutine(ApplyMinigamePositionAdjustment());
            }
            // Update the score from PlayerState when returning from minigame
            if (gameManager != null && PlayerState.ReturningFromMinigame)
            {
                gameManager.SetScore(PlayerState.Score);
            }
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
    private IEnumerator ApplyMinigamePositionAdjustment()
    {
        // Wait a moment for the board to fully load
        yield return new WaitForSeconds(0.5f);

        int adjustment = PlayerState.TileMovementAdjustment;
        PlayerState.TileMovementAdjustment = 0; // Reset the adjustment

        // Calculate target tile index
        int targetIndex = Mathf.Clamp(currentTileIndex + adjustment, 0, gridManager.PathLength - 1);

        if (targetIndex != currentTileIndex)
        {
            if (adjustment > 0)
            {
                Debug.Log($"Moving forward {adjustment} tiles as minigame reward!");
            }
            else
            {
                Debug.Log($"Moving backward {-adjustment} tiles as minigame penalty!");
            }

            // Use existing movement system to animate the adjustment
            StartCoroutine(MovePlayerToIndex(targetIndex));
        }
    }

    private IEnumerator MovePlayerToIndex(int targetIndex)
    {
        if (isMoving) yield break;
        isMoving = true;

        // Play animation if available
        if (animator != null)
        {
            animator.SetBool("IsMoving", true);
        }

        // Determine movement path and direction
        int direction = targetIndex > currentTileIndex ? 1 : -1;
        int steps = Mathf.Abs(targetIndex - currentTileIndex);

        // Move one step at a time
        for (int i = 0; i < steps; i++)
        {
            // Calculate next tile index
            int nextTileIndex = currentTileIndex + direction;

            // Check boundaries
            if (nextTileIndex < 0 || nextTileIndex >= gridManager.PathLength) break;

            Tile nextTile = gridManager.GetTileAtIndex(nextTileIndex);
            if (nextTile == null) break;

            Vector2 nextPosition = gridManager.GetTilePosition(nextTile);

            // Move to the next position with bezier curve
            yield return StartCoroutine(MoveAlongPath(transform.position, nextPosition));

            // Update current position
            currentPosition = nextPosition;
            currentTileIndex = nextTileIndex;

            // Update PlayerState to sync with current position
            PlayerState.CurrentTileIndex = currentTileIndex;
            PlayerState.CurrentPosition = currentPosition;
            PlayerState.LastPosition = transform.position;

            // Play start movement sound
            if (AudioManager.Instance != null)
            {
                //AudioManager.Instance.PlaySound(moveStartSound);
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

        // Note: We DON'T trigger special tile effects after minigame adjustment
        // to avoid potential infinite loops

        Tile currentTile = gridManager.GetTileAtIndex(currentTileIndex);
        if (currentTile != null && currentTile.tileType != Tile.TileType.Normal)
        {
            yield return StartCoroutine(HandleSpecialTile(currentTile));
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
                Debug.Log(PlayerState.CurrentTileIndex);
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

            // Update PlayerState to sync with current position
            PlayerState.CurrentTileIndex = currentTileIndex;
            PlayerState.CurrentPosition = currentPosition;
            PlayerState.LastPosition = transform.position;

            /// Play start movement sound
            if (AudioManager.Instance != null)
            {
                //AudioManager.Instance.PlaySound(moveStartSound);
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
                // Play checkpoint sound
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySound(checkpointSound);
                }

                // Award checkpoint points 
                if (gameManager != null)
                {
                    gameManager.AddScore(checkpointPoints);
                    Debug.Log($"Checkpoint bonus: +{checkpointPoints} points!");
                }
                else
                {
                    // Update PlayerState directly if GameManager not available
                    PlayerState.Score += checkpointPoints;
                    Debug.Log($"Checkpoint bonus: +{checkpointPoints} points! (PlayerState updated directly)");
                }

                // Save checkpoint for respawn
                PlayerState.LastCheckpointIndex = currentTileIndex;
                break;

            case Tile.TileType.Event:
                Debug.Log("Landed on event tile");
                // Play event tile sound
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySound(eventTileSound);
                }

                // Random movement on event tile (NEW FEATURE)
                int randomSteps = Random.Range(minEventSteps, maxEventSteps + 1);
                if (randomSteps != 0)
                {
                    Debug.Log($"Event tile effect: Moving {(randomSteps > 0 ? "forward" : "backward")} {Mathf.Abs(randomSteps)} steps!");

                    // Wait a moment before moving
                    yield return new WaitForSeconds(1.0f);

                    // Calculate target tile index
                    int targetIndex = Mathf.Clamp(currentTileIndex + randomSteps, 0, gridManager.PathLength - 1);

                    // Use existing movement system to animate the adjustment
                    yield return StartCoroutine(MovePlayerToIndex(targetIndex));

                    // Exit early since we've already handled the movement
                    yield break;
                }

                // Find and trigger the event manager
                EventManager eventManager = FindObjectOfType<EventManager>();
                if (eventManager != null)
                {
                    eventManager.TriggerRandomEvent(EventType.Neutral);
                }
                break;

            case Tile.TileType.Special:
                Debug.Log("Landed on special tile");
                // Play special tile sound
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySound(specialTileSound);
                }
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
        // Random selection among available minigames
        string[] minigames = new string[] {
        "QuestionScene",
        "MemoryMinigame",
        "ObjectFallingScene"
        };

        int index = Random.Range(0, minigames.Length);
        return minigames[index];
    }
    // For external scripts to force player movement
    public void TeleportToTile(int tileIndex)
    {
        // Validate references
        if (gridManager == null)
        {
            Debug.LogError("GridManager is not assigned!");
            return;
        }

        // Validate tile index
        if (tileIndex < 0 || tileIndex >= gridManager.PathLength)
        {
            Debug.LogError($"Invalid tile index: {tileIndex}. Path length is {gridManager.PathLength}");
            return;
        }

        // Prevent teleporting during movement
        if (isMoving)
        {
            Debug.LogWarning("Cannot teleport while moving!");
            return;
        }

        Tile targetTile = gridManager.GetTileAtIndex(tileIndex);
        if (targetTile == null)
        {
            Debug.LogError($"No tile found at index {tileIndex}");
            return;
        }

        Vector2 targetPos = gridManager.GetTilePosition(targetTile);

        // Update transform position
        transform.position = new Vector3(targetPos.x, targetPos.y, 0);

        // Update current position tracking
        currentPosition = targetPos;
        currentTileIndex = tileIndex;

        // Update PlayerState
        PlayerState.CurrentPosition = currentPosition;
        PlayerState.CurrentTileIndex = currentTileIndex;

        // Trigger tile-specific events if needed
        StartCoroutine(HandleSpecialTile(targetTile));

        Debug.Log($"Teleported to tile {tileIndex} at position {targetPos}");
    }
}