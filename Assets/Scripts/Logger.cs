using UnityEngine;
using UnityEngine.UI;

// Responsible for logging actions in the game.
public class Logger : MonoBehaviour
{
    // Temporary UI Element
    Text _text;

    private static Logger _instance;

    private void Awake()
    {
        if (_instance != null) { Destroy(this); return; }

        _instance = this;
        DontDestroyOnLoad(this);
    }

    public static void Log(string msg)
    {
        _instance._text.text = msg;   
    }
}
