using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEditor.Rendering.LookDev;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private int maxTurns = 20;
    [SerializeField] private bool enableTurnLimit = true;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI turnCounterText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverMessage;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private GameObject characterPanel;
    [SerializeField] private GameObject dicePanel;
    [SerializeField] private TextMeshProUGUI keyCountText;


    [Header("Game References")]
    [SerializeField] private DiceRoller diceRoller;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GridManager gridManager;

    // Game state
    private int currentTurn = 1;
    private int playerScore = 0;
    private bool isGameOver = false;

    private void Awake()
    {
        // Initialize game state
        currentTurn = 1;
        playerScore = 0;
        isGameOver = false;

        // Load saved player state if returning from minigame
        if (PlayerState.ReturningFromMinigame)
        {
            playerScore = PlayerState.Score;
        }
        else
        {
            // Initialize PlayerState for a new game
            PlayerState.ResetState();
        }
    }

    private void Start()
    {
        // Hide game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Setup button listeners
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        // Subscribe to dice roller events
        if (diceRoller != null)
        {
            diceRoller.OnDiceRolled.AddListener(OnDiceRolled);
        }

        // Update UI
        UpdateUI();

        // Start background music
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBackgroundMusic("mainTheme");
        }

        // Check if we're coming back from the final challenge with a game result
        if (PlayerState.ShowEndGame)
        {
            // Show game over screen with appropriate result
            EndGame(PlayerState.GameWon, PlayerState.GameWon ?
                "Chiến thắng!! Trạng Quỳnh đã hoàn thành chuyến phiêu lưu của mình!" :
                "Thất bại... Trạng Quỳnh cần phải thử lại!");

            // Reset the flag
            PlayerState.ShowEndGame = false;
        }
    }

    private void Update()
    {
        // Check for game end conditions
        CheckGameEndConditions();
    }

    private void OnDiceRolled(int diceValue)
    {
        if (isGameOver) return;

        // Increment turn after dice roll
        currentTurn++;

        // Update PlayerState
        PlayerState.CurrentTurn = currentTurn;

        // Update UI
        UpdateUI();

        // Update buff durations at end of turn
        PlayerState.UpdateBuffDurations();
    }

    private void CheckGameEndConditions()
    {
        if (isGameOver) return;

        // Check if player reached the final tile
        if (playerController != null && gridManager != null)
        {
            int currentTileIndex = PlayerState.CurrentTileIndex;
            int finalTileIndex = gridManager.PathLength - 1;

            if (currentTileIndex >= finalTileIndex)
            {
                // Don't end the game here, let the final challenge scene handle it
                // This will be handled by the PlayerController's HandleFinalTile method
                return;
            }
        }

        // Check turn limit
        if (enableTurnLimit && currentTurn > maxTurns)
        {
            EndGame(false, "Thất bại...");
        }
    }

    private void EndGame(bool isVictory, string message)
    {
        isGameOver = true;

        // Stop all sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAllSounds();
        }
        if (isVictory)
        {
            // Play sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound("positiveEvent");
            }
        } else
        {
            // Play sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound("negativeEffect");
            }
        }
        // Hide Character Panel
        if (characterPanel != null)
        {
            characterPanel.SetActive(false);
        }

        // Hide Dice Panel
        if (dicePanel != null)
        {
            dicePanel.SetActive(false);
        }

        // Update UI with game over message
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (gameOverMessage != null)
        {
            gameOverMessage.text = message + "\nTổng Điểm: " + playerScore;
        }

        // Disable dice roller
        if (diceRoller != null)
        {
            diceRoller.enabled = false;
        }
    }

    private void UpdateUI()
    {
        if (turnCounterText != null)
        {
            int remainingTurns = enableTurnLimit ? (maxTurns - currentTurn + 1) : 999;
            turnCounterText.text = "Lượt: " + currentTurn + " / " + maxTurns;

            // Change color if running out of turns
            if (enableTurnLimit && remainingTurns <= 3)
            {
                turnCounterText.color = Color.red;
            }
        }

        if (scoreText != null)
        {
            scoreText.text = "Điểm số: " + playerScore;
        }

        // Update key count display
        if (keyCountText != null)
        {
            keyCountText.text = "Chìa khóa: " + PlayerState.KeyCount;
        }
    }

    // Update score from minigames
    public void AddScore(int points)
    {
        playerScore += points;
        PlayerState.Score = playerScore;
        UpdateUI();
    }

    // Set score directly
    public void SetScore(int score)
    {
        playerScore = score;
        PlayerState.Score = playerScore;
        UpdateUI();
    }

    // Button handlers
    private void RestartGame()
    {
        PlayerState.ResetState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Replace with your main menu scene name
    }
}