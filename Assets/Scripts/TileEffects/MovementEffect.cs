using UnityEngine;
using System.Collections;

/// <summary>
/// Effect that modifies player movement (teleport, move forward/backward)
/// </summary>
public class MovementEffect : TileEffect
{
    public enum MovementType
    {
        MoveForward,
        MoveBackward,
        Teleport,
        ReturnToCheckpoint,
        SkipNextTurn,
        ExtraTurn
    }

    [Header("Movement Settings")]
    [SerializeField] private MovementType movementType = MovementType.MoveForward;
    [SerializeField] private int spaces = 3;
    [Tooltip("Only used for Teleport type")]
    [SerializeField] private int targetTileIndex = -1;
    [Tooltip("Random range to pick from (min/max)")]
    [SerializeField] private Vector2Int randomRange = new Vector2Int(1, 6);
    [SerializeField] private bool useRandomSpaces = false;
    [SerializeField] private float movementDelay = 1f;

    private GridManager gridManager;

    private void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        if (gridManager == null)
        {
            Debug.LogError("MovementEffect: No GridManager found in the scene!");
        }
    }

    public override bool ApplyEffect(PlayerController player)
    {
        // Call base implementation for common effect handling
        base.ApplyEffect(player);

        if (gridManager == null)
        {
            gridManager = FindObjectOfType<GridManager>();
            if (gridManager == null)
            {
                Debug.LogError("MovementEffect: No GridManager found in the scene!");
                return false;
            }
        }

        // Get the actual number of spaces to move
        int actualSpaces = useRandomSpaces ? Random.Range(randomRange.x, randomRange.y + 1) : spaces;

        // Apply movement effect based on type
        switch (movementType)
        {
            case MovementType.MoveForward:
                StartCoroutine(DelayedMovement(player, actualSpaces));
                break;

            case MovementType.MoveBackward:
                StartCoroutine(DelayedMovement(player, -actualSpaces));
                break;

            case MovementType.Teleport:
                int teleportIndex = targetTileIndex;
                if (teleportIndex < 0 || teleportIndex >= gridManager.PathLength)
                {
                    // If invalid target index, teleport to a random valid position
                    teleportIndex = Random.Range(0, gridManager.PathLength);
                }
                StartCoroutine(DelayedTeleport(player, teleportIndex));
                break;

            case MovementType.ReturnToCheckpoint:
                StartCoroutine(DelayedTeleport(player, PlayerState.LastCheckpointIndex));
                break;

            case MovementType.SkipNextTurn:
                PlayerState.MovesRemaining = Mathf.Max(0, PlayerState.MovesRemaining - 1);
                Debug.Log("Movement Effect: Skip next turn");
                break;

            case MovementType.ExtraTurn:
                PlayerState.MovesRemaining += 1;
                Debug.Log("Movement Effect: Extra turn granted");
                break;
        }

        return true;
    }

    private IEnumerator DelayedMovement(PlayerController player, int spacesToMove)
    {
        yield return new WaitForSeconds(movementDelay);

        // Calculate target index
        int currentIndex = PlayerState.CurrentTileIndex;
        int targetIndex = Mathf.Clamp(currentIndex + spacesToMove, 0, gridManager.PathLength - 1);

        // Use the player's built-in teleport function
        player.TeleportToTile(targetIndex);

        Debug.Log($"Movement Effect: Moving {(spacesToMove >= 0 ? "forward" : "backward")} {Mathf.Abs(spacesToMove)} spaces to tile {targetIndex}");
    }

    private IEnumerator DelayedTeleport(PlayerController player, int targetIndex)
    {
        yield return new WaitForSeconds(movementDelay);

        // Ensure valid index
        int validIndex = Mathf.Clamp(targetIndex, 0, gridManager.PathLength - 1);

        // Use the player's built-in teleport function
        player.TeleportToTile(validIndex);

        Debug.Log($"Movement Effect: Teleporting to tile {validIndex}");
    }

    public override string GetEffectSummary()
    {
        string summary = "";

        switch (movementType)
        {
            case MovementType.MoveForward:
                summary = useRandomSpaces
                    ? $"Tiến {randomRange.x}-{randomRange.y} bước"
                    : $"Tiến {spaces} bước";
                break;

            case MovementType.MoveBackward:
                summary = useRandomSpaces
                    ? $"Lùi {randomRange.x}-{randomRange.y} bước"
                    : $"Lùi {spaces} bước";
                break;

            case MovementType.Teleport:
                summary = targetTileIndex >= 0
                    ? $"Đi tới {targetTileIndex}"
                    : "Đi tới vị trí ngẫu nhiên";
                break;

            case MovementType.ReturnToCheckpoint:
                summary = "Về Checkpoint đã ghé!";
                break;

            case MovementType.SkipNextTurn:
                summary = "Mất lượt";
                break;

            case MovementType.ExtraTurn:
                summary = "Thêm lượt";
                break;
        }

        return summary;
    }
}