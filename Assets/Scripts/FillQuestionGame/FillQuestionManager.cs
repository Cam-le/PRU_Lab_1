using UnityEngine;
using TMPro;
using System.Linq;

[System.Serializable]
public class Question
{
    public string questionText; // Câu hỏi
    public string correctAnswer; // Đáp án đúng
}

public class FillQuestionManager : MonoBehaviour
{
    public TMP_Text questionText; 
    public TMP_InputField answerInputField; 
    public TMP_Text resultText; 
    public Question[] questions; 

    public GameObject successScreen; 
    public GameObject failedScreen; 
    public int numberOfQuestionsToShow = 2; // Số lượng câu hỏi cần trả lời
    public int maxAllowedIncorrectAnswers = 1; // Số câu hỏi sai tối đa cho phép

    private Question[] selectedQuestions; // Mảng chứa các câu hỏi đã chọn

    private int currentQuestionIndex = 0; // Chỉ số câu hỏi hiện tại
    private int incorrectAnswerCount = 0; // Số câu hỏi trả lời sai

    void Start()
    {
        InitializeQuestions(); // Gọi hàm để khởi tạo câu hỏi
        SelectRandomQuestions();
        DisplayQuestion();
    }

    void InitializeQuestions()
    {
        questions = new Question[]
        {
            new Question { questionText = "Quê hương của Trạng Quỳnh thuộc tỉnh nào ngày nay?", correctAnswer = "Thanh Hóa" },
            new Question { questionText = "Bộ sách 'Đại Việt sử ký toàn thư' được biên soạn dưới triều đại nào?", correctAnswer = "Nhà Lê" },
            new Question { questionText = "Con sông nào được xem là cái nôi của nền văn minh Việt Nam?", correctAnswer = "Sông Hồng" },
            new Question { questionText = "'Hịch tướng sĩ' là tác phẩm nổi tiếng của ai?", correctAnswer = "Trần Hưng Đạo" },
            new Question { questionText = "Ai là tác giả của 'Truyện Kiều'?", correctAnswer = "Nguyễn Du" },
            new Question { questionText = "Hoàng thành Thăng Long được xây dựng vào năm nào?", correctAnswer = "1010" },
            new Question { questionText = "Hội Thề Lũng Nhai diễn ra dưới thời vua nào?", correctAnswer = "Lê Lợi" },
            new Question { questionText = "Trong câu ca dao 'Thức bao nhiêu, đắp chăn bấy nhiêu' muốn dạy điều gì?", correctAnswer = "Liệu sức mà làm" },
            new Question { questionText = "Vạn Lý Trường Thành của Việt Nam là cách gọi khác của công trình nào?", correctAnswer = "Trường Lũy" },
            new Question { questionText = "Cao Bá Quát nổi tiếng với bút danh nào?", correctAnswer = "Mẫn Hiên" },
            new Question { questionText = "Quốc hiệu Đại Cồ Việt được đặt dưới triều vua nào?", correctAnswer = "Đinh Tiên Hoàng" },
            new Question { questionText = "Tương truyền, Bà Trưng khi ra trận thường cưỡi gì?", correctAnswer = "Voi" },
            new Question { questionText = "Tác phẩm 'Bình Ngô Đại Cáo' được viết vào năm nào?", correctAnswer = "1428" },
            new Question { questionText = "Hai câu 'Non sông nước Nam vua Nam ở, Rành rành định phận tại sách trời' trích từ tác phẩm nào?", correctAnswer = "Nam Quốc Sơn Hà" },
            new Question { questionText = "Huyền Trân Công Chúa là con gái của vị vua nào?", correctAnswer = "Trần Nhân Tông" },
            new Question { questionText = "Nối câu thơ sau: 'Đầu lòng hai ả tố nga, Thúy Kiều là chị em là...'", correctAnswer = "Thúy Vân" },
            new Question { questionText = "Hoàn thành câu ca dao: 'Con ơi ghi nhớ lời này, Công cha như núi Thái Sơn, ...'", correctAnswer = "Nghĩa mẹ như nước trong nguồn chảy ra" },
            new Question { questionText = "Chén trà, cuộc rượu, câu thơ của người xưa gọi là gì?", correctAnswer = "Thi tửu trà" },
            new Question { questionText = "Hãy cho biết số lượng vần và điệu của một bài thơ Đường luật bát cú?", correctAnswer = "8 câu, 5 vần" }
        };
    }

    void SelectRandomQuestions()
    {
        // Trộn ngẫu nhiên câu hỏi
        selectedQuestions = questions.OrderBy(x => Random.value).Take(numberOfQuestionsToShow).ToArray();
    }

    public void DisplayQuestion()
    {
        if (currentQuestionIndex < selectedQuestions.Length)
        {
            questionText.text = selectedQuestions[currentQuestionIndex].questionText;
            answerInputField.text = ""; // Xóa ô nhập
            resultText.text = ""; // Xóa kết quả
        }
        else
        {
            // Hiển thị màn hình thành công nếu có đủ câu hỏi được trả lời
            successScreen.SetActive(true);
            questionText.gameObject.SetActive(false);
            answerInputField.gameObject.SetActive(false);
            resultText.gameObject.SetActive(false);
        }
    }

    public void CheckAnswer()
    {
        if (answerInputField.text.Equals(selectedQuestions[currentQuestionIndex].correctAnswer, System.StringComparison.OrdinalIgnoreCase))
        {
            resultText.text = "Chính xác!";
            currentQuestionIndex++;
            DisplayQuestion();
        }
        else
        {
            resultText.text = "Sai rồi!";
            incorrectAnswerCount++; // Tăng số câu hỏi sai

            // Kiểm tra nếu số câu hỏi sai vượt quá giới hạn cho phép
            if (incorrectAnswerCount > maxAllowedIncorrectAnswers)
            {
                failedScreen.SetActive(true);
                questionText.gameObject.SetActive(false);
                answerInputField.gameObject.SetActive(false);
                resultText.gameObject.SetActive(false);
            }
            else
            {
                // Vẫn hiển thị câu hỏi tiếp theo nếu chưa đạt giới hạn sai
                DisplayQuestion();
            }
        }
    }
}