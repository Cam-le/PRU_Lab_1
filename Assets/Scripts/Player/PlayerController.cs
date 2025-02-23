using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GridManager gridManager;

    private Vector2Int currentPosition = new Vector2Int(0, 0);
    private bool isMoving = false;

    private void Start()
    {
        transform.position = new Vector3(currentPosition.x, currentPosition.y, 0);
        // Ensure the player is rendered above the tiles
        SpriteRenderer playerRenderer = GetComponent<SpriteRenderer>();
        if (playerRenderer != null)
        {
            playerRenderer.sortingOrder = 1; // This will render above objects with lower sorting order
        }

        
    }
    void Update()
    {
        // Simple dice roll with spacebar
        if (Input.GetKeyDown(KeyCode.Space) && !isMoving)
        {
            int steps = Random.Range(1, 7); // Simulate dice roll
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
                break; // We've reached the end of the path

            // Move smoothly to next position
            Vector3 startPos = transform.position;
            Vector3 endPos = new Vector3(nextPosition.x, nextPosition.y, 0);
            float elapsedTime = 0;

            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime * moveSpeed;
                transform.position = Vector3.Lerp(startPos, endPos, elapsedTime);
                yield return null;
            }

            currentPosition = nextPosition;
            yield return new WaitForSeconds(0.1f);
        }

        isMoving = false;
    }
}