using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Helper script to create a quiz panel in the scene
/// </summary>
public class QuizPanelPrefab : MonoBehaviour
{
    [Header("Required Components")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private GameObject panelPrefab;

    [Header("Quiz Settings")]
    [SerializeField] private bool createOnStart = true;
    [SerializeField] private Vector2 panelSize = new Vector2(500, 400);
    [SerializeField] private Color panelColor = new Color(0.1f, 0.1f, 0.1f, 0.9f);
    [SerializeField] private Color buttonColor = new Color(0.2f, 0.4f, 0.8f, 1f);

    private void Start()
    {
        if (createOnStart)
        {
            CreateQuizPanel();
        }
    }

    [ContextMenu("Create Quiz Panel")]
    public void CreateQuizPanel()
    {
        // Find canvas if not assigned
        if (mainCanvas == null)
        {
            mainCanvas = FindObjectOfType<Canvas>();
            if (mainCanvas == null)
            {
                Debug.LogError("No Canvas found in the scene!");
                return;
            }
        }

        // Create parent panel
        GameObject quizPanelObj = new GameObject("QuizPanel");
        quizPanelObj.transform.SetParent(mainCanvas.transform, false);

        // Add components
        RectTransform rectTransform = quizPanelObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = panelSize;

        Image panelImage = quizPanelObj.AddComponent<Image>();
        panelImage.color = panelColor;

        // Add layout group
        VerticalLayoutGroup layoutGroup = quizPanelObj.AddComponent<VerticalLayoutGroup>();
        layoutGroup.padding = new RectOffset(20, 20, 20, 20);
        layoutGroup.spacing = 15;
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.childControlHeight = false;
        layoutGroup.childControlWidth = true;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childForceExpandWidth = true;

        // Create title
        GameObject titleObj = new GameObject("QuestionText");
        titleObj.transform.SetParent(quizPanelObj.transform, false);

        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.sizeDelta = new Vector2(panelSize.x - 40, 80);

        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "Câu hỏi sẽ hiển thị ở đây?";
        titleText.fontSize = 24;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;

        // Create buttons container
        GameObject buttonsObj = new GameObject("AnswerButtons");
        buttonsObj.transform.SetParent(quizPanelObj.transform, false);

        RectTransform buttonsRect = buttonsObj.AddComponent<RectTransform>();
        buttonsRect.sizeDelta = new Vector2(panelSize.x - 40, 200);

        // Add grid layout for buttons
        GridLayoutGroup gridLayout = buttonsObj.AddComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2((panelSize.x - 60) / 2, 70);
        gridLayout.spacing = new Vector2(20, 20);
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 2;

        // Create answer buttons (4)
        Button[] buttons = new Button[4];
        for (int i = 0; i < 4; i++)
        {
            GameObject buttonObj = new GameObject($"AnswerButton{i}");
            buttonObj.transform.SetParent(buttonsObj.transform, false);

            // Add button component
            Button button = buttonObj.AddComponent<Button>();
            buttons[i] = button;

            // Add image component
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = buttonColor;
            button.targetGraphic = buttonImage;

            // Create button text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
            buttonText.text = $"Đáp án {i + 1}";
            buttonText.fontSize = 18;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.color = Color.white;
        }

        // Create result panel
        GameObject resultPanelObj = new GameObject("ResultPanel");
        resultPanelObj.transform.SetParent(quizPanelObj.transform, false);

        RectTransform resultRect = resultPanelObj.AddComponent<RectTransform>();
        resultRect.sizeDelta = new Vector2(panelSize.x - 40, 60);

        Image resultImage = resultPanelObj.AddComponent<Image>();
        resultImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        // Add result text
        GameObject resultTextObj = new GameObject("ResultText");
        resultTextObj.transform.SetParent(resultPanelObj.transform, false);

        RectTransform resultTextRect = resultTextObj.AddComponent<RectTransform>();
        resultTextRect.anchorMin = Vector2.zero;
        resultTextRect.anchorMax = Vector2.one;
        resultTextRect.sizeDelta = Vector2.zero;

        TextMeshProUGUI resultText = resultTextObj.AddComponent<TextMeshProUGUI>();
        resultText.text = "Kết quả";
        resultText.fontSize = 24;
        resultText.alignment = TextAlignmentOptions.Center;
        resultText.color = Color.white;

        // Add TileQuizManager component
        TileQuizManager quizManager = quizPanelObj.AddComponent<TileQuizManager>();

        // Set references - using the public fields now
        quizManager.quizPanel = quizPanelObj;
        quizManager.questionText = titleText;
        quizManager.resultPanel = resultPanelObj;
        quizManager.resultText = resultText;
        quizManager.answerButtons = buttons;

        // Hide panels initially
        quizPanelObj.SetActive(false);
        resultPanelObj.SetActive(false);

        Debug.Log("Quiz Panel created successfully!");
    }
}