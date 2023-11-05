using Prefab;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hierarchy
{
    internal class HierarchyElement : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public static HierarchyManager HierarchyManager;

        public Prefab.Prefab Prefab { get; set; }

        private Vector3 _mouseOffset;

        // Reference to the parent transform of the UI element
        private Transform _parentTransform;

        public void OnBeginDrag(PointerEventData eventData)
        {
            _mouseOffset = transform.position - Input.mousePosition;
            _parentTransform = transform.parent;

            // Reparent the UI element to the root canvas
            transform.SetParent(transform.root);
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition + _mouseOffset;
        }

        public void OnEndDrag(PointerEventData eventData)
        {

            // Raycast to find the drop target (category)
            GraphicRaycaster raycaster = transform.root.GetComponent<GraphicRaycaster>();
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = eventData.position;

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerEventData, results);

            foreach (RaycastResult result in results)
            {
                // Check if the result is a valid drop target (e.g., another UI element representing a category)
                if (result.gameObject.CompareTag(HierarchyManager.GroupTag))
                {
                    FindFirstObjectByType<HierarchyManager>().ChangeHierarchyElementGroup(transform, result.gameObject);
                    return;
                }
            }

            // Put the element back in its original category if no valid drop target was found.
            if (_parentTransform.CompareTag(HierarchyManager.GroupTag)) 
                FindFirstObjectByType<HierarchyManager>().ChangeHierarchyElementGroup(transform, _parentTransform.parent.gameObject);
            else transform.SetParent(_parentTransform);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                FindFirstObjectByType<PrefabDragManager>().SelectObject(Prefab.gameObject);
        }

        private void OnDestroy()
        {
            Destroy(Prefab.gameObject);
        }
    }
}
