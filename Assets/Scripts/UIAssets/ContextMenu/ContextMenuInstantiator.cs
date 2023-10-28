using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ContextMenu
{
    /*
        Add this script to a ui element that has a context menu.
    */

    internal class ContextMenuInstantiator : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject _contextMenuPrefab;
        private GameObject _contextMenu;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right) 
            {
                if (_contextMenu.IsDestroyed()) _contextMenu = null;
                else if (_contextMenu != null) return;

                _contextMenu = Instantiate(_contextMenuPrefab, transform.root);
                _contextMenu.GetComponent<ContextMenu>().SetOwner(gameObject);
                _contextMenu.transform.position = Input.mousePosition;

                Vector2 sizeDelta = _contextMenu.GetComponent<RectTransform>().sizeDelta;
                _contextMenu.transform.localPosition += new Vector3(sizeDelta.x / 2, sizeDelta.y / 2, 0);
            }
        }
    }
}
