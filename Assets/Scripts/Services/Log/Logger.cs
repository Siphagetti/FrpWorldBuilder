using Language;
using Services;
using System.Linq;
using UnityEngine;

namespace Log
{
    // Enumeration to represent different log types
    public enum LogType
    {
        Track,   // Tracking information
        Info,    // General information
        Warning, // Warning messages
        Error,   // Error messages
        Fatal    // Fatal error messages
    }

    // Logger class responsible for handling logging messages
    public class Logger : MonoBehaviour
    {
        private static Logger _instance;
        private ILoggerUI[] _loggerUIs;

        // Awake method to initialize the Logger instance and find ILoggerUI components
        private void Awake()
        {
            if (_instance != null) return;
            _instance = this;

            // Find all components that implement the ILoggerUI interface
            _loggerUIs = FindObjectsOfType<MonoBehaviour>().OfType<ILoggerUI>().ToArray();
        }

        // Log method for writing log messages with localization support
        private void Log(string key, LogType logType, params object[] args)
        {
            // Get the localized text for the log message
            var text = ServiceManager.GetService<ILanguageService>().GetLocalizedText(key, args);

            // Notify all registered ILoggerUI components
            foreach (var loggerUI in _instance._loggerUIs)
                loggerUI.Log(text, logType);
        }

        // Static methods to log messages of different types
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
