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

        new SaveManager(); // Initialize Save Manager singleton.
        new ServiceManager(); // Initialize services.
    }

    public static GameManager Instance { get; private set; } = null;

}
