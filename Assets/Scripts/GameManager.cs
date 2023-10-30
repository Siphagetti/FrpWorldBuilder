using Save;
using Services;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private CoroutineRunner _coroutineRunner;

    private void Awake()
    {
        // Ensure only one instance of GameManager exists
        if (Instance != null) { Destroy(this); return; }
        Instance = this;
        DontDestroyOnLoad(this);

        // Create a CoroutineRunner component for managing coroutines
        _coroutineRunner = gameObject.AddComponent<CoroutineRunner>();

        // Initialize the Save Manager for handling game data saving and loading.
        new SaveManager();

        // Initialize the Service Manager for managing game services.
        new ServiceManager();
    }

    private void Update()
    {
        // Check if the player presses Ctrl + S or F5 to trigger a manual save
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.F5))
            SaveManager.Instance.Save();

        // Check if the player presses F8 to trigger a manual load
        if (Input.GetKeyDown(KeyCode.F8))
            SaveManager.Instance.Load();
    }

    public static GameManager Instance { get; private set; } = null;

    public static Coroutine NewCoroutine(IEnumerator coroutine)
    {
        // Start a new coroutine using the CoroutineRunner
        return Instance._coroutineRunner.StartCoroutine(coroutine);
    }
}
