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

        gameObject.AddComponent<CoroutineHandler>();


        new SaveManager(); // Initialize Save Manager.
        new ServiceManager(); // Initialize services.


        // Temporary
        SaveManager.Instance.Load();

    }
    private void Start()
    {
        // Temporary
        SaveManager.Instance.Save();
    }

    public static GameManager Instance { get; private set; } = null;
}
