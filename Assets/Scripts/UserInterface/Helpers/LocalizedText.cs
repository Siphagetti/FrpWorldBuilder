using UnityEngine;
using TMPro;
using Services;

namespace Language
{
    internal class LocalizedText : MonoBehaviour
    {
        private TMP_Text _text;

        public string key;

        private void Start()
        {
            _text = GetComponent<TMP_Text>();
            UpdateText();
            ServiceManager.Instance.GetService<ILanguageService>().Subscribe(UpdateText);
        }

        private void UpdateText()
        {
            if (_text != null) 
                _text.text = ServiceManager.Instance.GetService<ILanguageService>().GetLocalizedValue(key);
        }
    }
}
