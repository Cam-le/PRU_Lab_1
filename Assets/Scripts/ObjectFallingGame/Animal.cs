using UnityEngine;

public class Animal : MonoBehaviour
{
    Main main;
    Transform tr;

    [SerializeField]
    private float fallingSpeed = 0.1f;
    [SerializeField]
    private int score = -10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tr = GetComponent<Transform>();
        main = GameObject.Find("Scripts").GetComponent<Main>();
    }

    void FixedUpdate()
    {
        tr.position -= new Vector3(0f, fallingSpeed, 0f);
        if(tr.position.y < -5f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Character")
        {
            Destroy(this.gameObject);
            main.AddScore(score);
        }
    }
}
