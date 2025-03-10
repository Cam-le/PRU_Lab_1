using UnityEngine;

public class Timer : MonoBehaviour
{
    Main main;

    [SerializeField] private float timeLeft = 60f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        main = GameObject.Find("Scripts").GetComponent<Main>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!main.gameOver && !main.isTutorial)
        {
            timeLeft -= Time.deltaTime;
        }
        if(timeLeft <= 0)
        {
            timeLeft = 0;
            main.gameWin = true;
        }
    }

        void OnGUI()
    {
        GUI.Label(new Rect(10, 40, 500, 100), "Thời gian còn lại: " + Mathf.Ceil(timeLeft));  // Hiển thị thời gian còn lại
        GUI.skin.label.fontSize = 20;
        GUI.skin.label.normal.textColor = Color.white;
    }
}
