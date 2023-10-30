using Hierarchy;
using Save;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Prefab
{
    public class Thumbnail : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public static RectTransform OwnerUIPanelRect { get; set; }
        public static PrefabDragManager dragManager { get; set; }

        [SerializeField] private RawImage _image;
        [SerializeField] private TMPro.TMP_Text _text;

        private GameObject _prefab;

        // Content that UI element belongs to.
        private Transform _parentTransform;

        // Keep sibling index to put in its beginning place 
        private int _initialSiblingIndex;

        // Offset for drag the UI element from where its been hold.
        private Vector3 _mouseOffset;

        // Hide UI when cursor should drag the spawned prefab.
        private bool _hideUI;


        private GameObject _spawnedPrefab;
        private Vector3 _prefabSpawnPos = 10000 * Vector3.one;

        public void OnBeginDrag(PointerEventData eventData)
        {
            _parentTransform = transform.parent;

            // Store the initial sibling index
            _initialSiblingIndex = transform.GetSiblingIndex();

            transform.SetParent(transform.root);

            _mouseOffset = transform.position - Input.mousePosition;

            _hideUI = false;

            // Keep instantiated prefab at invisible position.
            _spawnedPrefab = _prefab.GetComponent<Prefab>().CreateWrapper(_prefabSpawnPos);
        }

        public void OnDrag(PointerEventData eventData)
        {
            // If prefab not spawned, just move the ui element
            if (!_hideUI) transform.position = Input.mousePosition + _mouseOffset;

            // -------------- Control UI Position --------------

            // Convert the screen space position to the owner UI panel's local position
            Vector2 localMousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(OwnerUIPanelRect, Input.mousePosition, null, out localMousePosition);

            // Compare the y-coordinate to determine if the mouse is higher or lower
            float elementHeight = OwnerUIPanelRect.rect.height;

            if (localMousePosition.y > elementHeight / 2)
            {
                if (_hideUI) return;

                // Set instantiated prefab as dragging object of Drag Manager.
                dragManager.SetSelectedObject(_spawnedPrefab);

                _image.gameObject.SetActive(false);
                _text.gameObject.SetActive(false);

                _hideUI = true;
            }
            else
            {
                if (!_hideUI) return;

                // Set instantiated prefab's position as '_prefabSpawnPos' back  
                // and unassign dragging object
                _spawnedPrefab.transform.position = _prefabSpawnPos;
                dragManager.DeselectObject();

                _image.gameObject.SetActive(true);
                _text.gameObject.SetActive(true);

                _hideUI = false;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_hideUI)
            {
                _image.gameObject.SetActive(true);
                _text.gameObject.SetActive(true);
            }

            if (_spawnedPrefab.transform.position == _prefabSpawnPos) Destroy(_spawnedPrefab);
            else
            {
                Prefab prefab = _spawnedPrefab.GetComponent<Prefab>();
                prefab.UpdateTransform();
                prefab.Data.guid = Guid.NewGuid().ToString();
                FindFirstObjectByType<HierarchyManager>().AddHierarchyElement(prefab);
            }
            // Set UI's parent as its beginning parent.
            transform.SetParent(_parentTransform);

            // Set the element's sibling index back to the initial index
            transform.SetSiblingIndex(_initialSiblingIndex);
        }

        internal void SetPrefab(GameObject prefab)
        {
            _prefab = prefab;
            _text.text = prefab.name;
        }
    }
}
