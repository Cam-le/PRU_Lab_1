using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    Main main;
    public TMP_Text tutorialText;
    public GameObject tutorialPanel;
    private bool isTutorialActive = true;

    private int currentStep = 0;
    private string[] tutorialSteps = new string[]
    {
        "Chào mừng đến với màn chơi hứng hoa quả",
        "Hãy dùng phím mũi tên bên trái hoặc bên phải để di chuyển",
        "Nên hứng các loại hoa quả (chuối, táo, măng cụt) và bánh kẹo để cộng điểm",
        "Hãy tránh các con vật, mũi dao trên đường. \nNếu bạn lỡ hứng nhầm mũi dao sẽ bị trừ mạng, còn hứng các con vật sẽ bị trừ điểm",
        "Chúc bạn chơi game vui vẻ!"
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ShowTutorialStep(currentStep);
        main = GameObject.Find("Scripts").GetComponent<Main>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isTutorialActive && Input.anyKeyDown)
        {
            currentStep++ ;
            ShowTutorialStep(currentStep);
        }
    }

    void ShowTutorialStep(int step)
    {
        if(step < tutorialSteps.Length)
        {
            tutorialText.text = tutorialSteps[step];
        }
        else
        {
            tutorialText.gameObject.SetActive(false);
            tutorialPanel.SetActive(false);
            isTutorialActive = false;
            main.isTutorial = false;
        }
    }

}
