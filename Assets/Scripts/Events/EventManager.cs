using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Collections;

[System.Serializable]
public class GameEvent
{
    public string eventId;
    public string title;
    [TextArea(3, 10)]
    public string description;
    public Sprite eventIcon;
    public EventType type;
    public int effectValue;
    public AudioClip sound;

    // Constructor for simple event creation
    public GameEvent(string id, string eventTitle, EventType eventType, int value)
    {
        eventId = id;
        title = eventTitle;
        type = eventType;
        effectValue = value;
    }
}

public enum EventType
{
    Positive,
    Negative,
    Neutral,
    Challenge,
    Story
}

[System.Serializable]
public class EventTriggeredEvent : UnityEvent<GameEvent> { }

public class EventManager : MonoBehaviour
{
    [Header("Events")]
    public EventTriggeredEvent OnEventTriggered = new EventTriggeredEvent();

    [Header("UI References")]
    [SerializeField] private GameObject eventPanel;
    [SerializeField] private TMPro.TextMeshProUGUI eventTitle;
    [SerializeField] private TMPro.TextMeshProUGUI eventDescription;
    [SerializeField] private UnityEngine.UI.Image eventIcon;
    [SerializeField] private UnityEngine.UI.Button acceptButton;
    [SerializeField] private UnityEngine.UI.Button declineButton;

    [Header("Event Database")]
    [SerializeField] private List<GameEvent> positiveEvents = new List<GameEvent>();
    [SerializeField] private List<GameEvent> negativeEvents = new List<GameEvent>();
    [SerializeField] private List<GameEvent> neutralEvents = new List<GameEvent>();
    [SerializeField] private List<GameEvent> challengeEvents = new List<GameEvent>();
    [SerializeField] private List<GameEvent> storyEvents = new List<GameEvent>();

    // Internal tracking
    private GameEvent currentEvent;
    private System.Action onAccept;
    private System.Action onDecline;

    // Player reference
    private PlayerController player;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();

        // Set up UI events
        if (acceptButton != null)
        {
            acceptButton.onClick.AddListener(OnAcceptClicked);
        }

        if (declineButton != null)
        {
            declineButton.onClick.AddListener(OnDeclineClicked);
        }

        // Hide panel initially
        if (eventPanel != null)
        {
            eventPanel.SetActive(false);
        }
    }

    // Trigger a random event of specified type
    public void TriggerRandomEvent(EventType type)
    {
        List<GameEvent> eventList = GetEventList(type);

        if (eventList.Count == 0)
        {
            Debug.LogWarning($"No events of type {type} found");
            return;
        }

        // Select random event
        GameEvent randomEvent = eventList[Random.Range(0, eventList.Count)];
        TriggerEvent(randomEvent);
    }

    // Trigger a specific event
    public void TriggerEvent(GameEvent gameEvent)
    {
        currentEvent = gameEvent;

        // Play sound if available
        if (gameEvent.sound != null)
        {
            AudioSource.PlayClipAtPoint(gameEvent.sound, Camera.main.transform.position);
        }

        // Set up UI
        if (eventTitle != null)
        {
            eventTitle.text = gameEvent.title;
        }

        if (eventDescription != null)
        {
            eventDescription.text = gameEvent.description;
        }

        if (eventIcon != null && gameEvent.eventIcon != null)
        {
            eventIcon.sprite = gameEvent.eventIcon;
            eventIcon.enabled = true;
        }
        else if (eventIcon != null)
        {
            eventIcon.enabled = false;
        }

        // Show/hide buttons based on event type
        if (declineButton != null)
        {
            declineButton.gameObject.SetActive(gameEvent.type != EventType.Story);
        }

        // Define event handlers
        onAccept = () => ApplyEventEffect(gameEvent);
        onDecline = () => Debug.Log($"Declined event: {gameEvent.title}");

        // Show panel
        if (eventPanel != null)
        {
            eventPanel.SetActive(true);
        }

        // Trigger event
        OnEventTriggered.Invoke(gameEvent);
    }

    // Handle accept button click
    private void OnAcceptClicked()
    {
        if (onAccept != null)
        {
            onAccept();
        }

        // Hide panel
        if (eventPanel != null)
        {
            eventPanel.SetActive(false);
        }
    }

    // Handle decline button click
    private void OnDeclineClicked()
    {
        if (onDecline != null)
        {
            onDecline();
        }

        // Hide panel
        if (eventPanel != null)
        {
            eventPanel.SetActive(false);
        }
    }

    // Apply event effect
    private void ApplyEventEffect(GameEvent gameEvent)
    {
        switch (gameEvent.type)
        {
            case EventType.Positive:
                ApplyPositiveEffect(gameEvent);
                break;

            case EventType.Negative:
                ApplyNegativeEffect(gameEvent);
                break;

            case EventType.Challenge:
                StartChallenge(gameEvent);
                break;

            case EventType.Story:
                AdvanceStory(gameEvent);
                break;

            default:
                Debug.Log($"Event effect applied: {gameEvent.title}");
                break;
        }
    }

    // Apply positive event effect
    private void ApplyPositiveEffect(GameEvent gameEvent)
    {
        // Customize this based on your game's mechanics
        switch (Random.Range(0, 3))
        {
            case 0:
                // Add extra moves
                PlayerState.MovesRemaining += gameEvent.effectValue;
                Debug.Log($"Positive event: +{gameEvent.effectValue} moves");
                break;

            case 1:
                // Add score
                PlayerState.Score += gameEvent.effectValue * 10;
                Debug.Log($"Positive event: +{gameEvent.effectValue * 10} score");
                break;

            case 2:
                // Add buff
                PlayerState.AddBuff("LuckBoost", gameEvent.effectValue);
                Debug.Log($"Positive event: Luck boost for {gameEvent.effectValue} turns");
                break;
        }
    }

    // Apply negative event effect
    private void ApplyNegativeEffect(GameEvent gameEvent)
    {
        // Customize this based on your game's mechanics
        switch (Random.Range(0, 3))
        {
            case 0:
                // Lose moves
                PlayerState.MovesRemaining = Mathf.Max(0, PlayerState.MovesRemaining - gameEvent.effectValue);
                Debug.Log($"Negative event: Lost {gameEvent.effectValue} moves");
                break;

            case 1:
                // Lose score
                PlayerState.Score = Mathf.Max(0, PlayerState.Score - gameEvent.effectValue * 5);
                Debug.Log($"Negative event: Lost {gameEvent.effectValue * 5} score");
                break;

            case 2:
                // Teleport backward
                if (player != null)
                {
                    int targetIndex = Mathf.Max(0, PlayerState.CurrentTileIndex - gameEvent.effectValue);
                    player.TeleportToTile(targetIndex);
                    Debug.Log($"Negative event: Teleported backward {gameEvent.effectValue} tiles");
                }
                break;
        }
    }

    // Start a challenge
    private void StartChallenge(GameEvent gameEvent)
    {
        Debug.Log($"Starting challenge: {gameEvent.title}");

        // Save current state
        PlayerState.LastPosition = player.transform.position;
        PlayerState.CurrentPosition = new Vector2(
            player.transform.position.x,
            player.transform.position.y
        );
        PlayerState.CurrentTileIndex = PlayerState.CurrentTileIndex;
        PlayerState.ReturningFromMinigame = true;

        // Load challenge scene - this would be customized based on challenge type
        StartCoroutine(LoadChallengeScene());
    }

    private IEnumerator LoadChallengeScene()
    {
        // Wait a moment before loading
        yield return new WaitForSeconds(1.0f);

        // Determine which minigame to load
        string sceneToLoad = "MinigameScene";

        // You could have different challenge types here
        int challengeType = Random.Range(0, 3);
        switch (challengeType)
        {
            case 0:
                sceneToLoad = "PuzzleMinigame";
                break;
            case 1:
                sceneToLoad = "CardGameScene";
                break;
            case 2:
                sceneToLoad = "MemoryGameScene";
                break;
            default:
                sceneToLoad = "MinigameScene";
                break;
        }

        // Load the scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }

    // Advance story
    private void AdvanceStory(GameEvent gameEvent)
    {
        Debug.Log($"Story event: {gameEvent.title}");
        // Implement story progression
    }

    // Get event list by type
    private List<GameEvent> GetEventList(EventType type)
    {
        switch (type)
        {
            case EventType.Positive:
                return positiveEvents;
            case EventType.Negative:
                return negativeEvents;
            case EventType.Neutral:
                return neutralEvents;
            case EventType.Challenge:
                return challengeEvents;
            case EventType.Story:
                return storyEvents;
            default:
                return new List<GameEvent>();
        }
    }
}