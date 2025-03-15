using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Completely redesigned TileEffectManager focused on quiz-based effects
/// </summary>
public class TileEffectManager : MonoBehaviour
{
    [Header("Quiz Effect System")]
    [SerializeField] private QuizEffect quizEffectPrefab;
    [SerializeField] private float quizDelayTime = 0.5f;

    [Header("Effect Distribution")]
    [SerializeField] private float effectChance = 0.3f;
    [SerializeField] private int minEffectsOnPath = 10;

    [Header("Effect Balance")]
    [SerializeField] private float positiveEffectChance = 0.6f;
    [SerializeField] private float negativeEffectChance = 0.4f;

    [Header("Points Effects")]
    [SerializeField] private int positivePointsMin = 200;
    [SerializeField] private int positivePointsMax = 500;
    [SerializeField] private int negativePointsMin = -300;
    [SerializeField] private int negativePointsMax = -100;

    [Header("Movement Effects")]
    [SerializeField] private int forwardTilesMin = 1;
    [SerializeField] private int forwardTilesMax = 3;
    [SerializeField] private int backwardTilesMin = -2;
    [SerializeField] private int backwardTilesMax = -1;

    [Header("Moves/Turns Effects")]
    [SerializeField] private int extraMovesMin = 1;
    [SerializeField] private int extraMovesMax = 2;
    [SerializeField] private int loseMovesMin = -1;
    [SerializeField] private int loseMovesMax = -1;

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
        // Wait a moment before distributing effects
        StartCoroutine(DelayedDistribution());
    }

    private IEnumerator DelayedDistribution()
    {
        yield return new WaitForSeconds(0.5f);
        DistributeEffects();
    }

    /// <summary>
    /// Distributes quiz effects across the game tiles
    /// </summary>
    public void DistributeEffects()
    {
        if (gridManager == null)
        {
            Debug.LogError("Cannot distribute effects: GridManager not found");
            return;
        }

        // Clear any existing effects
        ClearAllEffects();

        // Calculate how many tiles should have effects
        int pathLength = gridManager.PathLength;
        int effectCount = Mathf.FloorToInt(pathLength * effectChance);
        effectCount = Mathf.Max(effectCount, minEffectsOnPath);
        effectCount = Mathf.Min(effectCount, pathLength - 2); // Don't use all tiles, and leave start/end free

        Debug.Log($"Distributing {effectCount} quiz effects across {pathLength} tiles");

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

        // Assign effects to tiles
        for (int i = 0; i < validTileIndices.Count && i < effectCount; i++)
        {
            int tileIndex = validTileIndices[i];
            Tile tile = gridManager.GetTileAtIndex(tileIndex);
            if (tile == null) continue;

            // Create a new quiz effect
            CreateQuizEffectForTile(tile);
        }

        Debug.Log($"Distributed {effectCount} quiz effects");
    }

    /// <summary>
    /// Creates a quiz effect for a specific tile with randomized effect settings
    /// </summary>
    private void CreateQuizEffectForTile(Tile tile)
    {
        if (tile == null || quizEffectPrefab == null) return;

        // Create a new instance of the QuizEffect
        QuizEffect effect = Instantiate(quizEffectPrefab, tile.transform);

        // Customize the effect - we'll need to use reflection or public setters 
        // if QuizEffect doesn't expose its properties

        // For now, we'll rely on the inspector configuration of the quiz effect prefab
        // In a full implementation, you could customize each instance here

        // Store the effect
        tileEffects[tile] = effect;

        // Add listener to trigger the effect when the player lands on this tile
        tile.OnTileEntered.AddListener(() => {
            StartCoroutine(TriggerEffectWithDelay(tile, effect));
        });
    }

    /// <summary>
    /// Triggers the effect with a small delay to ensure proper game state
    /// </summary>
    private IEnumerator TriggerEffectWithDelay(Tile tile, TileEffect effect)
    {
        yield return new WaitForSeconds(quizDelayTime);

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null && !player.IsMoving)
        {
            effect.ApplyEffect(player);
        }
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
                Destroy(effect.gameObject);
            }
        }

        tileEffects.Clear();
    }

    /// <summary>
    /// Generate a random positive or negative effect value
    /// </summary>
    private int GetRandomEffectValue(bool positive)
    {
        if (positive)
        {
            float type = Random.value;
            if (type < 0.33f)
            {
                // Points
                return Random.Range(positivePointsMin, positivePointsMax + 1);
            }
            else if (type < 0.66f)
            {
                // Forward movement
                return Random.Range(forwardTilesMin, forwardTilesMax + 1);
            }
            else
            {
                // Extra moves/turns
                return Random.Range(extraMovesMin, extraMovesMax + 1);
            }
        }
        else
        {
            float type = Random.value;
            if (type < 0.33f)
            {
                // Points penalty
                return Random.Range(negativePointsMin, negativePointsMax + 1);
            }
            else if (type < 0.66f)
            {
                // Backward movement
                return Random.Range(backwardTilesMin, backwardTilesMax + 1);
            }
            else
            {
                // Lose moves/turns
                return Random.Range(loseMovesMin, loseMovesMax + 1);
            }
        }
    }
}