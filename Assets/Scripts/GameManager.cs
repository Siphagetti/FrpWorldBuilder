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
        if (Instance != null) { Destroy(this); return; }
        Instance = this;
        DontDestroyOnLoad(this);

        _coroutineRunner = gameObject.AddComponent<CoroutineRunner>();

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

    public static void NewCoroutine(IEnumerator coroutine)
    {
        Instance._coroutineRunner.StartCoroutine(coroutine);
    }
}
