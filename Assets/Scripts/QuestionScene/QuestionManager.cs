using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class QuestionManager : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string questionText;
        public string[] answers;
        public int correctAnswerIndex;
    }

    [Header("UI Elements")]
    public TMP_Text questionText;
    public Button[] answerButtons;
    public GameObject failScreen;
    public GameObject successScreen;
    public Button retryButton;

    [Header("Question Data")]
    public List<Question> questions = new List<Question>
    {
        new Question { questionText = "Ăn quả nhớ ...?", answers = new string[] { "kẻ trồng cây", "người bán cây", "người hái quả", "người tưới cây" }, correctAnswerIndex = 0 },
new Question { questionText = "Có công mài sắt, có ngày nên ...?", answers = new string[] { "nhà", "kim", "đá", "đồng" }, correctAnswerIndex = 1 },
new Question { questionText = "Gần mực thì ...?", answers = new string[] { "đen", "trắng", "xám", "nâu" }, correctAnswerIndex = 0 },
new Question { questionText = "Chọn bạn mà ...?", answers = new string[] { "chơi", "ăn", "uống", "đi" }, correctAnswerIndex = 0 },
new Question { questionText = "Một cây làm chẳng nên ...?", answers = new string[] { "công", "chuyện", "rừng", "non" }, correctAnswerIndex = 3 },
new Question { questionText = "Đói cho ... , rách cho ... ?", answers = new string[] { "sạch - thơm", "sạch - rắn", "sạch - tốt", "thơm - bền" }, correctAnswerIndex = 0 },
new Question { questionText = "Cái khó ló cái ...?", answers = new string[] { "khôn", "ngu", "liều", "bạo" }, correctAnswerIndex = 0 },
new Question { questionText = "Ăn .... học hay?", answers = new string[] { "vóc", "già", "tốt", "số" }, correctAnswerIndex = 0 },
new Question { questionText = "Lời nói chẳng mất ...?", answers = new string[] { "tiền mua", "công sức", "đôi môi", "đồng nào" }, correctAnswerIndex = 0 },
new Question { questionText = "Thương người như thể thương ...?", answers = new string[] { "mình", "nhà", "trời" ,"thân"}, correctAnswerIndex = 3 },
new Question { questionText = "Tốt gỗ hơn tốt ...?", answers = new string[] {  "lời nói", "nước sơn", "hình hài", "người sang" }, correctAnswerIndex = 1 },
new Question { questionText = "Uống nước nhớ ...?", answers = new string[] {  "giếng", "cây", "nguồn", "mưa" }, correctAnswerIndex = 2 },
new Question { questionText = "Ăn cơm .... thấm về lâu?", answers = new string[] { "mắm", "trắng", "rừng", "hàng" }, correctAnswerIndex = 0 },
new Question { questionText = "Con hơn cha là nhà có ...?", answers = new string[] {  "lộc", "phúc", "đức", "họ" }, correctAnswerIndex = 1 },
new Question { questionText = "Học thầy không tày học ...?", answers = new string[] {  "cha", "mẹ", "bạn", "trò" }, correctAnswerIndex = 2 },
new Question { questionText = "Đi một ngày đàng học một ...?", answers = new string[] {  "lời hay", "câu khôn", "sàng khôn", "chữ đẹp" }, correctAnswerIndex = 2 },
new Question { questionText = "Cha nào con ...?", answers = new string[] { "ấy", "nấy", "kia", "nọ" }, correctAnswerIndex = 1 },
new Question { questionText = "Đường dài mới biết ...?", answers = new string[] {  "người khôn", "lòng nhau", "đường gần","ngựa hay" }, correctAnswerIndex = 3 },
new Question { questionText = "Bới bèo ra ...?", answers = new string[] { "rễ", "lá", "cỏ", "bọ" }, correctAnswerIndex = 3 },
new Question { questionText = "Yêu nhau củ ấu cũng ...?", answers = new string[] { "tròn", "vuông", "dài", "ngắn" }, correctAnswerIndex = 0 }
    };

    private int currentQuestionIndex = 0;
    private int correctStreak = 0;
    private const int requiredCorrectStreak = 3;

    [SerializeField] private MinigameManager minigameManager;
    private void Start()
    {
        if (questions.Count == 0)
        {
            Debug.LogError("Danh sách câu hỏi rỗng!");
            return;
        }

        failScreen.SetActive(false);
        successScreen.SetActive(false);

        // Không cần retry
        //retryButton.onClick.AddListener(RetryGame);

        LoadQuestion();

        // Find MinigameManager if not assigned
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
            AudioManager.Instance.PlaySound("questionMinigameTheme");
        }
    }

    void LoadQuestion()
    {
        if (questions.Count == 0)
        {
            Debug.Log("Hết câu hỏi! Reset lại...");
            ResetGame();
            return;
        }

        currentQuestionIndex = Random.Range(0, questions.Count); // Lấy ngẫu nhiên 1 câu
        Question currentQuestion = questions[currentQuestionIndex];
        questionText.text = currentQuestion.questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            TMP_Text btnText = answerButtons[i].GetComponentInChildren<TMP_Text>();
            if (btnText != null)
                btnText.text = currentQuestion.answers[i];

            answerButtons[i].GetComponent<Image>().color = Color.white;
            answerButtons[i].onClick.RemoveAllListeners();
            int index = i;
            answerButtons[i].onClick.AddListener(() => CheckAnswer(index));
        }
    }

    void CheckAnswer(int selectedIndex)
    {
        Question currentQuestion = questions[currentQuestionIndex];

        if (selectedIndex == currentQuestion.correctAnswerIndex)
        {
            answerButtons[selectedIndex].GetComponent<Image>().color = Color.green;
            correctStreak++;

            if (correctStreak >= requiredCorrectStreak)
            {
                ShowSuccessScreen();
            }
            else
            {
                questions.RemoveAt(currentQuestionIndex); // Loại bỏ câu đã đúng
                LoadQuestion(); // Chuyển câu mới
            }
        }
        else
        {
            answerButtons[selectedIndex].GetComponent<Image>().color = Color.red;
            correctStreak = 0;
            questions.RemoveAt(currentQuestionIndex); // Loại bỏ câu sai
            ShowFailScreen();


        }
    }

    void ResetGame()
    {
        // Khôi phục lại toàn bộ câu hỏi ban đầu
        questions = new List<Question>
    {
       new Question { questionText = "Ăn quả nhớ ...?", answers = new string[] { "kẻ trồng cây", "người bán cây", "người hái quả", "người tưới cây" }, correctAnswerIndex = 0 },
new Question { questionText = "Có công mài sắt, có ngày nên ...?", answers = new string[] { "nhà", "kim", "đá", "đồng" }, correctAnswerIndex = 1 },
new Question { questionText = "Gần mực thì ...?", answers = new string[] { "đen", "trắng", "xám", "nâu" }, correctAnswerIndex = 0 },
new Question { questionText = "Chọn bạn mà ...?", answers = new string[] { "chơi", "ăn", "uống", "đi" }, correctAnswerIndex = 0 },
new Question { questionText = "Một cây làm chẳng nên ...?", answers = new string[] { "công", "chuyện", "rừng", "non" }, correctAnswerIndex = 3 },
new Question { questionText = "Đói cho ... , rách cho ... ?", answers = new string[] { "sạch - thơm", "sạch - rắn", "sạch - tốt", "thơm - bền" }, correctAnswerIndex = 0 },
new Question { questionText = "Cái khó ló cái ...?", answers = new string[] { "khôn", "ngu", "liều", "bạo" }, correctAnswerIndex = 0 },
new Question { questionText = "Ăn .... học hay?", answers = new string[] { "vóc", "già", "tốt", "số" }, correctAnswerIndex = 0 },
new Question { questionText = "Lời nói chẳng mất ...?", answers = new string[] { "tiền mua", "công sức", "đôi môi", "đồng nào" }, correctAnswerIndex = 0 },
new Question { questionText = "Thương người như thể thương ...?", answers = new string[] { "mình", "nhà", "trời" ,"thân"}, correctAnswerIndex = 3 },
new Question { questionText = "Tốt gỗ hơn tốt ...?", answers = new string[] {  "lời nói", "nước sơn", "hình hài", "người sang" }, correctAnswerIndex = 1 },
new Question { questionText = "Uống nước nhớ ...?", answers = new string[] {  "giếng", "cây", "nguồn", "mưa" }, correctAnswerIndex = 2 },
new Question { questionText = "Ăn cơm .... thấm về lâu?", answers = new string[] { "mắm", "trắng", "rừng", "hàng" }, correctAnswerIndex = 0 },
new Question { questionText = "Con hơn cha là nhà có ...?", answers = new string[] {  "lộc", "phúc", "đức", "họ" }, correctAnswerIndex = 1 },
new Question { questionText = "Học thầy không tày học ...?", answers = new string[] {  "cha", "mẹ", "bạn", "trò" }, correctAnswerIndex = 2 },
new Question { questionText = "Đi một ngày đàng học một ...?", answers = new string[] {  "lời hay", "câu khôn", "sàng khôn", "chữ đẹp" }, correctAnswerIndex = 2 },
new Question { questionText = "Cha nào con ...?", answers = new string[] { "ấy", "nấy", "kia", "nọ" }, correctAnswerIndex = 1 },
new Question { questionText = "Đường dài mới biết ...?", answers = new string[] {  "người khôn", "lòng nhau", "đường gần","ngựa hay" }, correctAnswerIndex = 3 },
new Question { questionText = "Bới bèo ra ...?", answers = new string[] { "rễ", "lá", "cỏ", "bọ" }, correctAnswerIndex = 3 },
new Question { questionText = "Yêu nhau củ ấu cũng ...?", answers = new string[] { "tròn", "vuông", "dài", "ngắn" }, correctAnswerIndex = 0 }
    };

        currentQuestionIndex = 0;
        correctStreak = 0;
        failScreen.SetActive(false);
        successScreen.SetActive(false);
        LoadQuestion();
    }


    void ShowFailScreen()
    {
        failScreen.SetActive(true);
        Debug.Log("Bạn đã thất bại!");
    }

    void ShowSuccessScreen()
    {
        successScreen.SetActive(true);
        Debug.Log("Bạn đã qua màn!");
    }

    void RetryGame()
    {
        currentQuestionIndex = 0;
        correctStreak = 0;
        failScreen.SetActive(false);
        LoadQuestion();
    }
}
