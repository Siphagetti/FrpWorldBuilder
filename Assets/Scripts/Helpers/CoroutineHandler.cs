using System.Collections;
using UnityEngine;

public class CoroutineHandler : MonoBehaviour
{
    private static CoroutineHandler _instance;

    public static void NewCoroutine(IEnumerator coroutine)
    {
        _instance.StartCoroutine(coroutine);
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
            return;
        }
        _instance = this;
    }
}

