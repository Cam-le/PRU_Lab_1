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
    
    [Header("Victory Settings")]
    [SerializeField] private string victoryTitle = "Victory!";
    [SerializeField] private string victoryDescription = "You have successfully completed Trạng Quỳnh's journey!";
    [SerializeField] private GameObject victoryEffects;
    
    [Header("Defeat Settings")]
    [SerializeField] private string defeatTitle = "Game Over";
    [SerializeField] private string defeatDescription = "You ran out of turns before reaching the destination.";
    [SerializeField] private GameObject defeatEffects;
    
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
        
        // Set the appropriate title and description
        if (gameOverTitle != null)
        {
            gameOverTitle.text = isVictory ? victoryTitle : defeatTitle;
        }
        
        if (gameOverDescription != null)
        {
            gameOverDescription.text = isVictory ? victoryDescription : defeatDescription;
        }
        
        // Show final score
        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + finalScore;
        }
        
        // Show appropriate effects
        if (victoryEffects != null)
        {
            victoryEffects.SetActive(isVictory);
        }
        
        if (defeatEffects != null)
        {
            defeatEffects.SetActive(!isVictory);
        }
        
        // Play appropriate sound effect
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            if (isVictory)
            {
                audioManager.PlaySound("victory");
            }
            else
            {
                audioManager.PlaySound("defeat");
            }
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