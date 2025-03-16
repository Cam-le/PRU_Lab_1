using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Simplified TileEffectManager that works with the existing GridManager
/// and just adds quiz effects to event tiles.
/// </summary>
public class TileEffectManager : MonoBehaviour
{
    [Header("Quiz Effect")]
    [SerializeField] private QuizEffect quizEffectPrefab;

    [Header("Notification UI")]
    [SerializeField] private GameObject notificationPrefab;
    [SerializeField] private float notificationDuration = 3.0f;
    [SerializeField] private Transform notificationContainer;
    [SerializeField] private bool animateNotifications = true;
    [SerializeField] private float fadeInTime = 0.5f;
    [SerializeField] private float fadeOutTime = 0.5f;

    // Track all tile effects
    private Dictionary<Tile, TileEffect> tileEffects = new Dictionary<Tile, TileEffect>();

    // Reference to GridManager
    private GridManager gridManager;

    private void Awake()
    {
        // Find GridManager
        gridManager = FindObjectOfType<GridManager>();
        if (gridManager == null)
        {
            Debug.LogError("TileEffectManager: GridManager not found in scene!");
        }

        // Subscribe to effect triggered event
        TileEffect.OnEffectTriggered += HandleEffectTriggered;
    }

    private void OnDestroy()
    {
        // Unsubscribe from event
        TileEffect.OnEffectTriggered -= HandleEffectTriggered;
    }

    private void Start()
    {
        // Wait a moment before setting up effects to ensure GridManager has created tiles
        StartCoroutine(DelayedSetup());
    }

    private IEnumerator DelayedSetup()
    {
        yield return new WaitForSeconds(0.5f);
        SetupQuizEffects();
    }

    /// <summary>
    /// Sets up quiz effects on all event tiles created by the GridManager
    /// </summary>
    public void SetupQuizEffects()
    {
        if (gridManager == null)
        {
            Debug.LogError("Cannot setup effects: GridManager not found");
            return;
        }

        // Clear any existing effects
        ClearAllEffects();

        int pathLength = gridManager.PathLength;
        int effectsAdded = 0;

        // Go through all tiles and add quiz effects to event tiles
        for (int i = 0; i < pathLength; i++)
        {
            Tile tile = gridManager.GetTileAtIndex(i);
            if (tile != null && tile.tileType == Tile.TileType.Event)
            {
                // Add a quiz effect to this event tile
                AddQuizEffectToTile(tile);
                effectsAdded++;
            }
        }

        Debug.Log($"Added quiz effects to {effectsAdded} event tiles");
    }

    /// <summary>
    /// Adds a quiz effect to a specific tile
    /// </summary>
    private void AddQuizEffectToTile(Tile tile)
    {
        if (tile == null || quizEffectPrefab == null) return;

        // Create a new instance of the QuizEffect
        QuizEffect effect = Instantiate(quizEffectPrefab, tile.transform);

        // Store the effect
        tileEffects[tile] = effect;

        // Add listener to trigger the effect directly when the player lands on this tile
        tile.OnTileEntered.AddListener(() => {
            // Safety check - make sure both effect and player exist
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null && !player.IsMoving && effect != null)
            {
                effect.ApplyEffect(player);
            }
        });
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

        // Ensure object still exists
        if (notification == null) yield break;

        canvasGroup.alpha = 1;

        // Wait for duration
        yield return new WaitForSeconds(notificationDuration);

        // Fade out
        elapsed = 0;
        while (elapsed < fadeOutTime && notification != null)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / fadeOutTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure object still exists
        if (notification == null) yield break;

        canvasGroup.alpha = 0;

        // Destroy after animation
        Destroy(notification);
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
                Destroy(effect.gameObject);
            }
        }

        tileEffects.Clear();
    }

    /// <summary>
    /// Manually refresh all effects (useful for debugging or after changing tile types)
    /// </summary>
    [ContextMenu("Refresh Quiz Effects")]
    public void RefreshEffects()
    {
        ClearAllEffects();
        SetupQuizEffects();
    }
}