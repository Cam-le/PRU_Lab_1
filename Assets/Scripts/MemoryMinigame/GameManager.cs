using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
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
    private int maxGuesses = 10;

    private int firstGuessIndex, secondGuessIndex;

    private string firstGuessPuzzle, secondGuessPuzzle;

    public GameObject gameWinPopup;

    public GameObject instructionPopup;

    public GameObject gameOverPopup;

    public Text attemptsText;

    private void Awake()
    {
        //puzzles = Resources.LoadAll<Sprite>("Fruits");
        puzzles = Resources.LoadAll<Sprite>("cardGame");
    }

    // Start is called before the first frame update
    void Start()
    {
        instructionPopup.SetActive(true);
        attemptsText.enabled = false;

        GetButtons();
        AddListeners();
        AddGamePuzzles();
        Shuffle(gamePuzzles);
        //DebugGamePuzzles();
        //DebugLoadedSprites();

        gameGuesses = gamePuzzles.Count / 2;

        // Disable other buttons if needed
        foreach (Button btn in btns)
        {
            btn.interactable = false;
        }
    }

    void DebugGamePuzzles()
    {
        for (int i = 0; i < gamePuzzles.Count; i++)
        {
            Debug.Log($"Index {i}: {gamePuzzles[i].name}");
        }
    }

    void DebugLoadedSprites()
    {
        Debug.Log("Tổng số hình ảnh load được: " + puzzles.Length);
        for (int i = 0; i < puzzles.Length; i++)
        {
            Debug.Log($"Hình {i}: {puzzles[i].name}");
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

        if(countGuesses >= maxGuesses)
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

                CheckTheGameIsFinished();
            }
            else
            {
                yield return new WaitForSeconds(1.5f);
                btns[firstGuessIndex].image.sprite = bgImage;
                btns[secondGuessIndex].image.sprite = bgImage;
            }

            firstGuess = secondGuess = false;
        }
    }

    void CheckTheGameIsFinished()
    {
        countCorrectGuesses++;
        if (countCorrectGuesses == gameGuesses)
        {
            print("Game Finished");
            gameWinPopup.SetActive(true);
            print("It took you " + countGuesses + " guesses to finish the game");
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

        if (remainingAttempts <= 0)
        {
            GameOver();
        }
    }


    void GameOver()
    {
        print("Bạn đã thua! Bạn đã hết lượt lật bài.");
        gameOverPopup.SetActive(true); // Hiển thị popup thua cuộc

        // Vô hiệu hóa tất cả các nút
        foreach (Button btn in btns)
        {
            btn.interactable = false;
        }
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