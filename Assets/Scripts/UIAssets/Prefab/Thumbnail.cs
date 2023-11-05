using Hierarchy;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Prefab
{
    public class Thumbnail : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        // Properties to set the owner UI panel's RectTransform and the drag manager
        public static RectTransform OwnerUIPanelRect { get; set; }
        public static PrefabDragManager dragManager { get; set; }

        [SerializeField] private RawImage _image;
        [SerializeField] private TMPro.TMP_Text _text;

        private GameObject _prefab;

        // Reference to the parent transform of the UI element
        private Transform _parentTransform;

        // Keep the initial sibling index to restore it after dragging
        private int _initialSiblingIndex;

        // Offset for dragging the UI element from where it's held
        private Vector3 _mouseOffset;

        // Flag to hide the UI element while dragging the prefab
        private bool _hideUI;

        // The spawned prefab that follows the cursor
        private GameObject _spawnedPrefab;

        // Position to place the spawned prefab when hidden
        private Vector3 _prefabSpawnPos = 10000 * Vector3.one;

        public void OnBeginDrag(PointerEventData eventData)
        {
            // Store the parent transform of the UI element
            _parentTransform = transform.parent;

            // Store the initial sibling index
            _initialSiblingIndex = transform.GetSiblingIndex();

            // Reparent the UI element to the root canvas
            transform.SetParent(transform.root);

            // Calculate the mouse offset
            _mouseOffset = transform.position - Input.mousePosition;

            // Initialize the hide UI flag
            _hideUI = false;

            // Create the spawned prefab at an invisible position
            _spawnedPrefab = _prefab.GetComponent<Prefab>().CreateWrapper(_prefabSpawnPos);
        }

        public void OnDrag(PointerEventData eventData)
        {
            // If the UI element should not be hidden, move it with the cursor
            if (!_hideUI)
                transform.position = Input.mousePosition + _mouseOffset;

            // Control UI position based on cursor position
            Vector2 localMousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(OwnerUIPanelRect, Input.mousePosition, null, out localMousePosition);

            // Compare the y-coordinate to determine if the mouse is higher or lower
            float containerHeight = OwnerUIPanelRect.rect.height;

            if (localMousePosition.y > containerHeight / 2)
            {
                // If the UI element was already hidden, return
                if (_hideUI) return;

                // Set the spawned prefab as the dragging object in the drag manager
                dragManager.SetSelectedObject(_spawnedPrefab);

                // Hide the UI image and text
                _image.gameObject.SetActive(false);
                _text.gameObject.SetActive(false);

                // Update the hide UI flag
                _hideUI = true;
            }
            else
            {
                // If the UI element was not hidden, return
                if (!_hideUI) return;

                // Restore the spawned prefab's position to its original hidden position
                // and unassign the dragging object in the drag manager
                _spawnedPrefab.transform.position = _prefabSpawnPos;
                dragManager.DeselectObject();

                // Show the UI image and text
                _image.gameObject.SetActive(true);
                _text.gameObject.SetActive(true);

                // Update the hide UI flag
                _hideUI = false;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // If the UI element was hidden, show the UI image and text
            if (_hideUI)
            {
                _image.gameObject.SetActive(true);
                _text.gameObject.SetActive(true);
            }

            // Check if the spawned prefab's position is the hidden position
            if (_spawnedPrefab.transform.position == _prefabSpawnPos)
            {
                // Destroy the spawned prefab
                Destroy(_spawnedPrefab);
            }
            else
            {
                // Update the prefab's transform and assign a new GUID
                Prefab prefab = _spawnedPrefab.GetComponent<Prefab>();
                prefab.UpdateTransform();
                prefab.Data.guid = Guid.NewGuid().ToString();
                // Add the hierarchy element to the hierarchy manager
                FindFirstObjectByType<HierarchyManager>().AddHierarchyElement(prefab);
            }

            // Set the UI's parent as its initial parent
            transform.SetParent(_parentTransform);

            // Restore the element's sibling index to the initial index
            transform.SetSiblingIndex(_initialSiblingIndex);
        }

        internal void SetPrefab(GameObject prefab)
        {
            _prefab = prefab;
            _text.text = prefab.name;
        }
    }
}
