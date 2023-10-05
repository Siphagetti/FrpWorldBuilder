using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UserInterface.Helpers
{
    internal class LogTimer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Slider _slider;

        // It will begin to disappear after given seconds
        [SerializeField] private float _disappearTime = 5f;

        // It will disappear in given seconds
        [SerializeField] private float _fadeDuration = 2f;

        private bool _timerWait = false;

        private void Start()
        {
            StartCoroutine(Countdown());
        }

        IEnumerator Countdown()
        {
            float timer = 0;

            while (timer < _disappearTime)
            {
                yield return new WaitUntil(() => _timerWait == false);

                timer += Time.deltaTime;
                _slider.value = timer / _disappearTime;
                yield return null;
            }

            StartCoroutine(FadeOutThenDestroy());
        }

        IEnumerator FadeOutThenDestroy()
        {
            CanvasGroup _canvasGroup = GetComponent<CanvasGroup>();
            float currFadeOutTime = 0f;

            while (currFadeOutTime < _fadeDuration)
            {
                currFadeOutTime += Time.deltaTime;
                _canvasGroup.alpha = 1f - currFadeOutTime / _fadeDuration;
                yield return null;
            }

            Destroy(gameObject);
        }

        public void OnPointerEnter(PointerEventData eventData) { _timerWait = true; }
        public void OnPointerExit(PointerEventData eventData) { _timerWait = false; }
    }
}
