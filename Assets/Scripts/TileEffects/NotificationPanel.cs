using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Handles the UI for a single notification panel
/// </summary>
public class NotificationPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Button closeButton;

    [Header("Animation Settings")]
    [SerializeField] private bool animateAppearance = true;
    [SerializeField] private float appearDuration = 0.5f;
    [SerializeField] private float disappearDuration = 0.3f;
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private Vector2 startPosition = new Vector2(300, 0);
    [SerializeField] private Vector2 endPosition = Vector2.zero;

    [Header("Auto-Close Settings")]
    [SerializeField] private bool autoClose = true;
    [SerializeField] private float displayDuration = 3.0f;

    // References
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Setup close button
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseNotification);
        }
    }

    private void OnEnable()
    {
        if (animateAppearance)
        {
            StartCoroutine(AnimateAppearance());
        }

        if (autoClose)
        {
            StartCoroutine(AutoCloseAfterDelay());
        }
    }

    /// <summary>
    /// Initialize the notification with content
    /// </summary>
    public void Initialize(string title, string message, Sprite icon = null, Color color = default)
    {
        if (titleText != null)
        {
            titleText.text = title;
        }

        if (messageText != null)
        {
            messageText.text = message;
        }

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

        if (backgroundImage != null && color != default)
        {
            // Use a semi-transparent version of the color
            Color bgColor = new Color(color.r, color.g, color.b, 0.8f);
            backgroundImage.color = bgColor;
        }
    }

    /// <summary>
    /// Close the notification
    /// </summary>
    public void CloseNotification()
    {
        if (animateAppearance)
        {
            StartCoroutine(AnimateDisappearance());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Animate the notification appearing
    /// </summary>
    private IEnumerator AnimateAppearance()
    {
        // Set initial state
        canvasGroup.alpha = 0;
        rectTransform.anchoredPosition = startPosition;

        float elapsed = 0f;

        while (elapsed < appearDuration)
        {
            float t = elapsed / appearDuration;
            float curvedT = animationCurve.Evaluate(t);

            // Update position and alpha
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, curvedT);
            canvasGroup.alpha = Mathf.Lerp(0, 1, curvedT);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final state
        rectTransform.anchoredPosition = endPosition;
        canvasGroup.alpha = 1;
    }

    /// <summary>
    /// Animate the notification disappearing
    /// </summary>
    private IEnumerator AnimateDisappearance()
    {
        float elapsed = 0f;

        while (elapsed < disappearDuration)
        {
            float t = elapsed / disappearDuration;
            float curvedT = animationCurve.Evaluate(t);

            // Update position and alpha
            rectTransform.anchoredPosition = Vector2.Lerp(endPosition, startPosition, curvedT);
            canvasGroup.alpha = Mathf.Lerp(1, 0, curvedT);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Destroy after animation
        Destroy(gameObject);
    }

    /// <summary>
    /// Auto-close the notification after a delay
    /// </summary>
    private IEnumerator AutoCloseAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        CloseNotification();
    }
}