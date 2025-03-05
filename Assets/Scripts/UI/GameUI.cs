using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("Game Info Panel")]
    [SerializeField] private GameObject gameInfoPanel;
    [SerializeField] private TextMeshProUGUI turnCounterText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverMessage;
    [SerializeField] private GameObject victoryIcon;
    [SerializeField] private GameObject defeatIcon;

    private void Start()
    {
        // Show game info panel
        if (gameInfoPanel != null)
        {
            gameInfoPanel.SetActive(true);
        }

        // Hide game over panel initially
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    // Public methods to update UI elements

    public void UpdateTurnCounter(int currentTurn, int maxTurns)
    {
        if (turnCounterText != null)
        {
            turnCounterText.text = "Turn: " + currentTurn + " / " + maxTurns;

            // Change color if running out of turns
            if (currentTurn >= maxTurns - 2)
            {
                turnCounterText.color = Color.red;
            }
            else
            {
                turnCounterText.color = Color.white;
            }
        }
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    public void ShowGameOver(bool isVictory, string message, int finalScore)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (gameOverMessage != null)
        {
            gameOverMessage.text = message + "\n\nFinal Score: " + finalScore;
        }

        if (victoryIcon != null)
        {
            victoryIcon.SetActive(isVictory);
        }

        if (defeatIcon != null)
        {
            defeatIcon.SetActive(!isVictory);
        }
    }
}