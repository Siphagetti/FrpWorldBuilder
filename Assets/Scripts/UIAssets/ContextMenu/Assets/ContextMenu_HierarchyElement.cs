using Hierarchy;
using UnityEngine.UI;

namespace ContextMenu
{
    internal class ContextMenu_HierarchyElement : ContextMenu
    {
        private void Start()
        {
            var hierarchyManager = FindFirstObjectByType<HierarchyManager>();

            var deleteButton = transform.GetChild(0).GetComponent<Button>();
            deleteButton.onClick.AddListener(() => hierarchyManager.DeleteHierarchyElement(_owner));
            deleteButton.onClick.AddListener(() => Destroy(gameObject));
        }
    }
}
