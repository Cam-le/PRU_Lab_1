using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UserInterfaceManager : MonoBehaviour
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


    [Header("UI Panels")]
    [SerializeField] private GameObject gameControlMapPanel;
    [SerializeField] private GameObject gameInfoPanel;
    [SerializeField] private GameObject instructionImgPanel;

    [SerializeField] private KeyCode toggleControlPanelKey = KeyCode.Tab;
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

        ShowUserInterface();
    }
    private void Update()
    {
        // Toggle control panel on hotkey press
        if (Input.GetKeyDown(toggleControlPanelKey))
        {
            ToggleControlPanel();
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
        if (AudioManager.Instance != null)
        {
            //AudioManager.Instance.PlayBackgroundMusic("mainTheme");
        }
    }


    public void ShowUserInterface()
    {
        // Activate game control and info panels 
        if (gameControlMapPanel != null)
        {
            gameControlMapPanel.SetActive(true);
        }

        if (gameInfoPanel != null)
        {
            gameInfoPanel.SetActive(true);
        }

        // Show instruction image
        if (instructionImgPanel != null)
        {
            instructionImgPanel.SetActive(true);
        }
    }



    public void ToggleControlPanel()
    {
        if (gameControlMapPanel != null)
        {
            // Toggle active state of control panel
            gameControlMapPanel.SetActive(!gameControlMapPanel.activeSelf);
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