using UnityEngine;
using UnityEngine.UI;

public class DebugTester : MonoBehaviour
{
    public Button testButton;

    void Start()
    {
        if (testButton != null)
        {
            testButton.onClick.AddListener(TestNotification);
        }
    }

    void TestNotification()
    {
        TileEffectManager effectManager = FindObjectOfType<TileEffectManager>();
        if (effectManager != null)
        {
            effectManager.ShowNotification("Test Effect", "This is a test notification!", null);
        }
    }
}
