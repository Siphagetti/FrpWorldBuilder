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
    // Enumeration to represent supported languages
    [Serializable]
    enum Language
    {
        EN, // English
        TR  // Turkish
    }

    // Struct to represent a localized message with a key and text
    [Serializable]
    struct LocalizedMessage
    {
        public string key;
        public string text;
    }

    // Class to represent a package of localized messages
    class LanguagePackage
    {
        public List<LocalizedMessage> localizedMessages;
    }

    // Language service responsible for managing localization
    internal class LanguageService : SavableObject, ILanguageService
    {
        private readonly string _folderPath = Path.Combine(Application.streamingAssetsPath, "Language");

        public event LanguageChangeAction OnLanguageChange;

        private LanguagePackage _languagePackage;

        // Serialized field to store the current language
        [SerializeField]
        private string language = Language.EN.ToString(); // Default language: English

        // Get the localized text for a given key, with support for parameters
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

        // Subscribe to the language change event
        public void Subscribe(LanguageChangeAction languageChangeAction) => OnLanguageChange += languageChangeAction;

        // Unsubscribe from the language change event
        public void Unsubscribe(LanguageChangeAction languageChangeAction) => OnLanguageChange -= languageChangeAction;

        // Change the current language
        public void ChangeLanguage(Language language)
        {
            this.language = language.ToString();
            LoadLocalizedStrings();
            OnLanguageChange.Invoke();
        }

        // Coroutine to get a localized value for a given key
        public IEnumerator GetLocalizedValue(string key)
        {
            yield return new WaitUntil(() => _languagePackage != null);
            LocalizedMessage textData = _languagePackage.localizedMessages.FirstOrDefault(d => d.key == key);
            if (textData.Equals(default)) yield return "Key not found";
            else yield return textData.text;
        }

        // Load localized strings for the current language
        private void LoadLocalizedStrings()
        {
            string filePath = Path.Combine(_folderPath, "Language_" + language + ".json");
            string data = File.ReadAllText(filePath);
            _languagePackage = JsonUtility.FromJson<LanguagePackage>(data);
        }

        // Load the language service with a specific key
        public LanguageService() : base(key: "LanguagePref") { }

        // Load method to handle data loading
        protected override Task Load(ref List<SaveData> data)
        {
            base.Load(ref data);
            LoadLocalizedStrings();
            return Task.CompletedTask;
        }
    }
}
