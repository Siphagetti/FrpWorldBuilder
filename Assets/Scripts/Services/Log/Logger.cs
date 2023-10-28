using Language;
using Services;
using System.Linq;
using UnityEngine;

namespace Log
{
    public enum LogType
    {
        Track,
        Info,
        Warning,
        Error,
        Fatal
    }

    public class Logger : MonoBehaviour
    {
        private static Logger _instance;

        private ILoggerUI[] _loggerUIs;

        private void Awake()
        {
            if (_instance != null) return;
            _instance = this;

            _loggerUIs = FindObjectsOfType<MonoBehaviour>().OfType<ILoggerUI>().ToArray();
        }

        private void Log(string key, LogType logType, params object[] args)
        {
            var text = ServiceManager.GetService<ILanguageService>().GetLocalizedText(key, args);
            foreach (var loggerUI in _instance._loggerUIs) loggerUI.Log(text, logType);
        }

        public static void Log_Track(string key, params object[] args)
        {
           _instance.Log(key, LogType.Track, args);
        }

        public static void Log_Info(string key, params object[] args)
        {
            _instance.Log(key, LogType.Info, args);

        }

        public static void Log_Warning(string key, params object[] args)
        {
            _instance.Log(key, LogType.Warning, args);
        }

        public static void Log_Error(string key, params object[] args)
        {
            _instance.Log(key, LogType.Error, args);
        }

        public static void Log_Fatal(string key, params object[] args)
        {
            _instance.Log(key, LogType.Fatal, args);
        }
    }
}


