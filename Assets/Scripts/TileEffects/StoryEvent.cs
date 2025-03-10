using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Interactive effect that presents story choices to the player
/// </summary>
public class StoryEvent : TileEffect
{
    [System.Serializable]
    public class StoryChoice
    {
        public string choiceText;
        [TextArea(2, 5)]
        public string resultText;
        public TileEffect resultEffect;
        public AudioClip choiceSound;
    }

    [Header("Story Settings")]
    [SerializeField] private string eventTitle = "Story Event";
    [TextArea(3, 10)]
    [SerializeField] private string eventText = "You encounter something interesting...";
    [SerializeField] private List<StoryChoice> choices = new List<StoryChoice>();
    [SerializeField] private Sprite backgroundImage;
    [SerializeField] private float delayAfterChoice = 2f;

    [Header("UI References")]
    [SerializeField] private GameObject storyEventPanelPrefab;
    private GameObject storyEventPanel;
    private TextMeshProUGUI titleText;
    private TextMeshProUGUI descriptionText;
    private List<Button> choiceButtons = new List<Button>();
    private Image backgroundImageComponent;

    private PlayerController targetPlayer;

    public override bool ApplyEffect(PlayerController player)
    {
        // Call base implementation for common effect handling
        base.ApplyEffect(player);

        targetPlayer = player;

        // Create or find the story event panel
        CreateOrFindStoryPanel();

        // Populate the panel with event content
        PopulateStoryPanel();

        // Return true to indicate effect was triggered
        return true;
    }

    private void CreateOrFindStoryPanel()
    {
        // Check if we already have a story panel in the scene
        storyEventPanel = GameObject.FindGameObjectWithTag("StoryEventPanel");

        // If not, instantiate the prefab
        if (storyEventPanel == null && storyEventPanelPrefab != null)
        {
            storyEventPanel = Instantiate(storyEventPanelPrefab);
            storyEventPanel.tag = "StoryEventPanel";
        }
        else if (storyEventPanel == null)
        {
            Debug.LogError("StoryEvent: No story panel prefab assigned and none found in scene");
            return;
        }

        // Find the necessary UI components
        titleText = storyEventPanel.transform.Find("TitleText")?.GetComponent<TextMeshProUGUI>();
        descriptionText = storyEventPanel.transform.Find("DescriptionText")?.GetComponent<TextMeshProUGUI>();
        backgroundImageComponent = storyEventPanel.transform.Find("BackgroundImage")?.GetComponent<Image>();

        // Find and clear existing choice buttons
        Transform choicesContainer = storyEventPanel.transform.Find("ChoicesContainer");
        if (choicesContainer != null)
        {
            foreach (Transform child in choicesContainer)
            {
                if (child.GetComponent<Button>() != null)
                {
                    choiceButtons.Add(child.GetComponent<Button>());
                }
            }
        }

        // Clear any existing listeners
        foreach (Button button in choiceButtons)
        {
            button.onClick.RemoveAllListeners();
            button.gameObject.SetActive(false);
        }

        // Activate the panel
        storyEventPanel.SetActive(true);
    }

    private void PopulateStoryPanel()
    {
        // Set title and description
        if (titleText != null)
            titleText.text = eventTitle;

        if (descriptionText != null)
            descriptionText.text = eventText;

        // Set background image if available
        if (backgroundImageComponent != null && backgroundImage != null)
        {
            backgroundImageComponent.sprite = backgroundImage;
            backgroundImageComponent.enabled = true;
        }
        else if (backgroundImageComponent != null)
        {
            backgroundImageComponent.enabled = false;
        }

        // Set up choice buttons
        for (int i = 0; i < choices.Count && i < choiceButtons.Count; i++)
        {
            Button button = choiceButtons[i];
            StoryChoice choice = choices[i];

            // Update button text
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
                buttonText.text = choice.choiceText;

            // Make button active
            button.gameObject.SetActive(true);

            // Capture index for closure
            int choiceIndex = i;

            // Set click handler
            button.onClick.AddListener(() => HandleChoice(choiceIndex));
        }
    }

    private void HandleChoice(int choiceIndex)
    {
        if (choiceIndex < 0 || choiceIndex >= choices.Count)
            return;

        StoryChoice selectedChoice = choices[choiceIndex];

        // Update description to show result text
        if (descriptionText != null)
            descriptionText.text = selectedChoice.resultText;

        // Disable all buttons
        foreach (Button button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }

        // Play choice sound if available
        if (selectedChoice.choiceSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(selectedChoice.choiceSound.name);
        }

        // Apply the choice's effect if one exists
        if (selectedChoice.resultEffect != null && targetPlayer != null)
        {
            selectedChoice.resultEffect.ApplyEffect(targetPlayer);
        }

        // Set up automatic closing of the story panel after delay
        if (storyEventPanel != null)
        {
            Invoke("CloseStoryPanel", delayAfterChoice);
        }
    }

    private void CloseStoryPanel()
    {
        if (storyEventPanel != null)
        {
            storyEventPanel.SetActive(false);
        }
    }

    public override string GetEffectSummary()
    {
        return $"Story Event: {eventTitle} ({choices.Count} choices)";
    }
}