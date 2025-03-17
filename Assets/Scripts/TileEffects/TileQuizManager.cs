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
    questionText = "Uống nước nhớ ...?",
    answers = new string[] { "uống từ từ", "nguồn", "đầy đủ", "sạch sẽ" },
    correctAnswerIndex = 1
},
new Question {
    questionText = "Đi một ngày đàng, học một ...?",
    answers = new string[] { "điều hay", "sàng khôn", "kinh nghiệm", "bài học" },
    correctAnswerIndex = 1
},
new Question {
    questionText = "Tiên học lễ, hậu học ...?",
    answers = new string[] { "văn", "lý", "sử", "địa" },
    correctAnswerIndex = 0
},
new Question {
    questionText = "Cây ngay không sợ ...?",
    answers = new string[] { "gió bão", "chết đứng", "chết khô", "đổ ngã" },
    correctAnswerIndex = 1
},
new Question {
    questionText = "Ăn quả nhớ ...?",
    answers = new string[] { "kẻ trồng cây", "người gieo", "kẻ hái", "người bán" },
    correctAnswerIndex = 0
},
new Question {
    questionText = "Gần mực thì ...?",
    answers = new string[] { "sạch sẽ", "đen", "trắng", "sáng" },
    correctAnswerIndex = 1
},
new Question {
    questionText = "Có công mài sắt, có ngày ...?",
    answers = new string[] { "rỉ sét", "lấp lánh", "nát vụn", "thành kim" },
    correctAnswerIndex = 3
},
new Question {
    questionText = "Nhất nước, nhì ...?",
    answers = new string[] { "nhà", "đất", "phân", "kim" },
    correctAnswerIndex = 2
},
new Question {
    questionText = "Lá lành đùm ...?",
    answers = new string[] { "lá rách", "lá xanh", "lá khô", "lá vàng" },
    correctAnswerIndex = 0
},
new Question {
    questionText = "Thất bại là mẹ của ...?",
    answers = new string[] { "thành công", "nản chí", "thất vọng", "bỏ cuộc" },
    correctAnswerIndex = 0
},
new Question {
    questionText = "Một con ngựa đau, cả tàu ...?",
    answers = new string[] { "bỏ cỏ", "chuồng bỏ", "đàn nghỉ", "chuồng buồn" },
    correctAnswerIndex = 0
},
new Question {
    questionText = "Trâu buộc thì ... trâu ăn?",
    answers = new string[] { "khác", "giống", "ghét", "ngựa" },
    correctAnswerIndex = 2
},
new Question {
    questionText = "Nhàn cư vi ...?",
    answers = new string[] { "chết", "lợi", "bất thiện", "hại" },
    correctAnswerIndex = 2
},
new Question {
    questionText = "Cẩn tắc vô ...?",
    answers = new string[] { "lợi", "áy náy", "ái", "ái" },
    correctAnswerIndex = 1
},
new Question {
    questionText = "Ở hiền thì gặp ...?",
    answers = new string[] { "lành", "dữ", "khó", "mệt" },
    correctAnswerIndex = 0
},
new Question {
    questionText = "Lời nói chẳng mất ... mua?",
    answers = new string[] { "tiền", "lời", "mất", "nghĩa" },
    correctAnswerIndex = 0
},
new Question {
    questionText = "Tấc đất, tấc ...?",
    answers = new string[] { "vàng", "bạc", "đồng", "đất" },
    correctAnswerIndex = 0
},
new Question {
    questionText = "Đừng thấy sóng cả mà ... tay chèo?",
    answers = new string[] { "chèo", "ngã", "đổ", "ngại" },
    correctAnswerIndex = 1
},
new Question {
    questionText = "Trời sinh voi, trời sinh ...?",
    answers = new string[] { "hổ", "sư tử", "cỏ", "ngựa" },
    correctAnswerIndex = 2
},
new Question {
    questionText = "Làm ít mà được ...?",
    answers = new string[] { "nhiều", "ít", "không", "được" },
    correctAnswerIndex = 0
},
new Question {
    questionText = "Học ăn, học ...?",
    answers = new string[] { "nói", "ngủ", "chơi", "đi" },
    correctAnswerIndex = 0
},
new Question {
    questionText = "Bán anh em xa, mua ... gần?",
    answers = new string[] { "hàng xóm", "láng giềng", "bạn bè", "vợ" },
    correctAnswerIndex = 1
},
new Question {
    questionText = "Đừng để nước đến ...?",
    answers = new string[] { "chân", "cổ", "đầu", "mắt" },
    correctAnswerIndex = 0
},
new Question {
    questionText = "Con hơn cha là ...?",
    answers = new string[] { "nhà có phúc", "nhà có phước", "nhà có lộc", "nhà có tài" },
    correctAnswerIndex = 0
},
new Question {
    questionText = "Thời gian là ...?",
    answers = new string[] { "vàng", "bạc", "đồng", "sắt" },
    correctAnswerIndex = 0
},
new Question {
    questionText = "Kính lão đắc ...?",
    answers = new string[] { "hậu", "phúc", "lộc", "thọ" },
    correctAnswerIndex = 3
},
new Question {
    questionText = "Nhiễu điều phủ lấy ...?",
    answers = new string[] { "giá gương", "người thương", "lá sương", "người tần" },
    correctAnswerIndex = 0
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