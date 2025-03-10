using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages character dialogue and message display
/// </summary>
public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialogueBubble;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image characterPortrait;
    [SerializeField] private GameObject portraitPanel;

    [Header("Settings")]
    [SerializeField] private float displayDuration = 3.5f;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private bool useTypewriterEffect = true;
    [SerializeField] private AudioClip typingSoundEffect;
    [SerializeField] private float typingSoundInterval = 0.1f;

    [Header("Message Lists")]
    [SerializeField]
    private List<string> positiveMessages = new List<string>
    {
        "Thật tuyệt vời!",
        "May mắn thay!",
        "Cơ hội tốt!",
        "Thành công rồi!",
        "Hên quá!",
        "Trời phù hộ rồi!"
    };

    [SerializeField]
    private List<string> negativeMessages = new List<string>
    {
        "Không may rồi!",
        "Thật đáng tiếc!",
        "Xui quá!",
        "Không như mong đợi!",
        "Thật không may!",
        "Phải cẩn thận hơn!"
    };

    [SerializeField]
    private List<string> neutralMessages = new List<string>
    {
        "Hmm, thú vị đấy!",
        "Chuyện gì thế này?",
        "Đây là điều mới mẻ!",
        "Lạ thật!",
        "Đáng suy nghĩ!"
    };

    // Internal variables
    private Coroutine currentDisplayCoroutine;
    private float lastTypingSoundTime;

    private void Awake()
    {
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
        // Hide dialogue bubble initially
        if (dialogueBubble != null)
        {
            dialogueBubble.SetActive(false);
        }
    }

    /// <summary>
    /// Handle effect triggered event
    /// </summary>
    private void HandleEffectTriggered(TileEffect effect, PlayerController player)
    {
        if (effect == null) return;

        // Determine message type based on effect category
        string message = "";

        // Check if this is a positive, negative, or neutral effect
        if (effect.GetType() == typeof(ResourceEffect))
        {
            ResourceEffect resourceEffect = (ResourceEffect)effect;
            string summary = resourceEffect.GetEffectSummary();

            if (summary.Contains("+"))
            {
                message = GetRandomMessage(positiveMessages);
            }
            else if (summary.Contains("-"))
            {
                message = GetRandomMessage(negativeMessages);
            }
            else
            {
                message = GetRandomMessage(neutralMessages);
            }
        }
        else if (effect.GetType() == typeof(MovementEffect))
        {
            MovementEffect movementEffect = (MovementEffect)effect;
            string summary = movementEffect.GetEffectSummary();

            if (summary.Contains("forward") || summary.Contains("extra"))
            {
                message = GetRandomMessage(positiveMessages);
            }
            else if (summary.Contains("backward") || summary.Contains("skip"))
            {
                message = GetRandomMessage(negativeMessages);
            }
            else
            {
                message = GetRandomMessage(neutralMessages);
            }
        }
        else
        {
            // Default to neutral for other effect types
            message = GetRandomMessage(neutralMessages);
        }

        // Show the message
        if (!string.IsNullOrEmpty(message))
        {
            ShowMessage(message);
        }
    }

    /// <summary>
    /// Display a message in the dialogue bubble
    /// </summary>
    public void ShowMessage(string message)
    {
        // Stop any currently displaying message
        if (currentDisplayCoroutine != null)
        {
            StopCoroutine(currentDisplayCoroutine);
        }

        // Start new message display
        currentDisplayCoroutine = StartCoroutine(DisplayMessageCoroutine(message));
    }

    /// <summary>
    /// Get a random message from the provided list
    /// </summary>
    private string GetRandomMessage(List<string> messages)
    {
        if (messages == null || messages.Count == 0)
            return "";

        int index = Random.Range(0, messages.Count);
        return messages[index];
    }

    /// <summary>
    /// Display message with typewriter effect and auto-hide
    /// </summary>
    private IEnumerator DisplayMessageCoroutine(string message)
    {
        // Show the dialogue bubble
        if (dialogueBubble != null)
        {
            dialogueBubble.SetActive(true);
        }

        if (portraitPanel != null)
        {
            portraitPanel.SetActive(true);
        }

        if (useTypewriterEffect && dialogueText != null)
        {
            // Clear the text
            dialogueText.text = "";

            // Type out the message one character at a time
            for (int i = 0; i < message.Length; i++)
            {
                dialogueText.text += message[i];

                // Play typing sound effect at intervals
                if (typingSoundEffect != null && Time.time - lastTypingSoundTime >= typingSoundInterval)
                {
                    lastTypingSoundTime = Time.time;
                    AudioSource.PlayClipAtPoint(typingSoundEffect, Camera.main.transform.position, 0.5f);
                }

                yield return new WaitForSeconds(typingSpeed);
            }
        }
        else if (dialogueText != null)
        {
            // Just set the text immediately
            dialogueText.text = message;
        }

        // Wait for display duration
        yield return new WaitForSeconds(displayDuration);

        // Hide the dialogue bubble
        if (dialogueBubble != null)
        {
            dialogueBubble.SetActive(false);
        }

        currentDisplayCoroutine = null;
    }

    /// <summary>
    /// Public method to add a custom message to a list
    /// </summary>
    public void AddCustomMessage(string message, string messageType)
    {
        switch (messageType.ToLower())
        {
            case "positive":
                positiveMessages.Add(message);
                break;
            case "negative":
                negativeMessages.Add(message);
                break;
            case "neutral":
                neutralMessages.Add(message);
                break;
        }
    }
}