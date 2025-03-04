using UnityEngine;
using System.Collections.Generic;

// Static class to persist player state between scenes
public static class PlayerState
{
    // Position tracking
    public static Vector3 LastPosition;
    public static Vector2 CurrentPosition;
    public static int CurrentTileIndex;
    public static bool ReturningFromMinigame;

    // Player stats
    public static int MovesRemaining;
    public static int Score;

    // Inventory
    public static List<string> Inventory = new List<string>();
    public static Dictionary<string, int> ItemQuantities = new Dictionary<string, int>();

    // Active buffs/debuffs
    public static Dictionary<string, float> BuffDurations = new Dictionary<string, float>();

    // Game state
    public static int CurrentTurn;
    public static GamePhase CurrentPhase = GamePhase.Move;

    // Helper methods
    public static void AddItem(string itemId, int quantity = 1)
    {
        if (!Inventory.Contains(itemId))
        {
            Inventory.Add(itemId);
            ItemQuantities[itemId] = quantity;
        }
        else
        {
            ItemQuantities[itemId] += quantity;
        }
    }

    public static bool UseItem(string itemId)
    {
        if (Inventory.Contains(itemId) && ItemQuantities[itemId] > 0)
        {
            ItemQuantities[itemId]--;

            // Remove item if quantity reaches 0
            if (ItemQuantities[itemId] <= 0)
            {
                Inventory.Remove(itemId);
                ItemQuantities.Remove(itemId);
            }

            return true;
        }

        return false;
    }

    public static void AddBuff(string buffId, float duration)
    {
        BuffDurations[buffId] = duration;
    }

    public static bool HasBuff(string buffId)
    {
        return BuffDurations.ContainsKey(buffId) && BuffDurations[buffId] > 0;
    }

    public static void ResetState()
    {
        // Reset position
        LastPosition = Vector3.zero;
        CurrentPosition = Vector2.zero;
        CurrentTileIndex = 0;
        ReturningFromMinigame = false;

        // Reset player stats
        MovesRemaining = 0;
        Score = 0;

        // Clear inventory
        Inventory.Clear();
        ItemQuantities.Clear();

        // Clear buffs
        BuffDurations.Clear();

        // Reset game state
        CurrentTurn = 1;
        CurrentPhase = GamePhase.Move;
    }
}

// Game phases enum
public enum GamePhase
{
    Start,
    Roll,
    Move,
    Action,
    Item,
    End
}