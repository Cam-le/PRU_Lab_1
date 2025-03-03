﻿using System.Collections;
using TMPro;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    public int numberOfSides = 12;
    private int lastRoll;
    public TextMeshProUGUI resultText;

    private Quaternion originalRotation;

    void Start()
    {
        originalRotation = transform.rotation;
        resultText.text = "0";
    }

    public void RollDice()
    {
        lastRoll = Random.Range(1, numberOfSides + 1);
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

        resultText.text = "";
        Vector3 originalPosition = transform.position;
        float duration = 1f; // Tổng thời gian quay
        float elapsed = 0f;
        float rotationSpeed = 0.01f;
        // Quay như bánh xe
        while (elapsed < duration)
        {
            // Tính toán góc quay
            float rotationAmount = rotationSpeed * 360f * (elapsed / duration); // Quay 360 độ trong thời gian và giảm tốc độ
            transform.Rotate(0, 0, rotationAmount); // Xoay quanh trục Z

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Đặt lại vị trí và góc ban đầu
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        UpdateResultText();
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
}