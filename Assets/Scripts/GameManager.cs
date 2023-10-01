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

        // Temporary
        //ServiceManager.Instance.GetService<Prefab.IPrefabService>().SavePrefab("D:\\TestMesh.obj", "Characters");
        //ServiceManager.Instance.GetService<Prefab.IPrefabService>().LoadPrefab("TestMesh", "Characters");
    }

    public static GameManager Instance { get; private set; } = null;

}
