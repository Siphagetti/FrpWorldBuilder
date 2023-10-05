using Save;
using Services;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        if (Instance != null) { Destroy(this); return; }
        Instance = this;
        DontDestroyOnLoad(this);

        new Log.Logger(); // Initialize Logger.
        new SaveManager(); // Initialize Save Manager.
        new ServiceManager(); // Initialize services.

        // Temporary
        SaveManager.Instance.Load("TestSave");
        Log.Logger.Instance.Log_Track("test_message");
        //
    }

    public static GameManager Instance { get; private set; } = null;
}
