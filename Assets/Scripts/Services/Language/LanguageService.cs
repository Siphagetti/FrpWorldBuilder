using Save;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Language
{
    [Serializable]
    enum Language
    {
        EN,
        TR
    }

    [Serializable]
    struct LocalizedMessage
    {
        public string key;
        public string text;
    }

    class LanguagePackage
    {
        public List<LocalizedMessage> localizedMessages;
    }

    internal class LanguageService : SavableObject, ILanguageService
    {
        private readonly string _folderPath = Path.Combine(Application.streamingAssetsPath, "Language");

        public event LanguageChangeAction OnLanguageChange;

        private LanguagePackage _languagePackage;

        [SerializeField]
        private string language = Language.EN.ToString();

        public string GetLocalizedText(string key, params object[] args)
        {
            GameManager.NewCoroutine(GetLocalizedValue(key));

            // Get the result from the IEnumerator
            var enumerator = GetLocalizedValue(key);
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is string)
                {
                    var msg = enumerator.Current as string;
                    return string.Format(msg, args);
                }
            }

            return string.Empty; // You can choose an appropriate default value if needed.
        }

        public void Subscribe(LanguageChangeAction languageChangeAction) => OnLanguageChange += languageChangeAction;
        public void Unsubscribe(LanguageChangeAction languageChangeAction) => OnLanguageChange -= languageChangeAction;

        public void ChangeLanguage(Language language)
        {
            this.language = language.ToString();
            LoadLocalizedStrings();
            OnLanguageChange.Invoke();
        }
        public IEnumerator GetLocalizedValue(string key)
        {
            yield return new WaitUntil(() => _languagePackage != null);
            LocalizedMessage textData = _languagePackage.localizedMessages.FirstOrDefault(d => d.key == key);
            if (textData.Equals(default)) yield return "Key not found";
            else yield return textData.text;
        }

        private void LoadLocalizedStrings()
        {
            string filePath = Path.Combine(_folderPath, "Language_" + language + ".json");
            string data = File.ReadAllText(filePath);
            _languagePackage = JsonUtility.FromJson<LanguagePackage>(data);
        }

        protected override Task Load(ref List<SaveData> data)
        {
            base.Load(ref data);
            LoadLocalizedStrings();
            return Task.CompletedTask;
        }


        public LanguageService() : base(key: "LanguagePref") { }
    }
}
