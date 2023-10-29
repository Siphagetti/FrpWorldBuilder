using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Hierarchy
{
    internal class HierarchyElement : MonoBehaviour
    {
        public Prefab.Prefab Prefab { get; set; }

        private string groupName;

        public void SetGroup(string groupName)
        {
            this.groupName = groupName;
            Prefab.Data.hierarchyGroupName = groupName;
        }
    }
}
