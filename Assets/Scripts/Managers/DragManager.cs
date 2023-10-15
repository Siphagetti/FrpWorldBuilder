using UnityEngine;

namespace Managers.WorldBuilding
{
    internal class DragManager : MonoBehaviour
    {
        public static float CamDist { get; set; } = 10f;

        private static DragManager _instance;

        private GameObject _draggingObj;
        private Vector3 _mouseOffset;

        public LayerMask targetLayer;

        public Material rimLightingMaterial;

        private void Update()
        {
            if (Input.GetMouseButtonUp(0)) _draggingObj = null;

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
                    }
                }
            }
        }

        public static void SetDraggingObj(GameObject obj)
        {
            Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, CamDist);
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            obj.transform.position = newPosition;

            _instance._draggingObj = obj;
            _instance._mouseOffset = obj.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, CamDist));
        }

        public static void DestroyDraggingObj()
        {
            Destroy(_instance._draggingObj);
            _instance._draggingObj = null;
        }

        private void Awake()
        {
            if ( _instance != null) { Destroy( _instance ); return; }
            _instance = this;
        }
    }
}
