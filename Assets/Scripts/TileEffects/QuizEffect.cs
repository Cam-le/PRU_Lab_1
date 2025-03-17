using UnityEngine;
using System.Collections;

/// <summary>
/// Effect that triggers a quiz and applies points and movement effects based on the result
/// </summary>
public class QuizEffect : TileEffect
{
    [Header("Quiz Effect Settings")]
    [SerializeField] private int pointsForCorrect = 500;
    [SerializeField] private int pointsForIncorrect = -200;

    [SerializeField] private int stepsForCorrect = 2;
    [SerializeField] private int stepsForIncorrect = -1;

    [SerializeField] private string correctAnswerMessage = "Trả lời đúng! Bạn nhận được phần thưởng.";
    [SerializeField] private string wrongAnswerMessage = "Trả lời sai! Bạn bị phạt.";

    [Header("Dialogue Settings")]
    [SerializeField] private bool useDialogueBubble = true;
    [SerializeField] private float quizDelay = 1.5f;

    private bool quizCompleted = false;
    private bool quizResult = false;
    private bool isShowingQuiz = false;
    private DialogueManager dialogueManager;
    private GameManager gameManager;

    protected override void Awake()
    {
        base.Awake();
        // Find dialogue manager
        dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager == null && useDialogueBubble)
        {
            Debug.LogWarning("DialogueManager not found, dialogue bubble won't be shown.");
        }

        // Find game manager
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogWarning("GameManager not found, score changes won't be applied.");
        }
    }

    public override bool ApplyEffect(PlayerController player)
    {
        // Reset quiz state
        quizCompleted = false;
        quizResult = false;

        // Show dialogue bubble first if available
        if (useDialogueBubble && dialogueManager != null)
        {
            // Show neutral message in dialogue bubble
            dialogueManager.ShowMessage(GetRandomNeutralMessage());

            // Wait a moment before showing the quiz
            StartCoroutine(ShowQuizAfterDelay(player));
            return true;
        }
        // If no dialogue manager or not using dialogue, continue with notification
        else if (quizManager != null)
        {
            // Show notification that a quiz is starting
            TileEffectManager effectManager = FindObjectOfType<TileEffectManager>();
            if (effectManager != null && ShowNotification)
            {
                effectManager.ShowNotification("Câu Đố", "Trả lời câu hỏi!", EffectIcon, EffectColor);
            }

            // Use direct method call
            isShowingQuiz = true;
            ShowQuizDirectly(player);

            return true;
        }
        else
        {
            // If no quiz manager, call the base implementation
            return base.ApplyEffect(player);
        }
    }

    private IEnumerator ShowQuizAfterDelay(PlayerController player)
    {
        yield return new WaitForSeconds(quizDelay);

        if (player != null && quizManager != null)
        {
            isShowingQuiz = true;
            ShowQuizDirectly(player);
        }
    }

    private void ShowQuizDirectly(PlayerController player)
    {
        if (!isShowingQuiz || quizManager == null) return;

        // Show the quiz and handle the result
        quizManager.ShowQuiz((isCorrect, effectType, value) =>
        {
            quizCompleted = true;
            quizResult = isCorrect;
            isShowingQuiz = false;

            // Only proceed if we still have a valid player reference
            if (player == null) return;

            // Apply effects based on result
            if (isCorrect)
            {
                // Award fixed points
                if (gameManager != null)
                {
                    gameManager.AddScore(pointsForCorrect);
                    Debug.Log($"Awarded {pointsForCorrect} points for correct answer");
                }

                // Show success message in dialogue bubble if available
                if (useDialogueBubble && dialogueManager != null)
                {
                    dialogueManager.ShowMessage(GetRandomPositiveMessage());
                }
                else
                {
                    // Show notification
                    TileEffectManager effectManager = FindObjectOfType<TileEffectManager>();
                    if (effectManager != null && ShowNotification)
                    {
                        effectManager.ShowNotification("Chính Xác!",
                            $"{correctAnswerMessage}\n+{pointsForCorrect} điểm, Tiến {stepsForCorrect} bước",
                            EffectIcon, Color.green);
                    }
                }

                // Move player forward fixed steps
                // Must be called after showing dialogue/notification
                StartCoroutine(MovePlayerAfterDelay(player, stepsForCorrect));
            }
            else
            {
                // Deduct fixed points
                if (gameManager != null)
                {
                    gameManager.AddScore(pointsForIncorrect); // This will be negative
                    Debug.Log($"Deducted {-pointsForIncorrect} points for incorrect answer");
                }

                // Show failure message in dialogue bubble if available
                if (useDialogueBubble && dialogueManager != null)
                {
                    dialogueManager.ShowMessage(GetRandomNegativeMessage());
                }
                else
                {
                    // Show notification
                    TileEffectManager effectManager = FindObjectOfType<TileEffectManager>();
                    if (effectManager != null && ShowNotification)
                    {
                        effectManager.ShowNotification("Sai Rồi!",
                            $"{wrongAnswerMessage}\n{pointsForIncorrect} điểm, Lùi {-stepsForIncorrect} bước",
                            EffectIcon, Color.red);
                    }
                }

                // Move player backward fixed steps
                // Must be called after showing dialogue/notification
                StartCoroutine(MovePlayerAfterDelay(player, stepsForIncorrect));
            }

            // Trigger the effect event
            if (player != null)
            {
                base.ApplyEffect(player);
            }
        });
    }

    // Coroutine to move player after a short delay
    private IEnumerator MovePlayerAfterDelay(PlayerController player, int steps)
    {
        // Short delay to allow dialogue to show
        yield return new WaitForSeconds(1.5f);

        if (player != null && !player.IsMoving)
        {
            Debug.Log($"Moving player {steps} steps");

            // Directly call MovePlayer with steps
            if (steps != 0)
            {
                player.MovePlayer(steps);
            }
        }
    }

    // Get a random neutral message
    private string GetRandomNeutralMessage()
    {
        if (dialogueManager != null)
        {
            // Looking at your DialogueManager.cs, it uses lists of predefined messages
            // and has a method to get random messages from these lists
            return GetRandomMessageFromDialogueManager("neutral");
        }

        // Default message if DialogueManager is not available
        return "Hmm, thách thức trí tuệ này thú vị đấy!";
    }

    // Get a random positive message
    private string GetRandomPositiveMessage()
    {
        if (dialogueManager != null)
        {
            return GetRandomMessageFromDialogueManager("positive");
        }

        return "Tuyệt vời! Bạn thật thông minh!";
    }

    // Get a random negative message
    private string GetRandomNegativeMessage()
    {
        if (dialogueManager != null)
        {
            return GetRandomMessageFromDialogueManager("negative");
        }

        return "Tiếc quá! Lần sau cố gắng hơn nhé!";
    }

    // Helper method to get random messages from DialogueManager based on type
    private string GetRandomMessageFromDialogueManager(string messageType)
    {
        // Based on your DialogueManager implementation, we need to access its message lists
        // and call the appropriate method to get a random message

        // Your DialogueManager has a method to get random messages from its lists
        switch (messageType.ToLower())
        {
            case "positive":
                // This accesses the neutral messages list in DialogueManager
                // We're using reflection since the lists might be private
                var positiveMessagesList = GetListFieldValue(dialogueManager, "positiveMessages");
                if (positiveMessagesList != null && positiveMessagesList.Count > 0)
                {
                    int randomIndex = Random.Range(0, positiveMessagesList.Count);
                    return (string)positiveMessagesList[randomIndex];
                }
                break;

            case "negative":
                var negativeMessagesList = GetListFieldValue(dialogueManager, "negativeMessages");
                if (negativeMessagesList != null && negativeMessagesList.Count > 0)
                {
                    int randomIndex = Random.Range(0, negativeMessagesList.Count);
                    return (string)negativeMessagesList[randomIndex];
                }
                break;

            case "neutral":
                var neutralMessagesList = GetListFieldValue(dialogueManager, "neutralMessages");
                if (neutralMessagesList != null && neutralMessagesList.Count > 0)
                {
                    int randomIndex = Random.Range(0, neutralMessagesList.Count);
                    return (string)neutralMessagesList[randomIndex];
                }
                break;
        }

        // Default fallback message
        return "Đây là một câu hỏi thú vị!";
    }

    // Helper method to get private fields using reflection
    private System.Collections.IList GetListFieldValue(DialogueManager manager, string fieldName)
    {
        if (manager == null) return null;

        var field = manager.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Public);

        if (field != null)
        {
            return field.GetValue(manager) as System.Collections.IList;
        }

        return null;
    }

    public override string GetEffectSummary()
    {
        if (!quizCompleted)
        {
            return "Trả lời câu đố để nhận điểm và di chuyển";
        }
        else if (quizResult)
        {
            return $"+{pointsForCorrect} điểm, Tiến {stepsForCorrect} bước";
        }
        else
        {
            return $"{pointsForIncorrect} điểm, Lùi {-stepsForIncorrect} bước";
        }
    }
}