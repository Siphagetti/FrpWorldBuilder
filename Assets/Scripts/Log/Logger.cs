using Language;
using Services;
using System.Linq;
using UnityEngine;

namespace Log
{
    public class Logger
    {
        public static Logger Instance { get; private set; }

        private ILoggerUI[] loggerUIs;
        public Logger()
        {
            if (Instance != null) return;
            Instance = this;

            loggerUIs = Object.FindObjectsOfType<MonoBehaviour>().OfType<ILoggerUI>().ToArray();
        }

        public void Log_Track(string key)
        {
            string text = GetText(key);
            foreach (var loggerUI in loggerUIs) loggerUI.Log_Track(text);
        }

        public void Log_Info(string key)
        {
            string text = GetText(key);
            foreach (var loggerUI in loggerUIs) loggerUI.Log_Info(text);
        }

        public void Log_Warning(string key)
        {
            string text = GetText(key);
            foreach (var loggerUI in loggerUIs) loggerUI.Log_Warning(text);
        }

        public void Log_Error(string key)
        {
            string text = GetText(key);
            foreach (var loggerUI in loggerUIs) loggerUI.Log_Error(text);
        }

        public void Log_Fatal(string key)
        {
            string text = GetText(key);
            foreach (var loggerUI in loggerUIs) loggerUI.Log_Fatal(text);
        }


        // -------------------- Helpers --------------------

        // Takes a key to return a text in current language
        private string GetText(string key) => ServiceManager.Instance.GetService<ILanguageService>().GetLocalizedValue(key);
    }
}


