using UnityEngine;

/// <summary>
/// Effect that modifies player resources (score, moves, etc.)
/// </summary>
public class ResourceEffect : TileEffect
{
    public enum ResourceType
    {
        Score,
        Moves,
        Both
    }

    [Header("Resource Settings")]
    [SerializeField] private ResourceType resourceType = ResourceType.Score;
    [SerializeField] private int scoreChange = 0;
    [SerializeField] private int movesChange = 0;
    [Tooltip("If true, changes are percentage based rather than absolute values")]
    [SerializeField] private bool percentageBased = false;
    [Range(0, 100)]
    [SerializeField] private int percentage = 10;

    public override bool ApplyEffect(PlayerController player)
    {
        // Call base implementation for common effect handling
        base.ApplyEffect(player);

        GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
        if (gameManager == null) return false;

        // Apply resource changes based on configuration
        switch (resourceType)
        {
            case ResourceType.Score:
                ApplyScoreChange(gameManager);
                break;
            case ResourceType.Moves:
                ApplyMovesChange();
                break;
            case ResourceType.Both:
                ApplyScoreChange(gameManager);
                ApplyMovesChange();
                break;
        }

        return true;
    }

    private void ApplyScoreChange(GameManager gameManager)
    {
        int finalScoreChange = scoreChange;

        if (percentageBased)
        {
            finalScoreChange = Mathf.RoundToInt(PlayerState.Score * (percentage / 100f));
            if (scoreChange < 0) finalScoreChange = -finalScoreChange;
        }

        gameManager.AddScore(finalScoreChange);

        Debug.Log($"Resource Effect: Score {(finalScoreChange >= 0 ? "+" : "")}{finalScoreChange}");
    }

    private void ApplyMovesChange()
    {
        int finalMovesChange = movesChange;

        if (percentageBased)
        {
            finalMovesChange = Mathf.RoundToInt(PlayerState.MovesRemaining * (percentage / 100f));
            if (movesChange < 0) finalMovesChange = -finalMovesChange;
        }

        PlayerState.MovesRemaining += finalMovesChange;

        // Ensure moves don't go below zero
        if (PlayerState.MovesRemaining < 0)
            PlayerState.MovesRemaining = 0;

        Debug.Log($"Resource Effect: Moves {(finalMovesChange >= 0 ? "+" : "")}{finalMovesChange}");
    }

    public override string GetEffectSummary()
    {
        string summary = "";

        switch (resourceType)
        {
            case ResourceType.Score:
                summary = percentageBased
                    ? $"{(scoreChange >= 0 ? "+" : "-")}{percentage}% điểm"
                    : $"{(scoreChange >= 0 ? "+" : "")}{scoreChange} điểm";
                break;
            case ResourceType.Moves:
                summary = percentageBased
                    ? $"{(movesChange >= 0 ? "+" : "-")}{percentage}% lượt"
                    : $"{(movesChange >= 0 ? "+" : "")}{movesChange} lượt";
                break;
            case ResourceType.Both:
                string scoreSummary = percentageBased
                    ? $"{(scoreChange >= 0 ? "+" : "-")}{percentage}% điểm"
                    : $"{(scoreChange >= 0 ? "+" : "")}{scoreChange} điểm";
                string movesSummary = percentageBased
                    ? $"{(movesChange >= 0 ? "+" : "-")}{percentage}% lượt"
                    : $"{(movesChange >= 0 ? "+" : "")}{movesChange} lượt";
                summary = $"{scoreSummary}, {movesSummary}";
                break;
        }

        return summary;
    }
}