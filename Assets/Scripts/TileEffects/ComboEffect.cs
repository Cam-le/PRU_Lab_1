using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Combines multiple effects into a single tile effect
/// Allows for complex tile behaviors with multiple outcomes
/// </summary>
public class ComboEffect : TileEffect
{
    [Header("Combo Settings")]
    [SerializeField] private List<TileEffect> effects = new List<TileEffect>();
    [SerializeField] private bool sequentialExecution = true;
    [SerializeField] private float delayBetweenEffects = 0.5f;
    [SerializeField] private bool randomizeOrder = false;
    [SerializeField] private bool stopOnFailure = false;

    [Header("Conditional Settings")]
    [SerializeField] private bool useConditionals = false;
    [Tooltip("Only used when useConditionals is true")]
    [SerializeField] private int minimumPlayerScore = 0;
    [Tooltip("Only used when useConditionals is true")]
    [SerializeField] private int minimumPlayerMoves = 0;

    public override bool ApplyEffect(PlayerController player)
    {
        // Call base implementation for common effect handling
        base.ApplyEffect(player);

        // Check conditions if enabled
        if (useConditionals)
        {
            if (PlayerState.Score < minimumPlayerScore)
            {
                Debug.Log($"ComboEffect: Conditional failed - Player score {PlayerState.Score} below minimum {minimumPlayerScore}");
                return false;
            }

            if (PlayerState.MovesRemaining < minimumPlayerMoves)
            {
                Debug.Log($"ComboEffect: Conditional failed - Player moves {PlayerState.MovesRemaining} below minimum {minimumPlayerMoves}");
                return false;
            }
        }

        // Process effects
        if (sequentialExecution)
        {
            StartCoroutine(SequentialEffects(player));
        }
        else
        {
            ApplyAllEffects(player);
        }

        return true;
    }

    private void ApplyAllEffects(PlayerController player)
    {
        // Create a copy of the effects list
        List<TileEffect> effectsToApply = new List<TileEffect>(effects);

        // Randomize order if requested
        if (randomizeOrder)
        {
            ShuffleList(effectsToApply);
        }

        // Apply all effects at once
        foreach (TileEffect effect in effectsToApply)
        {
            if (effect != null)
            {
                effect.ApplyEffect(player);
            }
        }
    }

    private IEnumerator SequentialEffects(PlayerController player)
    {
        // Create a copy of the effects list
        List<TileEffect> effectsToApply = new List<TileEffect>(effects);

        // Randomize order if requested
        if (randomizeOrder)
        {
            ShuffleList(effectsToApply);
        }

        // Apply effects in sequence with delay
        foreach (TileEffect effect in effectsToApply)
        {
            if (effect != null)
            {
                bool success = effect.ApplyEffect(player);

                // Stop if an effect fails and stopOnFailure is true
                if (!success && stopOnFailure)
                {
                    Debug.Log("ComboEffect: Stopping sequence due to effect failure");
                    yield break;
                }

                yield return new WaitForSeconds(delayBetweenEffects);
            }
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public override string GetEffectSummary()
    {
        int validEffects = 0;
        foreach (TileEffect effect in effects)
        {
            if (effect != null)
            {
                validEffects++;
            }
        }

        string executionText = sequentialExecution ? "sequential" : "parallel";
        string randomText = randomizeOrder ? ", randomized" : "";

        return $"Combo: {validEffects} và ({executionText}{randomText})";
    }
}