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

    private int firstGuessIndex, secondGuessIndex;

    private string firstGuessPuzzle, secondGuessPuzzle;

    public GameObject GameWinPopup;

    private void Awake()
    {
        //puzzles = Resources.LoadAll<Sprite>("Fruits");
        puzzles = Resources.LoadAll<Sprite>("cardGame");
        GetButtons();
    }

    // Start is called before the first frame update
    void Start()
    {
        GetButtons();
        AddListeners();
        AddGamePuzzles();
        Shuffle(gamePuzzles);

        gameGuesses = gamePuzzles.Count / 2;
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
            index++;
        }
    }

    //void AddGamePuzzles()
    //{
    //    // Calculate how many unique pairs we need
    //    int maxPairs = puzzles.Length; // Maximum possible pairs based on available sprites
    //    int looper = btns.Count;
    //    int index = 0;

    //    // Ensure we don't exceed available sprites
    //    if (looper / 2 > maxPairs)
    //    {
    //        Debug.LogWarning($"Not enough sprite pairs available. Have {maxPairs} pairs, but UI has {looper / 2} pairs.");
    //        looper = maxPairs * 2;
    //    }

    //    for (int i = 0; i < looper; i++)
    //    {
    //        if (index == looper / 2)
    //        {
    //            index = 0;
    //        }
    //        gamePuzzles.Add(puzzles[index]);
    //        index++;
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

        if (!firstGuess)
        {
            firstGuess = true;
            firstGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
            firstGuessPuzzle = gamePuzzles[firstGuessIndex].name;
            btns[firstGuessIndex].image.sprite = gamePuzzles[firstGuessIndex];
        }
        else if (!secondGuess)
        {
            secondGuess = true;
            secondGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
            secondGuessPuzzle = gamePuzzles[secondGuessIndex].name;
            btns[secondGuessIndex].image.sprite = gamePuzzles[secondGuessIndex];

            countGuesses++;

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
        yield return new WaitForSeconds(0.25f);

        if (firstGuessPuzzle == secondGuessPuzzle)
        {
            yield return new WaitForSeconds(0.25f);
            btns[firstGuessIndex].interactable = false;
            btns[secondGuessIndex].interactable = false;

            btns[firstGuessIndex].image.color = new Color(0, 0, 0, 0);
            btns[secondGuessIndex].image.color = new Color(0, 0, 0, 0);

            CheckTheGameIsFinished();
        }
        else
        {
            btns[firstGuessIndex].image.sprite = bgImage;
            btns[secondGuessIndex].image.sprite = bgImage;
        }

        yield return new WaitForSeconds(0.25f);

        firstGuess = secondGuess = false;
    }

    void CheckTheGameIsFinished()
    {
        countCorrectGuesses++;
        if (countCorrectGuesses == gameGuesses)
        {
            print("Game Finished");
            GameWinPopup.SetActive(true);
            print("It took you " + countGuesses + " guesses to finish the game");
        }
    }

    public void NextBtnClick()
    {
        print("Next Button Clicked");
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
}