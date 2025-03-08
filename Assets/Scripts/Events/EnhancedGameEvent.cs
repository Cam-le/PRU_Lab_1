using static EnhancedGameEvent;

[System.Serializable]
public class EnhancedGameEvent : GameEvent
{
    public enum InteractionType
    {
        Choice,          // Player makes a choice
        Challenge,       // Quick skill test
        Trade,          // Resource trading
        Story,          // Story progression
        Blessing,       // Random buff
        Curse           // Random debuff
    }

    public InteractionType interactionType;
    public string[] choices;          // For choice events
    public int[] choiceEffects;       // Effects for each choice
    public float challengeDifficulty; // For skill challenges
    public string[] tradeOptions;     // For trading events
    public float duration;            // For buff/debuff duration

    // Constructor for enhanced events
    public EnhancedGameEvent(string id, string title, InteractionType type) : base(id, title, EventType.Neutral, 0)
    {
        interactionType = type;
    }
}

// Example event definitions
public static class EventTemplates
{
    public static EnhancedGameEvent CreateTraderEvent()
    {
        return new EnhancedGameEvent("trader", "Wandering Trader", InteractionType.Trade)
        {
            description = "A mysterious trader offers to exchange your resources...",
            tradeOptions = new string[] {
                "Trade 2 moves for 500 points",
                "Trade 300 points for an extra move",
                "Trade current buff for 2 moves"
            }
        };
    }

    public static EnhancedGameEvent CreateBlessingEvent()
    {
        return new EnhancedGameEvent("blessing", "Ancient Blessing", InteractionType.Blessing)
        {
            description = "A mysterious force empowers you...",
            duration = 3f, // Lasts 3 turns
            effectValue = Random.Range(1, 4) // Random positive effect
        };
    }

    public static EnhancedGameEvent CreateStoryEvent()
    {
        return new EnhancedGameEvent("story", "Village Tale", InteractionType.Story)
        {
            description = "An old villager shares a tale of hidden shortcuts...",
            choices = new string[] {
                "Listen carefully (+2 moves)",
                "Ask about treasures (+300 points)",
                "Share your own story (random buff)"
            }
        };
    }
}