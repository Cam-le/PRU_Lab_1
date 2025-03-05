using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MinigameManager : MonoBehaviour
{
    [SerializeField] private Button returnButton;

    // Add game result tracking
    public enum GameResult { None, Win, Lose }
    public static GameResult LastGameResult { get; private set; } = GameResult.None;

    // Add reward/penalty configuration
    [Header("Rewards and Penalties")]
    [SerializeField] private int winMoveBonus = 2;
    [SerializeField] private int winScoreBonus = 50;
    [SerializeField] private int loseMovePenalty = 1;
    [SerializeField] private int loseScorePenalty = 25;

    // Store minigame type
    public enum MinigameType { Quiz, Memory, ObjectFalling, Blackjack }
    [SerializeField] private MinigameType gameType;

    void Start()
    {
        if (returnButton != null)
        {
            returnButton.onClick.AddListener(ReturnToBoard);
        }

        // Reset result at the start of a new minigame
        LastGameResult = GameResult.None;
    }

    // Call this method when player wins
    public void WinGame()
    {
        LastGameResult = GameResult.Win;
        // Apply rewards immediately or return to board first
        ApplyRewards();
        // Optional: show win message before returning
        StartCoroutine(ShowResultAndReturn(1.5f));
    }

    // Call this method when player loses
    public void LoseGame()
    {
        LastGameResult = GameResult.Lose;
        // Apply penalties immediately or return to board first
        ApplyPenalties();
        // Optional: show lose message before returning
        StartCoroutine(ShowResultAndReturn(1.5f));
    }

    private void ApplyRewards()
    {
        // Update player state with rewards
        PlayerState.MovesRemaining += winMoveBonus;
        PlayerState.Score += winScoreBonus;

        // Add a buff if you have a buff system
        PlayerState.AddBuff("LuckBoost", 3); // 3 turns of luck boost

        Debug.Log($"Applied rewards: +{winMoveBonus} moves, +{winScoreBonus} score");
    }

    private void ApplyPenalties()
    {
        // Update player state with penalties
        PlayerState.MovesRemaining = Mathf.Max(0, PlayerState.MovesRemaining - loseMovePenalty);
        PlayerState.Score = Mathf.Max(0, PlayerState.Score - loseScorePenalty);

        Debug.Log($"Applied penalties: -{loseMovePenalty} moves, -{loseScorePenalty} score");
    }

    private IEnumerator ShowResultAndReturn(float delay)
    {
        // Wait for specified time
        yield return new WaitForSeconds(delay);

        // Return to board
        ReturnToBoard();
    }

    void ReturnToBoard()
    {
        PlayerState.ReturningFromMinigame = true;
        SceneManager.LoadScene("DemoScene"); // The main board scene name
    }
}
