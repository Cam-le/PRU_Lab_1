using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI keysText;

    private int _keys = 0;
    private string mainSceneName = "SampleScene";
    private bool isInitializing = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == mainSceneName)
        {
            Debug.Log("Main scene loaded - initializing key UI");
            Invoke("InitializeUI", 0.2f);
        }
    }

    void InitializeUI()
    {
        // Set flag to prevent recursive calls
        if (isInitializing) return;
        isInitializing = true;

        // Find KeyNumberText directly under KeyImage in the hierarchy
        GameObject keyImage = GameObject.Find("KeyImage");
        if (keyImage != null)
        {
            Transform keyNumberText = keyImage.transform.Find("KeyNumberText");
            if (keyNumberText != null)
            {
                keysText = keyNumberText.GetComponent<TextMeshProUGUI>();
                Debug.Log("Found KeyNumberText under KeyImage");
            }
        }

        // Alternative direct approach if needed
        if (keysText == null)
        {
            GameObject keyNumberText = GameObject.Find("KeyNumberText");
            if (keyNumberText != null)
            {
                keysText = keyNumberText.GetComponent<TextMeshProUGUI>();
                Debug.Log("Found KeyNumberText by direct name");
            }
        }

        // Only update UI if we found the text component
        if (keysText != null)
        {
            keysText.text = _keys.ToString();
            Debug.Log($"Successfully initialized key UI: {_keys} keys");
        }
        else
        {
            Debug.LogError("Failed to find KeyNumberText component");
        }

        // Reset initialization flag
        isInitializing = false;
    }

    public void AddKey()
    {
        _keys++;
        UpdateUI();
        Debug.Log($"Added key. Total: {_keys} keys");
    }

    public int GetKeyCount()
    {
        return _keys;
    }

    void UpdateUI()
    {
        // Avoid updating if we're currently in the initialization process
        if (isInitializing) return;

        if (keysText == null)
        {
            Debug.LogWarning("KeyNumberText reference is missing - attempting to find it");
            InitializeUI();
            return;
        }

        keysText.text = _keys.ToString();
    }

    public void SaveToPlayerState()
    {
        //PlayerState.KeyCount = _keys;
        Debug.Log($"Saved {_keys} keys to PlayerState");
    }

    public void LoadFromPlayerState()
    {
        //_keys = PlayerState.KeyCount;
        Debug.Log($"Loaded {_keys} keys from PlayerState");
        UpdateUI();
    }

    public void RefreshUI()
    {
        InitializeUI();
    }
}