using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class TileQuizManager : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string questionText;
        public string[] answers = new string[4];
        public int correctAnswerIndex;
    }

    [Header("UI References")]
    // Changed from private to public to fix CS0122 errors
    public GameObject quizPanel;
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;

    [Header("Question Database")]
    [SerializeField]
    private List<Question> questions = new List<Question>()
    {
        new Question {
            questionText = "Ăn quả nhớ kẻ ...?",
            answers = new string[] { "trồng cây", "bán hàng", "hái trái", "chăm sóc" },
            correctAnswerIndex = 0
        },
        new Question {
            questionText = "Có công mài sắt, có ngày nên ...?",
            answers = new string[] { "giàu", "kim", "giỏi", "người" },
            correctAnswerIndex = 1
        },
        new Question {
            questionText = "Một cây làm chẳng nên ...?",
            answers = new string[] { "gì", "rừng", "non", "cây" },
            correctAnswerIndex = 1
        },
        new Question {
            questionText = "Uống nước nhớ ...?",
            answers = new string[] { "uống từ từ", "nguồn", "đầy đủ", "sạch sẽ" },
            correctAnswerIndex = 1
        },
        new Question {
            questionText = "Đi một ngày đàng, học một ...?",
            answers = new string[] { "điều hay", "sàng khôn", "kinh nghiệm", "bài học" },
            correctAnswerIndex = 1
        }
    };

    [Header("Effect Settings")]
    [SerializeField] private int pointsForCorrect = 300;
    [SerializeField] private int pointsForIncorrect = -150;
    [SerializeField] private int movesForCorrect = 1;
    [SerializeField] private int movesForIncorrect = -1;
    [SerializeField] private int forwardTilesForCorrect = 2;
    [SerializeField] private int backwardTilesForIncorrect = -1;

    private Question currentQuestion;
    private Action<bool, TileEffect.EffectType, int> onQuizCompleted;
    private bool isQuizActive = false;

    private void Awake()
    {
        // Hide panels initially
        if (quizPanel != null)
            quizPanel.SetActive(false);

        if (resultPanel != null)
            resultPanel.SetActive(false);
    }

    public void ShowQuiz(Action<bool, TileEffect.EffectType, int> callback)
    {
        if (isQuizActive)
            return;

        onQuizCompleted = callback;
        isQuizActive = true;

        // Pick a random question
        currentQuestion = questions[UnityEngine.Random.Range(0, questions.Count)];

        // Setup the UI
        questionText.text = currentQuestion.questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < currentQuestion.answers.Length)
            {
                TextMeshProUGUI buttonText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                    buttonText.text = currentQuestion.answers[i];

                int answerIndex = i; // Need to store for lambda
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => SelectAnswer(answerIndex));
                answerButtons[i].gameObject.SetActive(true);
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }

        // Show the quiz panel
        quizPanel.SetActive(true);
    }

    private void SelectAnswer(int selectedIndex)
    {
        bool isCorrect = (selectedIndex == currentQuestion.correctAnswerIndex);

        // Show result
        resultPanel.SetActive(true);
        resultText.text = isCorrect ? "Đúng rồi!" : "Sai rồi!";
        resultText.color = isCorrect ? Color.green : Color.red;

        // Wait for a moment, then hide all UI
        Invoke("CompleteQuiz", 1.5f);

        // Determine which effect to apply based on correct/incorrect
        TileEffect.EffectType effectType;
        int effectValue;

        if (isCorrect)
        {
            // Select a random positive effect
            int effectChoice = UnityEngine.Random.Range(0, 3);
            switch (effectChoice)
            {
                case 0:
                    effectType = TileEffect.EffectType.Points;
                    effectValue = pointsForCorrect;
                    break;
                case 1:
                    effectType = TileEffect.EffectType.Moves;
                    effectValue = movesForCorrect;
                    break;
                case 2:
                    effectType = TileEffect.EffectType.MoveForward;
                    effectValue = forwardTilesForCorrect;
                    break;
                default:
                    effectType = TileEffect.EffectType.Points;
                    effectValue = pointsForCorrect;
                    break;
            }
        }
        else
        {
            // Select a random negative effect
            int effectChoice = UnityEngine.Random.Range(0, 3);
            switch (effectChoice)
            {
                case 0:
                    effectType = TileEffect.EffectType.Points;
                    effectValue = pointsForIncorrect;
                    break;
                case 1:
                    effectType = TileEffect.EffectType.Moves;
                    effectValue = movesForIncorrect;
                    break;
                case 2:
                    effectType = TileEffect.EffectType.MoveBackward;
                    effectValue = backwardTilesForIncorrect;
                    break;
                default:
                    effectType = TileEffect.EffectType.Points;
                    effectValue = pointsForIncorrect;
                    break;
            }
        }

        // Call the callback with the result
        if (onQuizCompleted != null)
        {
            onQuizCompleted(isCorrect, effectType, effectValue);
        }
    }

    private void CompleteQuiz()
    {
        quizPanel.SetActive(false);
        resultPanel.SetActive(false);
        isQuizActive = false;
    }
}