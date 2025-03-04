using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    HPManager hPManager;
    [HideInInspector] public int score;

    [HideInInspector] public bool gameOver;

    [HideInInspector] public bool gameWin;
    
    [HideInInspector] public int hitPoint;

    [SerializeField] private Transform overTitle;

    [SerializeField] private TMP_Text winTitle;

    [HideInInspector] public bool isTutorial;



    void Start()
    {
        hPManager = GameObject.Find("Scripts").GetComponent<HPManager>();
        score = 0;
        gameOver = false;
        gameWin = false;
        isTutorial = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameOver)
        {
            overTitle.localPosition = new Vector3(0f,0f,0f);
        }
        if(gameWin)
        {
            winTitle.text = $"Chúc mừng, bạn đã qua ải!\nĐiểm của bạn: {score}";
            winTitle.gameObject.SetActive(true);
        }
    }

    public void AddScore(int additionCcore)
    {
        score += additionCcore;
        if(score <= 0)
        {
            score = 0;
        }
    }

    public int ExtractLifePoint(int point)
    {
        hitPoint -= point;
        hPManager.LoseHP();
        return hitPoint;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 70, 160, 30), "Điểm: " + score);  // Hiển thị thời gian còn lại
    }
}
