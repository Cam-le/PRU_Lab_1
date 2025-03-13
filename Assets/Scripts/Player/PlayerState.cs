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
    public static bool HasSeenInstructions = false;

    // Player stats
    public static int MovesRemaining;
    public static int Score;
    public static int KeyCount = 0;

    // Active buffs/debuffs
    public static Dictionary<string, float> BuffDurations = new Dictionary<string, float>();

    // NEW: Track status effect values (magnitude of effects)
    public static Dictionary<string, int> StatusEffectValues = new Dictionary<string, int>();

    // Game state
    public static int CurrentTurn;
    public static GamePhase CurrentPhase = GamePhase.Move;

    // Checkpoint tracking
    public static int LastCheckpointIndex;

    // Track minigame rewards
    public static int TotalMinigameWins;
    public static int TotalMinigameLosses;

    // Track tile movement after minigames
    public static int TileMovementAdjustment = 0;

    // Helper methods
    public static void AddBuff(string buffId, float duration)
    {
        BuffDurations[buffId] = duration;
        Debug.Log($"Added buff {buffId} with duration {duration}");
    }

    public static bool HasBuff(string buffId)
    {
        return BuffDurations.ContainsKey(buffId) && BuffDurations[buffId] > 0;
    }

    // Get status effect value
    public static int GetStatusEffectValue(string effectId, int defaultValue = 0)
    {
        if (StatusEffectValues.ContainsKey(effectId))
        {
            return StatusEffectValues[effectId];
        }
        return defaultValue;
    }

    //Update buffs durations at the end of turn
    public static void UpdateBuffDurations()
    {
        // Create a copy of the keys to safely iterate through
        string[] buffKeys = new string[BuffDurations.Count];
        BuffDurations.Keys.CopyTo(buffKeys, 0);

        foreach (string buffId in buffKeys)
        {
            float duration = BuffDurations[buffId];

            if (duration <= 0)
            {
                // Remove expired buff
                BuffDurations.Remove(buffId);

                // Also remove from status effect values if it exists
                if (StatusEffectValues.ContainsKey(buffId))
                {
                    StatusEffectValues.Remove(buffId);
                }

                Debug.Log($"Buff {buffId} has expired");
            }
            else
            {
                // Decrement duration
                BuffDurations[buffId] = duration - 1;
            }
        }
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

        // Clear buffs
        BuffDurations.Clear();

        // Clear status effect values
        StatusEffectValues.Clear();

        // Reset game state
        CurrentTurn = 1;
        CurrentPhase = GamePhase.Move;

        // Reset additional tracking
        LastCheckpointIndex = 0;
        TotalMinigameWins = 0;
        TotalMinigameLosses = 0;

        // Reset tile movement adjustment
        TileMovementAdjustment = 0;

        // Reset instruction panel flag (only if you want to show instructions on a completely new game)
        HasSeenInstructions = false;
    }
}

// Game phases enum
public enum GamePhase
{
    Start,
    Roll,
    Move,
    Action,
    End
}