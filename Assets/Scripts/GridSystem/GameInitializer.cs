using UnityEngine;
using System.Collections;

public class GameInitializer : MonoBehaviour
{
    [Header("Initialization Settings")]
    [SerializeField] private bool initializeOnStart = true;
    [SerializeField] private float initializationDelay = 0.5f;

    [Header("Subsystems")]
    [SerializeField] private TileEffectManager effectManager;
    [SerializeField] private TileEffectPrefab effectPrefabManager;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameManager gameManager;

    private void Start()
    {
        if (initializeOnStart)
        {
            StartCoroutine(InitializeGameSystems());
        }
    }

    public void InitializeNow()
    {
        StartCoroutine(InitializeGameSystems());
    }

    private IEnumerator InitializeGameSystems()
    {
        Debug.Log("Starting game system initialization...");

        // Short delay to ensure all components are ready
        yield return new WaitForSeconds(initializationDelay);

        // Find components if not assigned
        FindMissingComponents();

        // First, ensure the grid is set up
        if (gridManager != null)
        {
            Debug.Log("Initializing grid system...");
            // Add any specific grid initialization here if needed
        }

        // Then set up the effect prefabs
        if (effectPrefabManager != null)
        {
            Debug.Log("Setting up effect prefabs...");
            effectPrefabManager.SetupEffects();
            yield return new WaitForSeconds(0.2f);
        }

        // Finally distribute effects on the board
        if (effectManager != null)
        {
            Debug.Log("Distributing effects across the board...");
            effectManager.DistributeEffects();
        }

        // Signal game manager that initialization is complete
        if (gameManager != null)
        {
            Debug.Log("Notifying game manager of completed initialization...");
            // Call any game manager initialization methods if needed
        }

        Debug.Log("Game system initialization complete!");
    }

    private void FindMissingComponents()
    {
        if (effectManager == null)
            effectManager = FindObjectOfType<TileEffectManager>();

        if (effectPrefabManager == null)
            effectPrefabManager = FindObjectOfType<TileEffectPrefab>();

        if (gridManager == null)
            gridManager = FindObjectOfType<GridManager>();

        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
    }
}