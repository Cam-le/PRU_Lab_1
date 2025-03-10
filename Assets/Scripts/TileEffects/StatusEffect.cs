using UnityEngine;

/// <summary>
/// Effect that applies status buffs/debuffs to the player
/// </summary>
public class StatusEffect : TileEffect
{
    public enum StatusType
    {
        Luck,           // Affects random outcomes
        DiceBoost,      // Adds to dice rolls
        DoubleScore,    // Doubles score gains
        Protection,     // Protects from negative effects
        ScoreMultiplier // Multiplies score gains
    }

    [Header("Status Settings")]
    [SerializeField] private StatusType statusType = StatusType.Luck;
    [SerializeField] private bool isPositive = true;
    [SerializeField] private int magnitude = 1;
    [SerializeField] private int duration = 3; // In turns
    [SerializeField] private float chance = 1.0f; // Chance of effect applying (0-1)

    public override bool ApplyEffect(PlayerController player)
    {
        // Check if effect should be applied based on chance
        if (Random.value > chance)
        {
            Debug.Log($"Status Effect failed to apply (chance: {chance * 100}%)");
            return false;
        }

        // Call base implementation for common effect handling
        base.ApplyEffect(player);

        // Apply the status effect
        string statusId = GetStatusId();
        int effectValue = isPositive ? magnitude : -magnitude;
        PlayerState.AddBuff(statusId, duration);

        // Store the effect value in player state
        if (!PlayerState.StatusEffectValues.ContainsKey(statusId))
        {
            PlayerState.StatusEffectValues[statusId] = effectValue;
        }
        else
        {
            // For stacking effects, we could add to the existing value
            PlayerState.StatusEffectValues[statusId] += effectValue;
        }

        Debug.Log($"Status Effect: Applied {(isPositive ? "positive" : "negative")} {statusType} status (magnitude: {magnitude}, duration: {duration} turns)");

        return true;
    }

    private string GetStatusId()
    {
        return $"{statusType}{(isPositive ? "Boost" : "Penalty")}";
    }

    public override string GetEffectSummary()
    {
        string statusName = GetStatusName();
        string durationText = duration > 1 ? $"{duration} turns" : "1 turn";

        if (isPositive)
        {
            return $"+{magnitude} {statusName} for {durationText}";
        }
        else
        {
            return $"-{magnitude} {statusName} for {durationText}";
        }
    }

    private string GetStatusName()
    {
        switch (statusType)
        {
            case StatusType.Luck:
                return "Luck";
            case StatusType.DiceBoost:
                return "to Dice Rolls";
            case StatusType.DoubleScore:
                return "Score Doubler";
            case StatusType.Protection:
                return "Negative Effect Protection";
            case StatusType.ScoreMultiplier:
                return "Score Multiplier";
            default:
                return statusType.ToString();
        }
    }
}