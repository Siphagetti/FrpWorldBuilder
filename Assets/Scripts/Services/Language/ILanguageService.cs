namespace Language
{
    // Define a delegate and an event for language change notifications
    public delegate void LanguageChangeAction();

    internal interface ILanguageService : Services.IBaseService
    {
        public void Subscribe(LanguageChangeAction languageChangeAction);
        public void Unsubscribe(LanguageChangeAction languageChangeAction);

        public string GetLocalizedValue(string key);

        public void ChangeLanguage(Language language);
    }
}
