using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MinigameManager : MonoBehaviour
{
    [SerializeField] private Button returnButton;

    void Start()
    {
        if (returnButton != null)
        {
            returnButton.onClick.AddListener(ReturnToBoard);
        }
    }

    void ReturnToBoard()
    {
        PlayerState.ReturningFromMinigame = true;
        SceneManager.LoadScene("DemoScene"); // The main board scene name
    }
}