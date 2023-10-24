using Language;
using Services;
using System.Collections;
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

        private ILoggerUI[] loggerUIs;

        private void Awake()
        {
            if (_instance != null) return;
            _instance = this;

            loggerUIs = FindObjectsOfType<MonoBehaviour>().OfType<ILoggerUI>().ToArray();
        }

        private IEnumerator Log(string key, LogType logType, params object[] args)
        {
            yield return GameManager.NewCoroutine(GetText(key, args));
            var enumerator = GetText(key, args);

            while (enumerator.MoveNext())
            {
                var result = enumerator.Current;

                if (result is string)
                {
                    string text = result as string;

                    foreach (var loggerUI in _instance.loggerUIs)
                    {
                        loggerUI.Log(text, logType); yield return null;
                    }
                }
            }
        }

        public static void Log_Track(string key, params object[] args)
        {
            GameManager.NewCoroutine(_instance.Log(key, LogType.Track, args));
        }

        public static void Log_Info(string key, params object[] args)
        {
            GameManager.NewCoroutine(_instance.Log(key, LogType.Info, args));

        }

        public static void Log_Warning(string key, params object[] args)
        {
            GameManager.NewCoroutine(_instance.Log(key, LogType.Warning, args));
        }

        public static void Log_Error(string key, params object[] args)
        {
            GameManager.NewCoroutine(_instance.Log(key, LogType.Error, args));
        }

        public static void Log_Fatal(string key, params object[] args)
        {
            GameManager.NewCoroutine(_instance.Log(key, LogType.Fatal, args));
        }


        // -------------------- Helpers --------------------

        // Takes a key to return a text in current language
        private IEnumerator GetText(string key, params object[] args)
        {
            yield return GameManager.NewCoroutine(ServiceManager.GetService<ILanguageService>().GetLocalizedValue(key));

            // Get the result from the IEnumerator
            var enumerator = ServiceManager.GetService<ILanguageService>().GetLocalizedValue(key);
            while (enumerator.MoveNext())
            {
                var result = enumerator.Current;
                if (result is string)
                {
                    var msg = result as string;
                    yield return string.Format(msg, args);
                }
            }
        }
    }
}


