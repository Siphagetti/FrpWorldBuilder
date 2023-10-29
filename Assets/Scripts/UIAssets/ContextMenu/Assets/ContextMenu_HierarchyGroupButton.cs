using Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace ContextMenu
{
    internal class ContextMenu_HierarchyGroupButton : ContextMenu
    {
        private void Start()
        {
            var hierarchyManager = FindFirstObjectByType<HierarchyManager>();

            var deleteButton = transform.GetChild(0).GetComponent<Button>();
            deleteButton.onClick.AddListener(() => hierarchyManager.DeleteGroup(_owner));
            deleteButton.onClick.AddListener(() => Destroy(gameObject));
        }
    }
}
