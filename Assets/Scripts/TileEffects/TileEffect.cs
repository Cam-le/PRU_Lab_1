using UnityEngine;
using System;

/// <summary>
/// Base class for all tile effects in the game
/// </summary>
public abstract class TileEffect : MonoBehaviour
{
    [Header("Effect Settings")]
    [SerializeField] protected string effectName = "Unnamed Effect";
    [SerializeField] protected string effectDescription = "No description";
    [Tooltip("Higher values mean this effect is more likely to be selected when randomly assigning effects")]
    [SerializeField, Range(1, 10)] protected int effectWeight = 5;
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
    public int EffectWeight => effectWeight;
    public bool ShowNotification => showNotification;
    public Sprite EffectIcon => effectIcon;
    public Color EffectColor => effectColor;
    public AudioClip EffectSound => effectSound;

    // Event to notify when an effect is triggered
    public static event Action<TileEffect, PlayerController> OnEffectTriggered;

    /// <summary>
    /// Apply the effect to the player
    /// </summary>
    /// <param name="player">The player controller</param>
    /// <returns>True if the effect was applied successfully</returns>
    public virtual bool ApplyEffect(PlayerController player)
    {
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

    /// <summary>
    /// Gets a short summary of what this effect does (for tooltips)
    /// </summary>
    public virtual string GetEffectSummary()
    {
        return effectDescription;
    }
}