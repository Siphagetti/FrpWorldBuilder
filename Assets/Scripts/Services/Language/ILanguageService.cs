using System.Collections;

namespace Language
{
    // Define a delegate and an event for language change notifications
    public delegate void LanguageChangeAction();

    internal interface ILanguageService : Services.IBaseService
    {
        public string GetLocalizedText(string key, params object[] args);
        public void Subscribe(LanguageChangeAction languageChangeAction);
        public void Unsubscribe(LanguageChangeAction languageChangeAction);
        public IEnumerator GetLocalizedValue(string key);
        public void ChangeLanguage(Language language);
    }
}
