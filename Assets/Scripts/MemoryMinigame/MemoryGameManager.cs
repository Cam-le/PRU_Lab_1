using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoryGameManager : MonoBehaviour
{
    [SerializeField]
    private Sprite bgImage;

    public Sprite[] puzzles;

    public List<Sprite> gamePuzzles = new();

    public List<Button> btns = new();

    private bool firstGuess, secondGuess;

    private int countGuesses;
    private int countCorrectGuesses;
    private int gameGuesses;
    private int maxGuesses;

    private int firstGuessIndex, secondGuessIndex;

    private string firstGuessPuzzle, secondGuessPuzzle;

    public GameObject gameWinPopup;

    public GameObject instructionPopup;

    public GameObject gameOverPopup;

    public GameObject gameFinishedPopup;

    public Text attemptsText;

    public Text finishedText;

    public GameObject difficultySelectionPopup;

    [SerializeField] private MinigameManager minigameManager;
    private void Awake()
    {
        //puzzles = Resources.LoadAll<Sprite>("Fruits");
        puzzles = Resources.LoadAll<Sprite>("cardGame");
    }

    // Start is called before the first frame update
    void Start()
    {
        instructionPopup.SetActive(true);
        difficultySelectionPopup.SetActive(false);
        attemptsText.enabled = false;
        gameFinishedPopup.SetActive(false);

        GetButtons();
        AddListeners();
        AddGamePuzzles();
        Shuffle(gamePuzzles);

        gameGuesses = gamePuzzles.Count / 2;

        foreach (Button btn in btns)
        {
            btn.interactable = false;
        }

        if (minigameManager == null)
        {
            minigameManager = FindObjectOfType<MinigameManager>();
            if (minigameManager == null)
            {
                Debug.LogError("No MinigameManager found in the scene!");
            }
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopSound("mainTheme");
            AudioManager.Instance.PlaySound("memoryMinigameTheme");
        }
    }

    public void ShowDifficultySelection()
    {
        Debug.Log("Hiển thị menu chọn độ khó");
        instructionPopup.SetActive(false);
        difficultySelectionPopup.SetActive(true);

        foreach (Button btn in btns)
        {
            btn.interactable = false;
        }
    }

    public void SetDifficulty(string difficulty)
    {
        switch (difficulty)
        {
            case "Easy":
                maxGuesses = 25;
                break;
            case "Normal":
                maxGuesses = 20;
                break;
            case "Hard":
                maxGuesses = 15;
                break;
        }

        attemptsText.enabled = true;
        attemptsText.text = "Lượt còn lại: " + maxGuesses;
        difficultySelectionPopup.SetActive(false);

        foreach (Button btn in btns)
        {
            btn.interactable = true;
        }
    }

    void GetButtons()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("puzzleBtn");

        for (int i = 0; i < objects.Length; i++)
        {
            btns.Add(objects[i].GetComponent<Button>());
            btns[i].image.sprite = bgImage;
        }
    }

    void AddGamePuzzles()
    {
        int looper = btns.Count;
        int index = 0;

        for (int i = 0; i < looper; i++)
        {
            if (index == looper / 2)
            {
                index = 0;
            }
            gamePuzzles.Add(puzzles[index]);
            //gamePuzzles.Add(puzzles[index]);
            index++;
        }
    }

    //void AddGamePuzzles()
    //{
    //    gamePuzzles.Clear();

    //    int totalPairs = btns.Count / 2; // Số cặp cần thiết
    //    int maxAvailablePairs = puzzles.Length; // Số hình có sẵn

    //    if (totalPairs > maxAvailablePairs)
    //    {
    //        Debug.LogWarning($"Không đủ hình ảnh. Chỉ có {maxAvailablePairs} cặp, nhưng cần {totalPairs}.");
    //        totalPairs = maxAvailablePairs;
    //    }

    //    List<Sprite> tempPuzzles = new List<Sprite>();

    //    for (int i = 0; i < totalPairs; i++)
    //    {
    //        tempPuzzles.Add(puzzles[i]);
    //        tempPuzzles.Add(puzzles[i]); // Thêm hai lần để tạo cặp
    //    }

    //    gamePuzzles = new List<Sprite>(tempPuzzles);

    //    Shuffle(gamePuzzles);

    //    // Debug danh sách gamePuzzles sau khi xáo trộn
    //    Debug.Log("Danh sách gamePuzzles sau khi shuffle:");
    //    for (int i = 0; i < gamePuzzles.Count; i++)
    //    {
    //        Debug.Log($"Index {i}: {gamePuzzles[i].name}");
    //    }
    //}


    void AddListeners()
    {
        foreach (Button btn in btns)
        {
            btn.onClick.AddListener(() => PickPuzzle());
        }
    }

    public void PickPuzzle()
    {
        //string name = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;

        if (countGuesses >= maxGuesses)
        {
            GameOver();
            return;
        }

        if (!firstGuess)
        {
            firstGuess = true;
            firstGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
            firstGuessPuzzle = gamePuzzles[firstGuessIndex].name;
            btns[firstGuessIndex].image.sprite = gamePuzzles[firstGuessIndex];
        }
        else if (!secondGuess && firstGuessIndex != int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name))
        {
            secondGuess = true;
            secondGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
            secondGuessPuzzle = gamePuzzles[secondGuessIndex].name;
            btns[secondGuessIndex].image.sprite = gamePuzzles[secondGuessIndex];

            countGuesses++;

            // Update attempts UI
            UpdateAttemptsUI();

            if (firstGuessPuzzle == secondGuessPuzzle)
            {
                print("Correct");
                //StartCoroutine(CheckIfThePuzzlesMatch());
            }
            else
            {
                print("Wrong");
                //StartCoroutine(TurnPuzzlesBack());
            }

            StartCoroutine(CheckThePuzzlesMatch());
        }
    }

    IEnumerator CheckThePuzzlesMatch()
    {
        if (firstGuess && secondGuess)
        {
            if (firstGuessPuzzle == secondGuessPuzzle)
            {
                yield return new WaitForSeconds(0.5f);
                btns[firstGuessIndex].interactable = false;
                btns[secondGuessIndex].interactable = false;

                btns[firstGuessIndex].image.color = new Color(0, 0, 0, 0);
                btns[secondGuessIndex].image.color = new Color(0, 0, 0, 0);

                countCorrectGuesses++;
                CheckTheGameFinished();
            }
            else
            {
                yield return new WaitForSeconds(1);
                btns[firstGuessIndex].image.sprite = bgImage;
                btns[secondGuessIndex].image.sprite = bgImage;
                CheckTheGameFinished();
            }

            firstGuess = secondGuess = false;
        }
    }

    void CheckTheGameFinished()
    {
        if (countCorrectGuesses == gameGuesses)
        {
            print("Game Finished: Win");
            attemptsText.enabled = false;
            gameWinPopup.SetActive(true);
            finishedText.text = "Ngươi may mắn lần này thôi.";
            gameFinishedPopup.SetActive(true);
            print("It took you " + countGuesses + " guesses to finish the game");
        }
        else if (countGuesses >= maxGuesses)
        {
            IsGameOver();
        }
    }

    public void NextBtnClick()
    {
        print("Next Button Clicked");
    }

    public void CloseInstructionPopup()
    {
        instructionPopup.SetActive(false);

        // Disable other buttons if needed
        foreach (Button btn in btns)
        {
            btn.interactable = true;
        }

        attemptsText.enabled = true;
    }

    void UpdateAttemptsUI()
    {
        int remainingAttempts = maxGuesses - countGuesses;
        attemptsText.text = "Lượt còn lại: " + remainingAttempts;
    }

    void IsGameOver()
    {
        int remainingAttempts = maxGuesses - countGuesses;

        if (remainingAttempts <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        print("Bạn đã thua! Bạn đã hết lượt lật bài.");
        attemptsText.enabled = false;
        gameOverPopup.SetActive(true); // Hiển thị popup thua cuộc
        gameFinishedPopup.SetActive(true); // Hiển thị popup kết thúc game

        // Vô hiệu hóa tất cả các nút
        foreach (Button btn in btns)
        {
            btn.interactable = false;
        }

        // Report loss to MinigameManager
        //if (minigameManager != null)
        //{
        //    minigameManager.LoseGame();
        //}
    }


    void Shuffle(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Sprite temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    //void Shuffle(List<Sprite> list)
    //{
    //    for (int i = list.Count - 1; i > 0; i--)
    //    {
    //        int randomIndex = Random.Range(0, i + 1);
    //        Sprite temp = list[i];
    //        list[i] = list[randomIndex];
    //        list[randomIndex] = temp;
    //    }
    //}

}