using UnityEngine;
using TMPro;
using Services;
using System.Collections;

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
            StartCoroutine(UpdateTextCoroutine());
        }

        private IEnumerator UpdateTextCoroutine()
        {
            if (_text != null)
            {
                yield return ServiceManager.GetService<ILanguageService>().GetLocalizedValue(_key);
                _text.text = ServiceManager.GetService<ILanguageService>().GetLocalizedValue(_key).Current as string;
            }
        }

        private void OnDestroy()
        {
            ServiceManager.GetService<ILanguageService>().Unsubscribe(UpdateText);
        }
    }
}
