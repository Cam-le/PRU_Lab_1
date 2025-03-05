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

    // Active buffs/debuffs
    public static Dictionary<string, float> BuffDurations = new Dictionary<string, float>();

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

        // Clear buffs
        BuffDurations.Clear();

        // Reset game state
        CurrentTurn = 1;
        CurrentPhase = GamePhase.Move;

        // Reset additional tracking
        LastCheckpointIndex = 0;
        TotalMinigameWins = 0;
        TotalMinigameLosses = 0;

        // Reset tile movement adjustment
        TileMovementAdjustment = 0;
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