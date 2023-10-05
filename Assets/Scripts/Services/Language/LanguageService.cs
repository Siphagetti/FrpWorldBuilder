using Save;
using System;
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

    [Serializable]
    struct LanguagePackage
    {
        public List<LocalizedMessage> localizedMessages;
    }


    internal class LanguageService : Save.SavableObject, ILanguageService
    {
        private readonly string _folderPath = Path.Combine(Application.streamingAssetsPath, "Language");

        public event LanguageChangeAction OnLanguageChange;

        private LanguagePackage _languagePackage;

        public string language = Language.EN.ToString();

        private void LoadLocalizedStrings()
        {
            string filePath = Path.Combine(_folderPath, "Language_" + language + ".json");
            string data = File.ReadAllText(filePath);
            _languagePackage = JsonUtility.FromJson<LanguagePackage>(data);
        }

        public string GetLocalizedValue(string key)
        {
            LocalizedMessage textData = _languagePackage.localizedMessages.FirstOrDefault(d => d.key == key);
            if (textData.Equals(default)) return "Key not found";
            return textData.text;
        }

        public void Subscribe(LanguageChangeAction languageChangeAction) => OnLanguageChange += languageChangeAction;

        public void ChangeLanguage(Language language)
        {
            this.language = language.ToString();
            LoadLocalizedStrings();
            OnLanguageChange.Invoke();
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
