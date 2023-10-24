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
                IEnumerator localizedValueCoroutine = ServiceManager.GetService<ILanguageService>().GetLocalizedValue(_key);

                yield return GameManager.NewCoroutine(localizedValueCoroutine);

                if (localizedValueCoroutine.Current is string) _text.text = (string)localizedValueCoroutine.Current;
            }
        }

        private void OnDestroy()
        {
            ServiceManager.GetService<ILanguageService>().Unsubscribe(UpdateText);
        }
    }
}
