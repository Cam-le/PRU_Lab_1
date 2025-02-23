using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int maxDiceStep = 2;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float bounceHeight = 0.3f;
    [SerializeField] private float bounceSpeed = 4f;
    [SerializeField] private GridManager gridManager;

    private Vector2Int currentPosition = new Vector2Int(0, 0);
    private bool isMoving = false;
    private Vector3 lastPosition; // Store position before minigame

    void Start()
    {
        transform.position = new Vector3(currentPosition.x, currentPosition.y, 0);
        SpriteRenderer playerRenderer = GetComponent<SpriteRenderer>();
        if (playerRenderer != null)
        {
            playerRenderer.sortingOrder = 1;
        }
        lastPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isMoving)
        {
            int steps = Random.Range(1, maxDiceStep);
            StartCoroutine(MovePlayer(steps));
        }
    }

    IEnumerator MovePlayer(int steps)
    {
        isMoving = true;

        for (int i = 0; i < steps; i++)
        {
            Vector2Int nextPosition = gridManager.GetNextTilePosition(currentPosition);

            if (nextPosition == currentPosition)
                break;

            Vector3 startPos = transform.position;
            Vector3 endPos = new Vector3(nextPosition.x, nextPosition.y, 0);
            Vector3 controlPoint = MovementUtils.GetControlPoint(startPos, endPos);

            float elapsedTime = 0;
            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime * moveSpeed;
                float t = elapsedTime;

                // Calculate base position on curve
                Vector3 position = MovementUtils.QuadraticBezier(startPos, controlPoint, endPos, t);

                // Add bouncing effect
                float bounce = Mathf.Sin(t * bounceSpeed * Mathf.PI) * bounceHeight;
                position.y += bounce;

                transform.position = position;
                yield return null;
            }

            // Ensure we end up exactly at the target position
            transform.position = endPos;
            currentPosition = nextPosition;

            // Check if we landed on a special tile
            Tile currentTile = gridManager.GetTileAtPosition(currentPosition);
            if (currentTile != null && currentTile.tileType == Tile.TileType.Special)
            {
                lastPosition = transform.position;
                yield return StartCoroutine(HandleSpecialTile());
            }

            yield return new WaitForSeconds(0.1f);
        }

        isMoving = false;
    }

    IEnumerator HandleSpecialTile()
    {
        // Save current state
        PlayerState.LastPosition = transform.position;
        PlayerState.CurrentPosition = currentPosition;

        // Load minigame scene
        SceneManager.LoadScene("MinigameScene");
        yield break;
    }

    // Called when returning from minigame
    void OnEnable()
    {
        if (PlayerState.ReturningFromMinigame)
        {
            transform.position = PlayerState.LastPosition;
            currentPosition = PlayerState.CurrentPosition;
            PlayerState.ReturningFromMinigame = false;
        }
    }
}

// Static class to persist state between scenes
public static class PlayerState
{
    public static Vector3 LastPosition;
    public static Vector2Int CurrentPosition;
    public static bool ReturningFromMinigame;
}