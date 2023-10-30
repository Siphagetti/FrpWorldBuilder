using Prefab;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Hierarchy
{
    internal class HierarchyElement : MonoBehaviour, IPointerClickHandler
    {
        public Prefab.Prefab Prefab { get; set; }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                FindFirstObjectByType<PrefabDragManager>().SelectObject(Prefab.gameObject);
        }
    }
}
