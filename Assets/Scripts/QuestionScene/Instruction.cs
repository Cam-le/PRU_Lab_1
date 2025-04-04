﻿using UnityEngine;

public class InstructionManager : MonoBehaviour
{
    [SerializeField] private GameObject instructionCanvas; // Kéo InstructionCanvas vào đây
    [SerializeField] private GameObject questionGame; // Kéo QuestionGame vào đây

    private void Start()
    {
        // Chỉ hiển thị màn hướng dẫn ban đầu
        instructionCanvas.SetActive(true);
        if (questionGame != null) 
        {
            questionGame.SetActive(false);
        }
        
    }

    public void StartGame()
    {
        // Ẩn màn hướng dẫn và hiện game
        instructionCanvas.SetActive(false);
        if (questionGame != null)
        {
            questionGame.SetActive(true);
        }
    }

    public void HideInstruction()
    {
        instructionCanvas.SetActive(false);
    }
}
