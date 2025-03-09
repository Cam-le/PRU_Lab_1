using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Manages tile effects and notifications
/// </summary>
public class TileEffectManager : MonoBehaviour
{
    [Header("Effect Distribution")]
    [SerializeField] private List<TileEffect> availableEffects = new List<TileEffect>();
    [SerializeField] private bool distributeEffectsOnStart = true;
    [SerializeField] private float randomEffectChance = 0.3f;
    [SerializeField] private int minEffectsOnPath = 10;

    [Header("Effect Categories")]
    [SerializeField] private List<TileEffect> positiveEffects = new List<TileEffect>();
    [SerializeField] private List<TileEffect> negativeEffects = new List<TileEffect>();
    [SerializeField] private List<TileEffect> neutralEffects = new List<TileEffect>();
    [SerializeField] private List<TileEffect> specialEffects = new List<TileEffect>();
    [SerializeField] private List<TileEffect> storyEffects = new List<TileEffect>();

    [Header("Effect Balance")]
    [SerializeField] private float positiveEffectRatio = 0.5f;
    [SerializeField] private float negativeEffectRatio = 0.3f;
    [SerializeField] private float neutralEffectRatio = 0.1f;
    [SerializeField] private float specialEffectRatio = 0.05f;
    [SerializeField] private float storyEffectRatio = 0.05f;

    [Header("Notification UI")]
    [SerializeField] private GameObject notificationPrefab;
    [SerializeField] private float notificationDuration = 3.0f;
    [SerializeField] private Transform notificationContainer;
    [SerializeField] private bool animateNotifications = true;
    [SerializeField] private float fadeInTime = 0.5f;
    [SerializeField] private float fadeOutTime = 0.5f;

    private GridManager gridManager;
    private Dictionary<Tile, TileEffect> tileEffects = new Dictionary<Tile, TileEffect>();

    private void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();

        // Subscribe to effect triggered event
        TileEffect.OnEffectTriggered += HandleEffectTriggered;
    }

    private void OnDestroy()
    {
        // Unsubscribe from effect triggered event
        TileEffect.OnEffectTriggered -= HandleEffectTriggered;
    }

    private void Start()
    {
        if (distributeEffectsOnStart && gridManager != null)
        {
            DistributeEffects();
        }
    }

    /// <summary>
    /// Distributes effects across the game tiles
    /// </summary>
    public void DistributeEffects()
    {
        if (gridManager == null)
        {
            Debug.LogError("Cannot distribute effects: GridManager not found");
            return;
        }

        // Clear existing effects
        tileEffects.Clear();

        // Calculate how many tiles should have effects
        int pathLength = gridManager.PathLength;
        int effectCount = Mathf.FloorToInt(pathLength * randomEffectChance);
        effectCount = Mathf.Max(effectCount, minEffectsOnPath);
        effectCount = Mathf.Min(effectCount, pathLength - 2); // Don't use all tiles, and leave start/end free

        Debug.Log($"Distributing {effectCount} effects across {pathLength} tiles");

        // Create list of valid tiles (exclude start, end & only Normal tile)
        List<int> validTileIndices = new List<int>();
        for (int i = 1; i < pathLength - 1; i++)
        {
            Tile tile = gridManager.GetTileAtIndex(i);
            if (tile != null && tile.tileType == Tile.TileType.Event)
            {
                validTileIndices.Add(i);
            }
        }

        // Shuffle indices for random distribution
        ShuffleList(validTileIndices);

        // Calculate effect distribution based on ratios
        int positiveCount = Mathf.RoundToInt(effectCount * positiveEffectRatio);
        int negativeCount = Mathf.RoundToInt(effectCount * negativeEffectRatio);
        int neutralCount = Mathf.RoundToInt(effectCount * neutralEffectRatio);
        int specialCount = Mathf.RoundToInt(effectCount * specialEffectRatio);
        int storyCount = Mathf.RoundToInt(effectCount * storyEffectRatio);

        // Ensure we have at least one of each if possible and adjust to match total count
        int totalAssigned = positiveCount + negativeCount + neutralCount + specialCount + storyCount;
        int difference = effectCount - totalAssigned;

        // Adjust counts if needed
        if (difference != 0)
        {
            positiveCount += difference;
        }

        // Track how many we've assigned for each category
        int assignedPositive = 0;
        int assignedNegative = 0;
        int assignedNeutral = 0;
        int assignedSpecial = 0;
        int assignedStory = 0;

        // Assign effects to tiles
        for (int i = 0; i < validTileIndices.Count && i < effectCount; i++)
        {
            int tileIndex = validTileIndices[i];
            Tile tile = gridManager.GetTileAtIndex(tileIndex);
            if (tile == null) continue;

            TileEffect effect = null;

            // Determine which category to use based on counts
            if (assignedPositive < positiveCount && positiveEffects.Count > 0)
            {
                effect = GetRandomEffect(positiveEffects);
                assignedPositive++;
            }
            else if (assignedNegative < negativeCount && negativeEffects.Count > 0)
            {
                effect = GetRandomEffect(negativeEffects);
                assignedNegative++;
            }
            else if (assignedNeutral < neutralCount && neutralEffects.Count > 0)
            {
                effect = GetRandomEffect(neutralEffects);
                assignedNeutral++;
            }
            else if (assignedSpecial < specialCount && specialEffects.Count > 0)
            {
                effect = GetRandomEffect(specialEffects);
                assignedSpecial++;
            }
            else if (assignedStory < storyCount && storyEffects.Count > 0)
            {
                effect = GetRandomEffect(storyEffects);
                assignedStory++;
            }
            else if (availableEffects.Count > 0)
            {
                // Fallback to any available effect
                effect = GetRandomEffect(availableEffects);
            }

            // Assign effect to tile
            if (effect != null)
            {
                AssignEffectToTile(tile, effect);
            }
        }

        Debug.Log($"Distributed effects - Positive: {assignedPositive}, Negative: {assignedNegative}, " +
                  $"Neutral: {assignedNeutral}, Special: {assignedSpecial}, Story: {assignedStory}");
    }

    /// <summary>
    /// Assigns an effect to a specific tile
    /// </summary>
    public void AssignEffectToTile(Tile tile, TileEffect effectPrefab)
    {
        if (tile == null || effectPrefab == null) return;

        // Remove existing effect if any
        if (tileEffects.ContainsKey(tile))
        {
            TileEffect existingEffect = tileEffects[tile];
            if (existingEffect != null)
            {
                Destroy(existingEffect);
            }
            tileEffects.Remove(tile);
        }

        // Create a new instance of the effect
        TileEffect effect = Instantiate(effectPrefab, tile.transform);
        tileEffects[tile] = effect;

        // Update the tile to use the effect color
        //tile.SetColor(effect.EffectColor);

        // Add listener to trigger the effect when the player lands on this tile
        tile.OnTileEntered.AddListener(() => {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null && !player.IsMoving)
            {
                effect.ApplyEffect(player);
            }
        });
    }

    /// <summary>
    /// Get a random effect from the provided list, weighted by effect weight
    /// </summary>
    private TileEffect GetRandomEffect(List<TileEffect> effectList)
    {
        if (effectList.Count == 0) return null;

        int totalWeight = 0;
        foreach (TileEffect effect in effectList)
        {
            if (effect != null)
            {
                totalWeight += effect.EffectWeight;
            }
        }

        int randomWeight = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (TileEffect effect in effectList)
        {
            if (effect != null)
            {
                currentWeight += effect.EffectWeight;
                if (randomWeight < currentWeight)
                {
                    return effect;
                }
            }
        }

        // Fallback to first effect if something goes wrong
        return effectList[0];
    }

    /// <summary>
    /// Handle effect triggered event to show notification
    /// </summary>
    private void HandleEffectTriggered(TileEffect effect, PlayerController player)
    {
        if (effect == null || !effect.ShowNotification) return;

        ShowNotification(effect.EffectName, effect.GetEffectSummary(), effect.EffectIcon, effect.EffectColor);
    }

    /// <summary>
    /// Show notification UI
    /// </summary>
    public void ShowNotification(string title, string message, Sprite icon = null, Color color = default)
    {
        if (notificationPrefab == null || notificationContainer == null) return;

        // Default color if none provided
        if (color == default) color = Color.white;

        // Create notification instance
        GameObject notification = Instantiate(notificationPrefab, notificationContainer);

        // Set notification content
        TextMeshProUGUI titleText = notification.transform.Find("TitleText")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI messageText = notification.transform.Find("MessageText")?.GetComponent<TextMeshProUGUI>();
        Image iconImage = notification.transform.Find("IconImage")?.GetComponent<Image>();
        Image backgroundImage = notification.GetComponent<Image>();

        if (titleText != null) titleText.text = title;
        if (messageText != null) messageText.text = message;

        if (iconImage != null)
        {
            if (icon != null)
            {
                iconImage.sprite = icon;
                iconImage.enabled = true;
            }
            else
            {
                iconImage.enabled = false;
            }
        }

        if (backgroundImage != null)
        {
            // Use a tinted version of the color for the background
            Color bgColor = new Color(color.r, color.g, color.b, 0.8f);
            backgroundImage.color = bgColor;
        }

        // Animate notification
        if (animateNotifications)
        {
            StartCoroutine(AnimateNotification(notification));
        }
        else
        {
            // Just destroy after duration
            Destroy(notification, notificationDuration);
        }
    }

    /// <summary>
    /// Animate notification appearance and disappearance
    /// </summary>
    private IEnumerator AnimateNotification(GameObject notification)
    {
        CanvasGroup canvasGroup = notification.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = notification.AddComponent<CanvasGroup>();
        }

        // Fade in
        canvasGroup.alpha = 0;
        float elapsed = 0;
        while (elapsed < fadeInTime)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeInTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;

        // Wait for duration
        yield return new WaitForSeconds(notificationDuration);

        // Fade out
        elapsed = 0;
        while (elapsed < fadeOutTime)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / fadeOutTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;

        // Destroy the notification
        Destroy(notification);
    }

    /// <summary>
    /// Utility method to shuffle a list
    /// </summary>
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

    /// <summary>
    /// Get all assigned effects for debugging
    /// </summary>
    public Dictionary<Tile, TileEffect> GetAllTileEffects()
    {
        return tileEffects;
    }

    /// <summary>
    /// Clear all effects from tiles
    /// </summary>
    public void ClearAllEffects()
    {
        foreach (var effect in tileEffects.Values)
        {
            if (effect != null)
            {
                Destroy(effect);
            }
        }

        tileEffects.Clear();
    }
}