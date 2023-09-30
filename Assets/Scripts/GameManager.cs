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
        _serviceManager = new ServiceManager(); // Initialize services.
    }

    #region Services

    private ServiceManager _serviceManager;
    public static T GetService<T>() where T : IBaseService => Instance._serviceManager.GetService<T>();

    #endregion

    public static GameManager Instance { get; private set; } = null;

}
