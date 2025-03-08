using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Handles the UI for story event interactions
/// </summary>
public class StoryEventPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Transform choicesContainer;
    [SerializeField] private Button choiceButtonPrefab;
    [SerializeField] private Button closeButton;

    [Header("Animation Settings")]
    [SerializeField] private bool animatePanel = true;
    [SerializeField] private float appearDuration = 0.7f;
    [SerializeField] private float disappearDuration = 0.5f;
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // Private variables
    private CanvasGroup canvasGroup;
    private List<Button> choiceButtons = new List<Button>();
    private Action<int> onChoiceSelected;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Setup close button
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePanel);
        }
    }

    private void OnEnable()
    {
        if (animatePanel)
        {
            StartCoroutine(AnimateAppearance());
        }
    }

    /// <summary>
    /// Initialize the story panel with content
    /// </summary>
    public void Initialize(string title, string description, Sprite background = null)
    {
        if (titleText != null)
        {
            titleText.text = title;
        }

        if (descriptionText != null)
        {
            descriptionText.text = description;
        }

        if (backgroundImage != null && background != null)
        {
            backgroundImage.sprite = background;
            backgroundImage.enabled = true;
        }
        else if (backgroundImage != null)
        {
            backgroundImage.enabled = false;
        }

        // Clear any existing choice buttons
        ClearChoices();
    }

    /// <summary>
    /// Add choices to the story panel
    /// </summary>
    public void SetChoices(string[] choiceTexts, Action<int> onChoiceCallback)
    {
        if (choicesContainer == null || choiceButtonPrefab == null) return;

        onChoiceSelected = onChoiceCallback;

        // Create buttons for each choice
        for (int i = 0; i < choiceTexts.Length; i++)
        {
            Button choiceButton = Instantiate(choiceButtonPrefab, choicesContainer);

            // Set button text
            TextMeshProUGUI buttonText = choiceButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = choiceTexts[i];
            }

            // Set button callback
            int choiceIndex = i; // Capture for lambda
            choiceButton.onClick.AddListener(() => HandleChoiceSelected(choiceIndex));

            // Add to list
            choiceButtons.Add(choiceButton);
        }
    }

    /// <summary>
    /// Handle choice button click
    /// </summary>
    private void HandleChoiceSelected(int choiceIndex)
    {
        // Disable all buttons to prevent multiple selections
        foreach (Button button in choiceButtons)
        {
            button.interactable = false;
        }

        // Invoke callback with selected choice
        onChoiceSelected?.Invoke(choiceIndex);
    }

    /// <summary>
    /// Update the description text (used for showing results after choice)
    /// </summary>
    public void UpdateDescription(string newDescription)
    {
        if (descriptionText != null)
        {
            descriptionText.text = newDescription;
        }
    }

    /// <summary>
    /// Clear all choice buttons
    /// </summary>
    public void ClearChoices()
    {
        foreach (Button button in choiceButtons)
        {
            Destroy(button.gameObject);
        }

        choiceButtons.Clear();
    }

    /// <summary>
    /// Close the panel
    /// </summary>
    public void ClosePanel()
    {
        if (animatePanel)
        {
            StartCoroutine(AnimateDisappearance());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Auto-close the panel after a delay
    /// </summary>
    public void CloseAfterDelay(float delay)
    {
        StartCoroutine(CloseAfterDelayCoroutine(delay));
    }

    /// <summary>
    /// Animate the panel appearing
    /// </summary>
    private IEnumerator AnimateAppearance()
    {
        // Set initial state
        canvasGroup.alpha = 0;
        transform.localScale = Vector3.one * 0.8f;

        float elapsed = 0f;

        while (elapsed < appearDuration)
        {
            float t = elapsed / appearDuration;
            float curvedT = animationCurve.Evaluate(t);

            // Update scale and alpha
            transform.localScale = Vector3.Lerp(Vector3.one * 0.8f, Vector3.one, curvedT);
            canvasGroup.alpha = Mathf.Lerp(0, 1, curvedT);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final state
        transform.localScale = Vector3.one;
        canvasGroup.alpha = 1;
    }

    /// <summary>
    /// Animate the panel disappearing
    /// </summary>
    private IEnumerator AnimateDisappearance()
    {
        float elapsed = 0f;

        while (elapsed < disappearDuration)
        {
            float t = elapsed / disappearDuration;
            float curvedT = animationCurve.Evaluate(t);

            // Update scale and alpha
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.8f, curvedT);
            canvasGroup.alpha = Mathf.Lerp(1, 0, curvedT);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Hide panel after animation
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Wait for delay and then close the panel
    /// </summary>
    private IEnumerator CloseAfterDelayCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        ClosePanel();
    }
}