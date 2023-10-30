using Prefab;
using Services;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Hierarchy
{
    public class HierarchyManager : MonoBehaviour
    {
        // Serialized fields for inspector setup
        [Header("Hierarchy Content")]
        [SerializeField] private Transform hierarchyContent;

        [Header("Hierarchy Prefabs")]
        [SerializeField] private GameObject hierarchyGroupButtonPrefab;
        [SerializeField] private GameObject hierarchyGroupContainerPrefab;
        [SerializeField] private GameObject hierarchyElementPrefab;

        [Header("Hierarchy New Group")]
        [SerializeField] private GameObject newGroupInputField;

        [Header("Hierarchy Buttons")]
        [SerializeField] private Button newGroupButton;
        [SerializeField] private Button hierarchyPanelToggleButton;

        // Constants for group and container prefixes
        private const string GroupButtonPrefix = "BTN_";
        private const string GroupContainerPrefix = "Container_";

        // Dictionaries to manage hierarchy groups and items

        // Key: group button - Value: hierarchy item container
        private readonly Dictionary<GameObject, GameObject> hierarchyGroups = new Dictionary<GameObject, GameObject>();
        // Key: hierarchy item container - Value: hierarchy items
        private readonly Dictionary<GameObject, List<GameObject>> hierarchyItems = new Dictionary<GameObject, List<GameObject>>();

        private Coroutine newGroupCoroutine;

        // Awake is called when the script instance is being loaded.
        private void Awake()
        {
            // Attach a click listener to the "New Group" button.
            newGroupButton.onClick.AddListener(CreateNewGroup);
        }

        // Coroutine to handle creating a new group
        private void CreateNewGroup()
        {
            if (newGroupCoroutine != null) StopCoroutine(newGroupCoroutine);
            newGroupCoroutine = StartCoroutine(NewGroupCoroutine());
        }

        // Coroutine to create a new group with user input
        private IEnumerator NewGroupCoroutine()
        {
            // Show the input field for creating a new group.
            newGroupInputField.SetActive(true);

            var inputField = newGroupInputField.GetComponentInChildren<TMPro.TMP_InputField>();

            string newGroup = "";
            var prefabService = ServiceManager.GetService<IPrefabService>();

            while (!Input.GetKeyDown(KeyCode.Escape))
            {
                if (Input.GetKeyUp(KeyCode.Return))
                {
                    newGroup = inputField.text;

                    // Check if the new group name already exists.
                    if (prefabService.GetCategories().Contains(newGroup))
                    {
                        Log.Logger.Log_Error("group_exists", newGroup);
                    }
                    else
                    {
                        // Create a new group and add it to the hierarchy.
                        prefabService.NewCategory(newGroup);
                        inputField.text = "";
                        newGroupInputField.SetActive(false);
                        CreateGroup(newGroup);
                        yield break;
                    }
                }
                yield return null;
            }
            inputField.text = "";
            newGroupInputField.SetActive(false);
        }

        // Create a new group and add it to the hierarchy
        private GameObject CreateGroup(string groupName)
        {
            // Instantiate a new group button and group container.
            var groupButton = Instantiate(hierarchyGroupButtonPrefab, hierarchyContent);
            groupButton.name = GroupButtonPrefix + groupName;
            groupButton.GetComponentInChildren<TMPro.TMP_Text>().text = groupName;

            var groupContainer = Instantiate(hierarchyGroupContainerPrefab, hierarchyContent);
            groupContainer.name = GroupContainerPrefix + groupName;

            // Attach a click listener to toggle the group container's visibility.
            groupButton.GetComponent<Button>().onClick.AddListener(() => groupContainer.SetActive(!groupContainer.activeSelf));

            // Add the group button and container to the dictionaries.
            hierarchyGroups.Add(groupButton, groupContainer);
            hierarchyItems.Add(groupContainer, new List<GameObject>());

            return groupButton;
        }

        // Delete a group from the hierarchy
        public void DeleteGroup(GameObject groupButton)
        {
            StartCoroutine(DeleteGroupCoroutine(groupButton));
        }

        // Coroutine to handle the deletion of a group
        private IEnumerator DeleteGroupCoroutine(GameObject groupButton)
        {
            string groupName = groupButton.GetComponentInChildren<TMPro.TMP_Text>().text;

            bool result = false;

            // Show a confirmation popup before deleting the group.
            yield return StartCoroutine(PopupController.Instance.GetApproval((choice) => { result = choice; }, "delete_hierarchy_group", groupName));
            if (result == false) yield break;

            var groupContainer = hierarchyGroups[groupButton];

            // Remove the group container and button from the dictionaries.
            hierarchyItems.Remove(groupContainer);
            hierarchyGroups.Remove(groupButton);

            // Destroy the group container and button.
            Destroy(groupContainer);
            Destroy(groupButton);
        }

        // Load a hierarchy element into a group
        public void LoadHierarchyElement(Prefab.Prefab prefab, string groupName)
        {
            GameObject groupButton = null;

            if (groupName != "")
            {
                // Find the group button associated with the given group name, or create one if not found.
                groupButton = hierarchyGroups.Keys.FirstOrDefault(g => g.GetComponentInChildren<TMPro.TMP_Text>().text == groupName);
                if (groupButton == null) groupButton = CreateGroup(groupName);
            }

            // Instantiate a new hierarchy element and add it to the hierarchy.
            var newHierarchyElement = Instantiate(
                hierarchyElementPrefab,
                groupName == "" ? hierarchyContent : hierarchyGroups[groupButton].transform);

            newHierarchyElement.GetComponent<HierarchyElement>().Prefab = prefab;
            newHierarchyElement.GetComponentInChildren<TMPro.TMP_Text>().text = prefab.Data.prefabName;

            if (groupName != "") hierarchyItems[hierarchyGroups[groupButton]].Add(newHierarchyElement);
        }

        // Add a hierarchy element to the root of the hierarchy
        public void AddHierarchyElement(Prefab.Prefab prefab)
        {
            // Instantiate a new hierarchy element and add it to the root of the hierarchy.
            var newHierarchyElement = Instantiate(hierarchyElementPrefab, hierarchyContent);
            newHierarchyElement.GetComponent<HierarchyElement>().Prefab = prefab;
            newHierarchyElement.GetComponentInChildren<TMPro.TMP_Text>().text = prefab.Data.prefabName;

            ServiceManager.GetService<ISceneService>().AddPrefabToCurrentScene(prefab);
        }

        // Change the group of a hierarchy element
        public void ChangeHierarchyElementGroup(Transform element, GameObject targetGroup)
        {
            // Check if the target group is the root hierarchy content.
            if (targetGroup == hierarchyContent.gameObject)
            {
                // If the element's current parent is a group container, remove it from the group's item list.
                if (hierarchyGroups.ContainsValue(element.parent.gameObject))
                {
                    hierarchyItems[element.parent.gameObject].Remove(element.gameObject);
                    StartCoroutine(GroupContainerRefresh(element.parent.gameObject));
                }

                // Set the element's parent to the root hierarchy content.
                element.SetParent(targetGroup.transform);
                element.GetComponent<HierarchyElement>().Prefab.Data.hierarchyGroupName = "";
            }
            // Check if the target group is an existing group button.
            else if (hierarchyGroups.ContainsKey(targetGroup))
            {
                // If the element's current parent is a group container, remove it from the group's item list.
                if (hierarchyGroups.ContainsValue(element.parent.gameObject))
                {
                    hierarchyItems[element.parent.gameObject].Remove(element.gameObject);
                    StartCoroutine(GroupContainerRefresh(element.parent.gameObject));
                }

                // Set the element's parent to the selected group's container.
                element.SetParent(hierarchyGroups[targetGroup].transform);

                // Add the element to the selected group's item list.
                hierarchyItems[hierarchyGroups[targetGroup]].Add(element.gameObject);

                // Update the element's hierarchy group name based on the target group's name.
                element.GetComponent<HierarchyElement>().Prefab.Data.hierarchyGroupName = targetGroup.name.Replace(GroupButtonPrefix, "");

                // Refresh the target group's visibility in the hierarchy.
                StartCoroutine(GroupContainerRefresh(element.parent.gameObject));
            }
            // Check if the target group is an existing group container.
            else if (hierarchyGroups.ContainsValue(targetGroup))
            {
                // If the element's current parent is a group container, remove it from the group's item list.
                if (hierarchyGroups.ContainsValue(element.parent.gameObject))
                {
                    hierarchyItems[element.parent.gameObject].Remove(element.gameObject);
                    StartCoroutine(GroupContainerRefresh(element.parent.gameObject));
                }

                // Set the element's parent to the selected group's container.
                element.SetParent(targetGroup.transform);

                // Add the element to the selected group's item list.
                hierarchyItems[targetGroup].Add(element.gameObject);

                // Update the element's hierarchy group name based on the target group's name.
                element.GetComponent<HierarchyElement>().Prefab.Data.hierarchyGroupName = targetGroup.name.Replace(GroupContainerPrefix, "");

                // Refresh the target group's visibility in the hierarchy.
                StartCoroutine(GroupContainerRefresh(element.parent.gameObject));
            }
        }

        // Delete a hierarchy element
        public void DeleteHierarchyElement(GameObject element, GameObject group = null)
        {
            StartCoroutine(DeleteElementCoroutine(element, group));
        }

        // Coroutine to handle the deletion of a hierarchy element
        private IEnumerator DeleteElementCoroutine(GameObject element, GameObject group = null)
        {
            string elementName = element.GetComponentInChildren<TMPro.TMP_Text>().text;

            bool result = false;

            // Show a confirmation popup before deleting the element.
            yield return StartCoroutine(PopupController.Instance.GetApproval((choice) => { result = choice; }, "delete_hierarchy_element", elementName));
            if (result == false) yield break;

            ServiceManager.GetService<ISceneService>().DeletePrefab(element.GetComponent<HierarchyElement>().Prefab);
            Destroy(element);

            if (group == null)
            {
                foreach (var container in hierarchyItems.Keys)
                {
                    if (hierarchyItems[container].Contains(element))
                    {
                        hierarchyItems[container].Remove(element);
                        StartCoroutine(GroupContainerRefresh(container));
                        yield break;
                    }
                }
            }
            else if (hierarchyItems.ContainsKey(group))
            {
                hierarchyItems[group].Remove(element);
                StartCoroutine(GroupContainerRefresh(group));
            }
        }

        // Coroutine to refresh a group container's visibility
        private IEnumerator GroupContainerRefresh(GameObject container)
        {
            container?.SetActive(false);
            yield return null;
            container?.SetActive(true);
        }

        // Reset the hierarchy by clearing the dictionaries
        public void ResetHierarchy()
        {
            hierarchyItems.Clear();
            hierarchyGroups.Clear();
        }
    }
}
