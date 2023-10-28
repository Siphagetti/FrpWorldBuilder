using Log;
using TMPro;
using UnityEngine;

namespace UserInterface.World.Building.Log
{

    public class UI_Logger_WorldBuilding : MonoBehaviour, ILoggerUI
    {
        [SerializeField] private Transform _logContainer;
        [SerializeField] private GameObject _logPrefab;

        [SerializeField] private Color trackTextColor       = Color.white;
        [SerializeField] private Color infoTextColor        = Color.blue;
        [SerializeField] private Color warningTextColor     = Color.yellow;
        [SerializeField] private Color errorTextColor       = new Color(1f, 0.5f, 0f);
        [SerializeField] private Color fatalTextColor       = Color.red;

        public void Log(string message, global::Log.LogType logType)
        {
            switch (logType) 
            {
                case global::Log.LogType.Track:     Log_Track   (message); break;
                case global::Log.LogType.Info:      Log_Info    (message); break;
                case global::Log.LogType.Warning:   Log_Warning (message); break;
                case global::Log.LogType.Error:     Log_Error   (message); break;
                case global::Log.LogType.Fatal:     Log_Fatal   (message); break;
            }
        }


        private void Log_Track(string text)
        {
            if (!gameObject.activeSelf) return;
            CreateTextBox(text, trackTextColor);
        }

        private void Log_Info(string text)
        {
            if (!gameObject.activeSelf) return;
            CreateTextBox(text, infoTextColor);
        }

        private void Log_Warning(string text)
        {
            if (!gameObject.activeSelf) return;
            CreateTextBox(text, warningTextColor);
        }

        private void Log_Error(string text)
        {
            if (!gameObject.activeSelf) return;
            CreateTextBox(text, errorTextColor);
        }

        private void Log_Fatal(string text)
        {
            if (!gameObject.activeSelf) return;
            CreateTextBox(text, fatalTextColor);
        }

        private void CreateTextBox(string text, Color color)
        {
            TMP_Text _text = GetText();
            _text.text = text;
            _text.color = color;

            int lineCount = _text.text.Split('\n').Length;
            float preferredHeight = lineCount * _text.fontSize; 
            var rect = _text.transform.parent.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y + preferredHeight);
        }

        private GameObject CreateLog() => Instantiate(_logPrefab, _logContainer);

        private TMP_Text GetText() => CreateLog().GetComponentInChildren<TMP_Text>();
    }
}
