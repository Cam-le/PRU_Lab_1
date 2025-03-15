using UnityEngine;

/// <summary>
/// Effect that triggers a quiz and applies effects based on the result
/// </summary>
public class QuizEffect : TileEffect
{
    [Header("Quiz Effect Settings")]
    [SerializeField] private EffectType positiveEffectType = EffectType.Points;
    [SerializeField] private int positiveEffectValue = 300;
    [SerializeField] private EffectType negativeEffectType = EffectType.Points;
    [SerializeField] private int negativeEffectValue = -150;

    [SerializeField] private string correctAnswerMessage = "Trả lời đúng! Bạn nhận được phần thưởng.";
    [SerializeField] private string wrongAnswerMessage = "Trả lời sai! Bạn bị phạt.";

    private bool quizCompleted = false;
    private bool quizResult = false;

    public override bool ApplyEffect(PlayerController player)
    {
        // Reset quiz state
        quizCompleted = false;
        quizResult = false;

        // If we have a quiz manager, show the quiz
        if (quizManager != null)
        {
            // Show notification that a quiz is starting
            TileEffectManager effectManager = FindObjectOfType<TileEffectManager>();
            if (effectManager != null && ShowNotification)
            {
                effectManager.ShowNotification("Câu Đố", "Trả lời câu hỏi để nhận phần thưởng hoặc hình phạt!", EffectIcon, EffectColor);
            }

            // Short delay before showing quiz
            Invoke("ShowQuizWithDelay", 1.0f);

            return true;
        }
        else
        {
            // If no quiz manager, call the base implementation
            return base.ApplyEffect(player);
        }
    }

    private void ShowQuizWithDelay()
    {
        if (quizManager != null)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player == null) return;

            // Show the quiz and handle the result
            quizManager.ShowQuiz((isCorrect, effectType, value) =>
            {
                quizCompleted = true;
                quizResult = isCorrect;

                // Apply appropriate effect based on result
                if (isCorrect)
                {
                    ApplyEffectBasedOnQuizResult(player, true, positiveEffectType, positiveEffectValue);

                    // Show success notification
                    TileEffectManager effectManager = FindObjectOfType<TileEffectManager>();
                    if (effectManager != null && ShowNotification)
                    {
                        effectManager.ShowNotification("Chính Xác!", correctAnswerMessage, EffectIcon, Color.green);
                    }
                }
                else
                {
                    ApplyEffectBasedOnQuizResult(player, false, negativeEffectType, negativeEffectValue);

                    // Show failure notification
                    TileEffectManager effectManager = FindObjectOfType<TileEffectManager>();
                    if (effectManager != null && ShowNotification)
                    {
                        effectManager.ShowNotification("Sai Rồi!", wrongAnswerMessage, EffectIcon, Color.red);
                    }
                }

                // Trigger the effect event - using base.ApplyEffect() instead of directly accessing the event
                // This fixes the CS0070 error
                base.ApplyEffect(player);
            });
        }
    }

    public override string GetEffectSummary()
    {
        if (!quizCompleted)
        {
            return "Trả lời câu đố để xác định phần thưởng hoặc hình phạt";
        }
        else if (quizResult)
        {
            switch (positiveEffectType)
            {
                case EffectType.Points:
                    return $"+{positiveEffectValue} điểm";
                case EffectType.Moves:
                    return $"+{positiveEffectValue} lượt";
                case EffectType.Turns:
                    return $"+{positiveEffectValue} lượt";
                case EffectType.MoveForward:
                    return $"Tiến {positiveEffectValue} ô";
                default:
                    return "Phần thưởng cho câu trả lời đúng";
            }
        }
        else
        {
            switch (negativeEffectType)
            {
                case EffectType.Points:
                    return $"{negativeEffectValue} điểm";
                case EffectType.Moves:
                    return $"{negativeEffectValue} lượt";
                case EffectType.Turns:
                    return $"{negativeEffectValue} lượt";
                case EffectType.MoveBackward:
                    return $"Lùi {-negativeEffectValue} ô";
                default:
                    return "Hình phạt cho câu trả lời sai";
            }
        }
    }
}