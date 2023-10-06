using UnityEngine;
using TMPro;
using Services;

namespace Language
{
    internal class LocalizedText : MonoBehaviour
    {
        private TMP_Text _text;

        [SerializeField] private string _key;

        private void Start()
        {
            _text = GetComponent<TMP_Text>();
            UpdateText();
            ServiceManager.GetService<ILanguageService>().Subscribe(UpdateText);
        }

        private void UpdateText()
        {
            if (_text != null) 
                _text.text = ServiceManager.GetService<ILanguageService>().GetLocalizedValue(_key);
        }

        private void OnDestroy()
        {
            ServiceManager.GetService<ILanguageService>().Unsubscribe(UpdateText);
        }
    }
}
