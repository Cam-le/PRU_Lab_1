using UnityEngine;
using UnityEngine.UI;
using TMPro; // Sử dụng TextMeshPro

public class QuestionManager : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string questionText; // Nội dung câu hỏi
        public string[] answers; // Danh sách các đáp án (4 đáp án)
        public int correctAnswerIndex; // Vị trí của đáp án đúng (0-3)
    }

    [Header("UI Elements")]
    public TMP_Text questionText; // TextMeshPro cho câu hỏi
    public Button[] answerButtons; // 4 nút đáp án
    public GameObject failScreen; // Màn hình thất bại
    public GameObject successScreen; // Màn hình thành công
    public Button retryButton; // Nút chơi lại
    public Button nextQuestionButton; // Nút câu hỏi tiếp theo

    [Header("Question Data")]
    public Question currentQuestion; // Câu hỏi hiện tại

    private int attempts = 0; // Số lần thử của người chơi
    private const int maxAttempts = 2; // Giới hạn số lần thử

    private void Start()
    {
        ValidateQuestionData();
        DisplayQuestion();
        AssignButtonListeners();
        failScreen.SetActive(false); // Ẩn màn hình thất bại ban đầu
        successScreen.SetActive(false); // Ẩn màn hình thành công ban đầu

        if (retryButton != null)
            retryButton.onClick.AddListener(RetryGame);

        if (nextQuestionButton != null)
            nextQuestionButton.onClick.AddListener(NextQuestion);
    }

    // Kiểm tra dữ liệu câu hỏi
    void ValidateQuestionData()
    {
        if (currentQuestion == null)
        {
            Debug.LogError("No question data found!");
            return;
        }

        if (currentQuestion.answers == null || currentQuestion.answers.Length != 4)
        {
            Debug.LogError("Dữ liệu câu hỏi không có đủ 4 đáp án!");
            return;
        }

        if (currentQuestion.correctAnswerIndex < 0 || currentQuestion.correctAnswerIndex >= 4)
        {
            Debug.LogError("Chỉ số đáp án đúng không hợp lệ!");
        }
    }

    // Hiển thị câu hỏi và đáp án lên màn hình
    void DisplayQuestion()
    {
        questionText.text = currentQuestion.questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            TMP_Text btnText = answerButtons[i].GetComponentInChildren<TMP_Text>();
            if (btnText != null)
            {
                btnText.text = currentQuestion.answers[i];
            }
            else
            {
                Debug.LogError($"Button {i} không có TMP_Text con.");
            }

            // Reset màu về trắng
            answerButtons[i].GetComponent<Image>().color = Color.white;
        }
    }

    // Gán sự kiện click cho các nút
    void AssignButtonListeners()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i; // Giữ index cố định cho lambda
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => CheckAnswer(index));
        }
    }

    // Kiểm tra đáp án khi bấm nút
    void CheckAnswer(int selectedIndex)
    {
        // Reset màu sắc tất cả các nút trước khi check
        foreach (Button button in answerButtons)
        {
            button.GetComponent<Image>().color = Color.white;
        }

        if (selectedIndex == currentQuestion.correctAnswerIndex)
        {
            answerButtons[selectedIndex].GetComponent<Image>().color = Color.green; // Đáp án đúng: Xanh
            ShowSuccessScreen();
        }
        else
        {
            answerButtons[selectedIndex].GetComponent<Image>().color = Color.red; // Đáp án sai: Đỏ
            attempts++;
            Debug.Log($"Wrong answer! Attempts: {attempts}/{maxAttempts}");

            if (attempts >= maxAttempts)
            {
                ShowFailScreen();
            }
        }
    }

    // Hiển thị màn hình thất bại
    void ShowFailScreen()
    {
        failScreen.SetActive(true);
        Debug.Log("You failed!");
    }

    // Hiển thị màn hình thành công
    void ShowSuccessScreen()
    {
        successScreen.SetActive(true);
        Debug.Log("Congratulations! You got it right!");
    }

    // Chơi lại
    void RetryGame()
    {
        attempts = 0;
        failScreen.SetActive(false);
        DisplayQuestion();
    }

    // Chuyển sang câu hỏi tiếp theo
    void NextQuestion()
    {
        successScreen.SetActive(false);
        Debug.Log("Load next question...");
        // Thêm logic load câu hỏi mới ở đây
    }
}
