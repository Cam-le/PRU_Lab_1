using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    HPManager hPManager;
    [HideInInspector] public int score;

    [HideInInspector] public bool gameOver;

    [HideInInspector] public bool gameWin;
    
    [HideInInspector] public int hitPoint;

    [HideInInspector] public bool isTutorial;

    // Support ending game
    [HideInInspector] public bool hasReportedGameResult = false;

    [SerializeField] private MinigameManager minigameManager;

    // UI Elements for win/lose popups
    [Header("Game Result UI")]
    [SerializeField] private GameObject winPopup;
    [SerializeField] private GameObject losePopup;
    [SerializeField] private TMP_Text finalScoreText;

    [SerializeField] private Transform overTitle;
    [SerializeField] private TMP_Text winTitle;

    void Start()
    {
        hPManager = GameObject.Find("Scripts").GetComponent<HPManager>();
        score = 0;
        gameOver = false;
        gameWin = false;
        isTutorial = true;

        hasReportedGameResult = false;

        // Hide popups at start
        if (winPopup != null) winPopup.SetActive(false);
        if (losePopup != null) losePopup.SetActive(false);

        // Find MinigameManager if not assigned
        if (minigameManager == null)
        {
            minigameManager = FindObjectOfType<MinigameManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver && !hasReportedGameResult)
        {
            GameLost();
        }

        if (gameWin && !hasReportedGameResult)
        {
            GameWon();
        }
    }

    public void AddScore(int additionCcore)
    {
        score += additionCcore;
        if(score <= 0)
        {
            score = 0;
        }
    }

    public int ExtractLifePoint(int point)
    {
        hitPoint -= point;
        hPManager.LoseHP();
        return hitPoint;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 70, 160, 30), "Điểm: " + score);  // Hiển thị thời gian còn lại
    }

    private void GameWon()
    {
        PauseAllGameObjects();

        // Update UI
        if (finalScoreText != null)
        {
            finalScoreText.text = $"Điểm của bạn: {score}";
        }

        // Show win popup
        if (winPopup != null)
        {
            winPopup.SetActive(true);
        }
        else
        {
            winTitle.text = $"Chúc mừng, bạn đã qua ải!\nĐiểm của bạn: {score}";
            winTitle.gameObject.SetActive(true);
        }

        // Report win to MinigameManager with delay
        if (minigameManager != null)
        {
            Invoke("ReportWin", 2.0f); // Delay return by 2 seconds
        }

        hasReportedGameResult = true;
    }

    private void GameLost()
    {
        PauseAllGameObjects();

        // Update UI
        if (finalScoreText != null)
        {
            finalScoreText.text = $"Điểm của bạn: {score}";
        }

        // Show lose popup
        if (losePopup != null)
        {
            losePopup.SetActive(true);
        }
        else
        {
            overTitle.localPosition = new Vector3(0f, 0f, 0f);
        }

        // Report loss to MinigameManager with delay
        if (minigameManager != null)
        {
            Invoke("ReportLoss", 2.0f); // Delay return by 2 seconds
        }

        hasReportedGameResult = true;
    }

    private void ReportWin()
    {
        minigameManager.WinGame();
    }

    private void ReportLoss()
    {
        minigameManager.LoseGame();
    }

    private void PauseAllGameObjects()
    {
        // Stop all generators and moving objects
        Generator[] generators = FindObjectsOfType<Generator>();
        foreach (Generator generator in generators)
        {
            generator.enabled = false;
        }

        // Stop character movement
        Character playerCharacter = FindObjectOfType<Character>();
        if (playerCharacter != null)
        {
            playerCharacter.enabled = false;
        }

        // Stop all falling objects
        PauseAllFallingObjects();
    }

    private void PauseAllFallingObjects()
    {
        // Stop all fruits
        PlusScoreObject[] fruits = FindObjectsOfType<PlusScoreObject>();
        foreach (PlusScoreObject fruit in fruits)
        {
            fruit.enabled = false;
        }

        // Stop all animals
        Animal[] animals = FindObjectsOfType<Animal>();
        foreach (Animal animal in animals)
        {
            animal.enabled = false;
        }

        // Stop all knives
        Knife[] knives = FindObjectsOfType<Knife>();
        foreach (Knife knife in knives)
        {
            knife.enabled = false;
        }
    }
}
