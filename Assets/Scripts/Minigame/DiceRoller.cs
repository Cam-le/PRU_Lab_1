using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DiceRoller : MonoBehaviour
{
    [System.Serializable]
    public class DiceRolledEvent : UnityEvent<int> { }

    [Header("Sounds")]
    [SerializeField] private string diceRollSound = "diceRoll";
    [SerializeField] private string diceResultSound = "diceResult";

    public int numberOfSides = 12;
    private int lastRoll;
    public TextMeshProUGUI resultText;
    private bool isRolling = false;

    // Add event that will be triggered when dice is rolled
    public DiceRolledEvent OnDiceRolled = new DiceRolledEvent();

    private Quaternion originalRotation;

    void Start()
    {
        originalRotation = transform.rotation;
        resultText.text = "0";
    }

    public void RollDice()
    {
        // Check if player is currently moving
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null && player.IsMoving)
        {
            Debug.Log("Cannot roll dice while player is moving!");
            return;
        }

        // Prevent rolling while animation is playing
        if (isRolling)
        {
            return;
        }  
        int baseRoll = Random.Range(1, numberOfSides + 1);
        // Bonus effect
        lastRoll = ApplyDiceEffects(baseRoll);

        Debug.Log("Result Roll Dice: " + lastRoll);
        AnimateDiceRoll();
    }

    private void AnimateDiceRoll()
    {
        StartCoroutine(RollCoroutine());
    }

    private void UpdateResultText()
    {
        if (resultText != null)
        {
            resultText.text = lastRoll.ToString(); // Cập nhật nội dung TextMeshPro
        }
    }

    private IEnumerator RollCoroutine()
    {
        isRolling = true;

        resultText.text = "";
        Vector3 originalPosition = transform.position;
        float duration = 1f; // Tổng thời gian quay
        float elapsed = 0f;
        float rotationSpeed = 0.01f;

        // Play roll sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(diceRollSound);
        }

        // Quay như bánh xe
        while (elapsed < duration)
        {
            // Tính toán góc quay
            float rotationAmount = rotationSpeed * 360f * (elapsed / duration); // Quay 360 độ trong thời gian và giảm tốc độ
            transform.Rotate(0, 0, rotationAmount); // Xoay quanh trục Z

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Play result sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(diceResultSound);
        }

        // Đặt lại vị trí và góc ban đầu
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        UpdateResultText();

        // Set rolling flag to false
        isRolling = false;

        // Trigger the OnDiceRolled event with the roll result
        OnDiceRolled.Invoke(lastRoll);
    }

    public int GetLastRoll()
    {
        return lastRoll;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RollDice();
        }
    }

    private int ApplyDiceEffects(int originalRoll)
    {
        int modifiedRoll = originalRoll;

        // Check for dice modification effects
        if (PlayerState.HasBuff("DiceBoost"))
        {
            int boostAmount = PlayerState.GetStatusEffectValue("DiceBoost", 0);
            modifiedRoll += boostAmount;
            Debug.Log($"Applied dice boost: {originalRoll} → {modifiedRoll}");
        }

        return modifiedRoll;
    }
}