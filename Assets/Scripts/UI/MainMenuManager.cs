using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    [Header("Transition Settings")]
    [SerializeField] private float fadeTime = 1.0f;
    [SerializeField] private GameObject fadePanel;
    [SerializeField] private string gameSceneName = "SampleScene";

    [Header("Audio")]
    [SerializeField] private AudioSource buttonSound;

    private void Start()
    {
        // Set up button listeners
        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayButtonClicked);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        }

        // Hide fade panel if it exists
        if (fadePanel != null)
        {
            fadePanel.SetActive(false);
        }
    }

    private void OnPlayButtonClicked()
    {
        // Play sound if available
        if (buttonSound != null)
        {
            buttonSound.Play();
        }

        // Start loading with fade transition
        StartCoroutine(LoadGameScene());
    }

    private void OnQuitButtonClicked()
    {
        // Play sound if available
        if (buttonSound != null)
        {
            buttonSound.Play();
        }

        // Quit application
        Debug.Log("Quitting application");

#if UNITY_EDITOR
        // If in editor, stop playing
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // In build, quit application
        Application.Quit();
#endif
    }

    private IEnumerator LoadGameScene()
    {
        // If we have a fade panel, use it for transition
        if (fadePanel != null)
        {
            fadePanel.SetActive(true);
            CanvasGroup canvasGroup = fadePanel.GetComponent<CanvasGroup>();

            if (canvasGroup != null)
            {
                // Fade in
                canvasGroup.alpha = 0;
                float elapsedTime = 0;

                while (elapsedTime < fadeTime)
                {
                    canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeTime);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                canvasGroup.alpha = 1;
            }
            else
            {
                // Simple delay if no CanvasGroup
                yield return new WaitForSeconds(fadeTime);
            }
        }
        else
        {
            // Short delay before loading
            yield return new WaitForSeconds(0.5f);
        }

        // Load the game scene
        SceneManager.LoadScene(gameSceneName);
    }
}