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
    }
    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.F5))
            SaveManager.Instance.Save();
        
        if (Input.GetKeyDown(KeyCode.F8))
            SaveManager.Instance.Load();
    }

    public static GameManager Instance { get; private set; } = null;

    public static Coroutine NewCoroutine(IEnumerator coroutine)
    {
        return Instance._coroutineRunner.StartCoroutine(coroutine);
    }
}
