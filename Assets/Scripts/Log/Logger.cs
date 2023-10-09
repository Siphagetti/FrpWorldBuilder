using Language;
using Services;
using System.Linq;
using UnityEngine;

namespace Log
{
    public class Logger
    {
        private static Logger _instance;

        private ILoggerUI[] loggerUIs;
        public Logger()
        {
            if (_instance != null) return;
            _instance = this;

            loggerUIs = Object.FindObjectsOfType<MonoBehaviour>().OfType<ILoggerUI>().ToArray();
        }

        public static void Log_Track(string key, params object[] args)
        {
            string text = GetText(key, args);
            foreach (var loggerUI in _instance.loggerUIs) loggerUI.Log_Track(text);
        }

        public static void Log_Info(string key, params object[] args)
        {
            string text = GetText(key, args);
            foreach (var loggerUI in _instance.loggerUIs) loggerUI.Log_Info(text);
        }

        public static void Log_Warning(string key, params object[] args)
        {
            string text = GetText(key, args);
            foreach (var loggerUI in _instance.loggerUIs) loggerUI.Log_Warning(text);
        }

        public static void Log_Error(string key, params object[] args)
        {
            string text = GetText(key, args);
            foreach (var loggerUI in _instance.loggerUIs) loggerUI.Log_Error(text);
        }

        public static void Log_Fatal(string key, params object[] args)
        {
            string text = GetText(key, args);
            foreach (var loggerUI in _instance.loggerUIs) loggerUI.Log_Fatal(text);
        }


        // -------------------- Helpers --------------------

        // Takes a key to return a text in current language
        private static string GetText(string key, params object[] args)
        {
            var msg = ServiceManager.GetService<ILanguageService>().GetLocalizedValue(key);
            return string.Format(msg, args);
        }
    }
}


