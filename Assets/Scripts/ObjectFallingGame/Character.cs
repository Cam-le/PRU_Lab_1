using UnityEngine;

public class Character : MonoBehaviour
{
    Transform tr;
    Main main;

    [SerializeField] private float playerSpeed = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        main = GameObject.Find("Scripts").GetComponent<Main>();
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (main.gameOver || main.gameWin)
        {
            playerSpeed = 0f;
        }
        if (Input.GetKey("right") == true)
        {
            if (tr.position.x < 8f)
            {
                tr.position += new Vector3(playerSpeed, 0f, 0f);
            }
        }

        if (Input.GetKey("left") == true)
        {
            if (tr.position.x > -8f)
            {
                tr.position += new Vector3(-playerSpeed, 0f, 0f);
            }
        }

    }
}
