using UnityEngine;

namespace Prefab
{
    public class DragManager : MonoBehaviour
    {
        // Camera distance for '_draggingObj'
        // This will come from camera movement script in the future. (maybe)
        private float CamDist { get; set; } = 10f;

        private GameObject _draggingObj;
        private GameObject _hoveringObj;

        // Mouse offset where cursor begin to drag the '_draggingObj'
        private Vector3 _mouseOffset;

        // Target layer for raycast.
        public LayerMask targetLayer;

        // Requires for creating rim materials of prefabs.
        // Set this to Prefab.rimLightMaterial on awake 
        public Material rimLightMaterial;


        private void FixedUpdate()
        {
            if (!Input.GetKey(KeyCode.LeftControl))
            {
                if (_hoveringObj)
                {
                    _hoveringObj.GetComponent<Prefab>().ChangeMaterialsAsOrj();
                    _hoveringObj = null;
                }
                return;
            }

            if (_draggingObj != null) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer))
            {
                var prefab = hit.collider.gameObject;

                if (prefab != _hoveringObj)
                {
                    _hoveringObj?.GetComponent<Prefab>().ChangeMaterialsAsOrj();
                    _hoveringObj = prefab;
                    _hoveringObj.GetComponent<Prefab>().ChangeMaterialsAsRim();
                }
            }
            else if (_hoveringObj)
            {
                _hoveringObj.GetComponent<Prefab>().ChangeMaterialsAsOrj();
                _hoveringObj = null;
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0) && _draggingObj != null) 
            { 
                _draggingObj.GetComponent<Prefab>().ChangeMaterialsAsOrj(); 
                _draggingObj = null;
                _hoveringObj = null;
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

            obj.GetComponent<Prefab>().ChangeMaterialsAsRim();
        }

        public void removeDraggingObj()
        {
            _draggingObj = null;
        }

        private void Awake()
        {
            Thumbnail.dragManager = this;
            Prefab.rimLightMaterial = rimLightMaterial;
        }
    }
}
