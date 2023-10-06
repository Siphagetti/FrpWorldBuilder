using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UserInterface.World.Building.Log
{
    internal class LogTimer : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Slider _slider;

        // It will begin to disappear after given seconds
        private static float _disappearTime = 3f;

        // It will disappear in given seconds
        private static float _fadeDuration = 1f;

        private static TimerStopper _timerStopper;

        private void Start()
        {
            if (_timerStopper == null) _timerStopper = transform.parent.GetComponent<TimerStopper>();
            StartCoroutine(Countdown());
        }

        IEnumerator Countdown()
        {
            float timer = 0;

            while (timer < _disappearTime)
            {
                yield return new WaitUntil(() => _timerStopper.TimerWait == false);

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
        public void OnPointerClick(PointerEventData eventData) { Destroy(gameObject); }
    }
}
