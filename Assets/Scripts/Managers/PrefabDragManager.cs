using System.Collections;
using UnityEngine;

namespace Prefab
{
    public class PrefabDragManager : MonoBehaviour
    {
        // Camera distance for '_draggingObj'
        // This will come from camera movement script in the future. (maybe)
        private float CamDist { get; set; } = 10f;

        private bool _isDragging;
        private GameObject _selectedObject;

        // Mouse offset where cursor begin to drag the '_draggingObj'
        private Vector3 _mouseOffset;

        // Target layer for raycast.
        public LayerMask targetLayer;

        private void Update() { HandleMouseInput(); }

        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonUp(0) && _isDragging) StopDragging();

            if (_isDragging) MoveSelectedObjectWithCursor();
            else if (Input.GetMouseButtonDown(0)) HandleMouseClick();
        }

        private void StopDragging()
        {
            _isDragging = false;
            RevealPanel();
        }

        private void MoveSelectedObjectWithCursor()
        {
            Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, CamDist);
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            _selectedObject.transform.position = newPosition + _mouseOffset;
        }

        private void HandleMouseClick()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer)) SelectObject(hit.collider.gameObject);
            else if (_selectedObject != null) DeselectObject();

        }

        private void SelectObject(GameObject obj)
        {
            DontShineMesh(); // If there is a previously selected mesh, stop its shining.

            _selectedObject = obj;
            _mouseOffset = _selectedObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, CamDist));

            ShineMesh();
            HidePanel();

            _isDragging = true;
        }

        private void DeselectObject()
        {
            DontShineMesh();
            _selectedObject = null;
            _isDragging = false;
        }


        // Requires for Thumbnail to move its prefab after instantiate it.
        public void SetSelectedObject(GameObject obj)
        {
            Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, CamDist);
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            obj.transform.position = newPosition;

            DontShineMesh(); // If there is a previous selected mesh stop its shining.

            _selectedObject = obj;
            _mouseOffset = obj.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, CamDist));
            _isDragging = true;
            ShineMesh();
        }

        // Requires for Thumbnail to deselect its prefab.
        public void RemoveSelectedObject()
        {
            DontShineMesh();
            _selectedObject = null;
            _isDragging = false;
        }

        private void Awake()
        {
            Thumbnail.dragManager = this;
            _yPos = (int)GetComponent<RectTransform>().anchoredPosition.y;
        }

        #region Shine Mesh

        private void ShineMesh() { if (_selectedObject != null) _selectedObject.GetComponent<MeshRenderer>().enabled = true; }
        private void DontShineMesh() { if (_selectedObject != null) _selectedObject.GetComponent<MeshRenderer>().enabled = false; }

        #endregion

        #region Hide or Reveal Panel

        private int _yPos;
        private float _duration = 0.3f;

        Coroutine HideOrRevealCoorutine;

        private void HidePanel()
        {
            if (HideOrRevealCoorutine != null)
                StopCoroutine(HideOrRevealCoorutine);

            HideOrRevealCoorutine = StartCoroutine(HidePanelCoroutine());
        }

        private void RevealPanel()
        {
            if (HideOrRevealCoorutine != null)
                StopCoroutine(HideOrRevealCoorutine);

            HideOrRevealCoorutine = StartCoroutine(RevealPanelCoroutine());
        }

        IEnumerator HidePanelCoroutine()
        {
            RectTransform rect = GetComponent<RectTransform>();

            Vector2 startPos = rect.anchoredPosition;
            Vector2 targetPos = new Vector2(startPos.x, -_yPos);

            for (float t = 0; t < 1.0f; t += Time.deltaTime / _duration)
            {
                rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
                yield return null;
            }

            rect.anchoredPosition = targetPos; // Ensure the final position is exactly at the target

            HideOrRevealCoorutine = null;
        }

        IEnumerator RevealPanelCoroutine()
        {
            RectTransform rect = GetComponent<RectTransform>();

            Vector2 startPos = rect.anchoredPosition;
            Vector2 targetPos = new Vector2(startPos.x, _yPos);
            // Adjust the duration as needed

            for (float t = 0; t < 1.0f; t += Time.deltaTime / _duration)
            {
                rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
                yield return null;
            }

            rect.anchoredPosition = targetPos; // Ensure the final position is exactly at the target

            HideOrRevealCoorutine = null;
        }

        #endregion
    }
}
