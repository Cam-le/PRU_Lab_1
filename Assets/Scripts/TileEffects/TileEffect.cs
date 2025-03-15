using UnityEngine;
using System;

/// <summary>
/// Modified base class for all tile effects in the game
/// Now supports quiz-based effects
/// </summary>
public abstract class TileEffect : MonoBehaviour
{
    // New enum for simplified effect types
    public enum EffectType
    {
        Points,
        Moves,
        Turns,
        MoveForward,
        MoveBackward
    }

    [Header("Effect Settings")]
    [SerializeField] protected string effectName = "Unnamed Effect";
    [SerializeField] protected string effectDescription = "No description";
    [SerializeField] protected bool showNotification = true;
    [SerializeField] protected Sprite effectIcon;
    [SerializeField] protected Color effectColor = Color.white;
    [SerializeField] protected AudioClip effectSound;

    [Header("Visual Effects")]
    [SerializeField] protected GameObject effectParticlesPrefab;
    [SerializeField] protected float effectDuration = 1f;

    // Properties
    public string EffectName => effectName;
    public string EffectDescription => effectDescription;
    public bool ShowNotification => showNotification;
    public Sprite EffectIcon => effectIcon;
    public Color EffectColor => effectColor;
    public AudioClip EffectSound => effectSound;

    // Event to notify when an effect is triggered
    public static event Action<TileEffect, PlayerController> OnEffectTriggered;

    // Reference to the quiz manager
    protected TileQuizManager quizManager;

    protected virtual void Awake()
    {
        // Find quiz manager if available
        quizManager = FindObjectOfType<TileQuizManager>();
        if (quizManager == null)
        {
            Debug.LogWarning("TileQuizManager not found in the scene. Quiz-based effects won't work.");
        }
    }

    /// <summary>
    /// Apply the effect to the player
    /// </summary>
    /// <param name="player">The player controller</param>
    /// <returns>True if the effect was applied successfully</returns>
    public virtual bool ApplyEffect(PlayerController player)
    {
        // If we have a quiz manager, show the quiz rather than applying effects directly
        if (quizManager != null)
        {
            // Show the quiz and provide a callback for when it's completed
            quizManager.ShowQuiz((isCorrect, effectType, value) =>
            {
                // Apply the effect based on quiz result
                ApplyEffectBasedOnQuizResult(player, isCorrect, effectType, value);

                // Trigger the effect triggered event after the quiz
                OnEffectTriggered?.Invoke(this, player);
            });

            return true;
        }
        else
        {
            // Fallback to original behavior if no quiz manager
            // Trigger the effect triggered event
            OnEffectTriggered?.Invoke(this, player);

            // Play sound effect if available
            if (effectSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(effectSound.name);
            }

            // Spawn particles if available
            if (effectParticlesPrefab != null)
            {
                GameObject particles = Instantiate(effectParticlesPrefab, player.transform.position, Quaternion.identity);
                Destroy(particles, effectDuration);
            }

            return true;
        }
    }

    /// <summary>
    /// Apply effects based on quiz results
    /// </summary>
    protected virtual void ApplyEffectBasedOnQuizResult(PlayerController player, bool isCorrect, EffectType effectType, int value)
    {
        // Find the GameManager to apply score changes
        GameManager gameManager = FindObjectOfType<GameManager>();

        switch (effectType)
        {
            case EffectType.Points:
                if (gameManager != null)
                {
                    gameManager.AddScore(value);
                    Debug.Log($"Quiz result: Applied {value} points");
                }
                break;

            case EffectType.Moves:
                PlayerState.MovesRemaining += value;
                Debug.Log($"Quiz result: Applied {value} moves");
                break;

            case EffectType.Turns:
                // This would affect turn counter logic
                // Not directly implementing as it's handled by PlayerState.MovesRemaining
                PlayerState.MovesRemaining += value;
                Debug.Log($"Quiz result: Applied {value} turns");
                break;

            case EffectType.MoveForward:
                if (player != null)
                {
                    int targetIndex = Mathf.Min(PlayerState.CurrentTileIndex + value, FindObjectOfType<GridManager>().PathLength - 1);
                    player.TeleportToTile(targetIndex);
                    Debug.Log($"Quiz result: Moved forward {value} tiles");
                }
                break;

            case EffectType.MoveBackward:
                if (player != null)
                {
                    int targetIndex = Mathf.Max(PlayerState.CurrentTileIndex + value, 0); // value should be negative
                    player.TeleportToTile(targetIndex);
                    Debug.Log($"Quiz result: Moved backward {-value} tiles");
                }
                break;
        }

        // Play sound effect if available
        if (effectSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(effectSound.name);
        }

        // Spawn particles if available
        if (effectParticlesPrefab != null)
        {
            GameObject particles = Instantiate(effectParticlesPrefab, player.transform.position, Quaternion.identity);
            Destroy(particles, effectDuration);
        }
    }

    /// <summary>
    /// Gets a short summary of what this effect does (for tooltips)
    /// </summary>
    public virtual string GetEffectSummary()
    {
        return effectDescription;
    }
}