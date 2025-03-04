using UnityEngine;

public class HPManager : MonoBehaviour
{
    Main main;
    public int maxHP = 3;
    private int currentHP =3;
    public GameObject heartPrefab;
    public Transform hpContainer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // main = GameObject.Find("Scripts").GetComponent<Main>();
        // currentHP = maxHP;
        // main.hitPoint = currentHP;
        UpdateHP();
    }


    void UpdateHP()
    {
        foreach(Transform child in hpContainer)
        {
            Destroy(child.gameObject);
        }

        for(int i =0; i < currentHP; i++)
        {
            Instantiate(heartPrefab, hpContainer);
        }
    }

    public void LoseHP()
    {
        if(currentHP > 0)
        {
            currentHP-- ;
            UpdateHP();
        }
    } 

}
