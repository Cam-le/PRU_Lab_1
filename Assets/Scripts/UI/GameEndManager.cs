using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameEndManager : MonoBehaviour
{
    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverTitle;
    [SerializeField] private TextMeshProUGUI gameOverDescription;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    [Header("Victory/Defeat Messages")]
    [SerializeField] private string victoryTitle = "Chiến thắng!!";
    [SerializeField] private string victoryDescription = "Trạng Quỳnh đã hoàn thành chuyến phiêu lưu của mình";
    [SerializeField] private string defeatTitle = "Thất bại...";
    [SerializeField] private string defeatDescription = "Bạn hết lượt mất rồi";

    [Header("Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    private void Start()
    {
        // Hide game over panel initially
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
    }

    public void ShowGameOver(bool isVictory, int finalScore)
    {
        // Show the game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Set appropriate text based on result
        if (gameOverTitle != null)
        {
            gameOverTitle.text = isVictory ? victoryTitle : defeatTitle;
        }

        if (gameOverDescription != null)
        {
            gameOverDescription.text = isVictory ? victoryDescription : defeatDescription;
        }

        // Update final score text
        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + finalScore;
        }

        // Play appropriate sound effect
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlaySound(isVictory ? "victory" : "defeat");
        }
    }

    private void RestartGame()
    {
        // Reset player state and reload the current scene
        PlayerState.ResetState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GoToMainMenu()
    {
        // Load the main menu scene
        SceneManager.LoadScene("MainMenu"); // Replace with your main menu scene name
    }
}