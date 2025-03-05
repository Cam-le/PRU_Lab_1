using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MinigameManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button winButton;
    [SerializeField] private Button loseButton;
    [SerializeField] private Button returnToGameButton;

    // Add game result tracking
    public enum GameResult { None, Win, Lose }
    public static GameResult LastGameResult { get; private set; } = GameResult.None;

    // Add reward/penalty configuration
    [Header("Rewards and Penalties")]
    [SerializeField] private int winMoveBonus = 2;
    [SerializeField] private int winScoreBonus = 50;
    [SerializeField] private int loseMovePenalty = 1;
    [SerializeField] private int loseScorePenalty = 25;

    // Add tile movement rewards/penalties
    [SerializeField] private int winMoveForwardTiles = 3;
    [SerializeField] private int loseMoveBackTiles = 2;

    // Store minigame type
    public enum MinigameType { Quiz, Memory, ObjectFalling }
    [SerializeField] private MinigameType gameType;

    void Start()
    {
        if (winButton != null)
            winButton.onClick.AddListener(WinGame);

        if (loseButton != null)
            loseButton.onClick.AddListener(LoseGame);

        if (returnToGameButton != null)
            returnToGameButton.onClick.AddListener(ReturnToBoard);

        // Reset result at the start of a new minigame
        //LastGameResult = GameResult.None;

        // Reset position adjustment
        PlayerState.TileMovementAdjustment = 0;
    }

    // Call this method when player wins
    public void WinGame()
    {
        Debug.Log("Player won the minigame!");
        PlayerState.TileMovementAdjustment = winMoveForwardTiles;
        PlayerState.Score += 50; // Optional score bonus
        ReturnToBoard();
    }

    // Call this method when player loses
    public void LoseGame()
    {
        Debug.Log("Player lost the minigame!");
        PlayerState.TileMovementAdjustment = -loseMoveBackTiles;
        ReturnToBoard();
    }

    private void ApplyRewards()
    {
        // Update player state with rewards
        PlayerState.MovesRemaining += winMoveBonus;
        PlayerState.Score += winScoreBonus;

        // Set position adjustment for moving forward
        PlayerState.TileMovementAdjustment = winMoveForwardTiles;

        Debug.Log($"Applied rewards: +{winMoveBonus} moves, +{winScoreBonus} score, move forward {winMoveForwardTiles} tiles");
    }

    private void ApplyPenalties()
    {
        // Update player state with penalties
        PlayerState.MovesRemaining = Mathf.Max(0, PlayerState.MovesRemaining - loseMovePenalty);
        PlayerState.Score = Mathf.Max(0, PlayerState.Score - loseScorePenalty);

        // Set position adjustment for moving backward
        PlayerState.TileMovementAdjustment = -loseMoveBackTiles;

        Debug.Log($"Applied penalties: -{loseMovePenalty} moves, -{loseScorePenalty} score, move backward {loseMoveBackTiles} tiles");
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
        SceneManager.LoadScene("SampleScene"); // The main board scene name
    }
}
