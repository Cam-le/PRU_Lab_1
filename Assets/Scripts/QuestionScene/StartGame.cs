using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "QuestionGame"; 

    public void StartGameScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
