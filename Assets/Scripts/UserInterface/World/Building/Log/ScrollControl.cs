using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UserInterface.World.Building.Log
{
    public class ScrollControl : MonoBehaviour
    {
        [SerializeField] float scrollSpeed = 15f;
        private ScrollRect _scrollRect;

        private void Start()
        {
            _scrollRect = GetComponent<ScrollRect>();

            // Subscribe to the scroll event
            EventTrigger trigger = _scrollRect.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Scroll;
            entry.callback.AddListener((data) => { OnScrollEvent((PointerEventData)data); });
            trigger.triggers.Add(entry);

        }

        private void OnScrollEvent(PointerEventData eventData)
        {
            // Handle mouse wheel input to scroll the content
            float scrollDelta = eventData.scrollDelta.y * scrollSpeed;

            // If the content is at the top and event is scroll up then it should be prevented.
            if (scrollDelta < 0 && _scrollRect.normalizedPosition.y <= 0) return;

            // If the content is at the bottom and event is scroll down then it should be prevented.
            if (scrollDelta > 0 && _scrollRect.normalizedPosition.y >= 1) return;

            // Set new position of the content.
            float normalizedScrollDelta = scrollDelta / (_scrollRect.content.rect.height - _scrollRect.viewport.rect.height);
            _scrollRect.normalizedPosition += new Vector2(0, normalizedScrollDelta);

        }
    }
}