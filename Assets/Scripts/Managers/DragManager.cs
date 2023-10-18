using System.Collections;
using UnityEngine;

namespace Prefab
{
    public class DragManager : MonoBehaviour
    {
        // Camera distance for '_draggingObj'
        // This will come from camera movement script in the future. (maybe)
        private float CamDist { get; set; } = 10f;

        private GameObject _draggingObj;

        // Mouse offset where cursor begin to drag the '_draggingObj'
        private Vector3 _mouseOffset;

        // Target layer for raycast.
        public LayerMask targetLayer;

        private void Update()
        {
            if (Input.GetMouseButtonUp(0) && _draggingObj != null)
            {
                _draggingObj.GetComponent<Prefab>().DontShine();
                _draggingObj = null;
                RevealPanel();
            }

            if (_draggingObj != null)
            {
                Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, CamDist);
                Vector3 newPosition = Camera.main.ScreenToWorldPoint(screenPosition);

                _draggingObj.transform.position = newPosition + _mouseOffset;
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer))
                    {
                        _draggingObj = hit.collider.gameObject;
                        _mouseOffset = _draggingObj.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, CamDist));

                        _draggingObj.GetComponent<Prefab>().Shine();

                        HidePanel();
                    }
                }
            }
        }

        public void SetDraggingObj(GameObject obj)
        {
            Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, CamDist);
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            obj.transform.position = newPosition;

            _draggingObj = obj;
            _mouseOffset = obj.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, CamDist));

            _draggingObj.GetComponent<Prefab>().Shine();
        }

        public void removeDraggingObj()
        {
            _draggingObj = null;
        }

        private void Awake()
        {
            Thumbnail.dragManager = this;
            _yPos = (int)GetComponent<RectTransform>().anchoredPosition.y;
        }

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
