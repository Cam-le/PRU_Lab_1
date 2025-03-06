using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Giữ lại nhạc khi chuyển scene
        }
        else
        {
            Destroy(gameObject); // Tránh nhân đôi nhạc nếu scene load lại
        }
    }
}
