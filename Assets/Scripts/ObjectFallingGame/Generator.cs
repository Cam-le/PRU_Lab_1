using UnityEngine;

public class Generator : MonoBehaviour
{
    Main main;
    float timer = 1;

    [SerializeField] private GameObject[] gameObject;

    void Start()
    {
        main = GameObject.Find("Scripts").GetComponent<Main>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else if (timer <= 0 && !main.isTutorial && !main.gameOver && !main.gameWin)
        {
            // Tạo ngẫu nhiên tỷ lệ phần trăm
            int chance = Random.Range(1, 101);
            float pos_x = Random.Range(-8.0f, 8.0f); // tạo vị trí X ngẫu nhiên

            // mảng đối tượng rơi theo các nhóm
            GameObject[] fallingObjects;

            if (chance <= 25)
            {
                fallingObjects = new GameObject[] { gameObject[0], gameObject[1] }; 
            }
            else if (chance <= 50)
            {
                fallingObjects = new GameObject[] { gameObject[3], gameObject[4] }; 
            }
            else if (chance <= 75)
            {
                fallingObjects = new GameObject[] { gameObject[5], gameObject[6] }; 
            }
            else
            {
                fallingObjects = new GameObject[] { gameObject[7], gameObject[8] };
            }

            // chọn ngẫu nhiên một đối tượng trong mảng fallingObjects và Instantiate nó
            int objectFallingChance = Random.Range(0, fallingObjects.Length);
            Instantiate(fallingObjects[objectFallingChance], new Vector3(pos_x, 6.0f, 0.1f), Quaternion.identity);

            // tthiết lập lại thời gian chờ để vật rơi
            timer = 0.5f;
        }
    }
}

