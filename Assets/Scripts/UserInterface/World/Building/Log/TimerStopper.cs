using UnityEngine;
using UnityEngine.EventSystems;

namespace UserInterface.World.Building.Log
{
    public class TimerStopper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private bool _timerWait = false;

        public bool TimerWait => _timerWait;

        public void OnPointerEnter(PointerEventData eventData) { _timerWait = true; }
        public void OnPointerExit(PointerEventData eventData) { _timerWait = false; }
    }
}

