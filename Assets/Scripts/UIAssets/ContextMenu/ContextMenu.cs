using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ContextMenu
{
    internal class ContextMenu : MonoBehaviour
    {
        protected GameObject _owner;

        public void SetOwner(GameObject owner) => _owner = owner;

        private void Update()
        {
            if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && !IsCursorOverUIElement())
                Destroy(gameObject);
        }

        private bool IsCursorOverUIElement()
        {
            // Check if the mouse cursor is over a UI element
            if (EventSystem.current != null)
            {
                PointerEventData eventData = new PointerEventData(EventSystem.current);
                eventData.position = Input.mousePosition;
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, results);

                // Check if any of the raycast results are UI elements
                foreach (RaycastResult result in results)
                {
                    if (result.gameObject == gameObject)
                    {
                        // A UI element was found under the cursor
                        return true;
                    }
                }
            }

            // No UI element was found under the cursor
            return false;
        }
    }
}
