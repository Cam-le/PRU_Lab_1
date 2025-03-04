using System;
using UnityEngine;

public class Knife : MonoBehaviour
{
    Main main;
    Transform tr;

    [SerializeField]
    private float fallingSpeed = 0.15f;
    [SerializeField]
    private int hitPoint = 1;

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
            // until player use their 3 life, destroy their player
            int life = main.ExtractLifePoint(hitPoint);

            if(life == 0){
                Destroy(collision.gameObject);
                main.gameOver = true;
            }
        }
    }
}
