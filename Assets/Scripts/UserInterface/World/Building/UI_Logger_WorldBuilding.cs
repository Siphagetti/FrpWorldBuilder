using TMPro;
using UnityEngine;

namespace UserInterface.World.Building
{
    public class UI_Logger_WorldBuilding : MonoBehaviour, Log.ILoggerUI
    {
        [SerializeField] private Transform _logContainer;
        [SerializeField] private GameObject _logPrefab;

        [SerializeField] private Color trackTextColor       = Color.white;
        [SerializeField] private Color infoTextColor        = Color.blue;
        [SerializeField] private Color warningTextColor     = Color.yellow;
        [SerializeField] private Color errorTextColor       = new Color(1f, 0.5f, 0f);
        [SerializeField] private Color fatalTextColor       = Color.red;

        public void Log_Track(string text)
        {
            TMP_Text _text = GetText();
            _text.text = text;
            _text.color = trackTextColor;
        }

        public void Log_Info(string text)
        {
            TMP_Text _text = GetText();
            _text.text = text;
            _text.color = infoTextColor;
        }

        public void Log_Warning(string text)
        {
            TMP_Text _text = GetText();
            _text.text = text;
            _text.color = warningTextColor;
        }

        public void Log_Error(string text)
        {
            TMP_Text _text = GetText();
            _text.text = text;
            _text.color = errorTextColor;
        }

        public void Log_Fatal(string text)
        {
            TMP_Text _text = GetText();
            _text.text = text;
            _text.color = fatalTextColor;
        }

        private GameObject CreateLog() => Instantiate(_logPrefab, _logContainer);

        private TMP_Text GetText() => CreateLog().GetComponentInChildren<TMP_Text>();
    }
}
