using Prefab;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Hierarchy
{
    internal class HierarchyManager : MonoBehaviour
    {
        [Header("Hierarchy Content")]
        [SerializeField] private Transform _hierarchyContent;

        [Header("Hierarchy Prefabs")]
        [SerializeField] private GameObject _hierarchyGroupButtonPrefab;
        [SerializeField] private GameObject _hierarchyGroupContainerPrefab;
        [SerializeField] private GameObject _hierarchyElementPrefab;

        [Header("Hierarchy New Group")]
        [SerializeField] private GameObject _newGroupInputField;

        [Header("Hierarchy Buttons")]
        [SerializeField] private Button _newGroupButton;
        [SerializeField] private Button _hierarchyPanelToggleButton;

        // Key: group button, Value: group container
        private Dictionary<GameObject, GameObject> _hierarchyGroups = new();

        // Key: group container, Value: items
        private Dictionary<GameObject, List<GameObject>> _hierarchyItems = new();

        private string _groupButtonPrefix = "BTN_";
        private string _groupContainerPrefix = "Container_";

        #region Group

        Coroutine newGroupCoroutine;

        private void CreateNewGroup()
        {
            if (newGroupCoroutine != null) StopCoroutine(newGroupCoroutine);
            newGroupCoroutine = GameManager.NewCoroutine(NewGroupCoroutine());
        }

        private IEnumerator NewGroupCoroutine()
        {
            _newGroupInputField.SetActive(true);

            var inputFiled = _newGroupInputField.GetComponentInChildren<TMPro.TMP_InputField>();

            string newGroup = "";
            var prefabService = ServiceManager.GetService<IPrefabService>();

            while (!Input.GetKeyDown(KeyCode.Escape))
            {
                if (Input.GetKeyUp(KeyCode.Return))
                {
                    newGroup = inputFiled.text;

                    if (prefabService.GetCategories().Contains(newGroup))
                        Log.Logger.Log_Error("group_exists", newGroup);
                    else
                    {
                        prefabService.NewCategory(newGroup);
                        inputFiled.text = "";
                        _newGroupInputField.SetActive(false);
                        CreateGroup(newGroup);
                        yield break;
                    }
                }
                yield return null;
            }
            inputFiled.text = "";
            _newGroupInputField.SetActive(false);
        }

        private GameObject CreateGroup(string groupName)
        {
            var groupButton = Instantiate(_hierarchyGroupButtonPrefab, _hierarchyContent);
            groupButton.name = _groupButtonPrefix + groupName ;
            groupButton.GetComponentInChildren<TMPro.TMP_Text>().text = groupName;

            var groupContainer = Instantiate(_hierarchyGroupContainerPrefab, _hierarchyContent);
            groupContainer.name = _groupContainerPrefix + groupName;
            groupButton.GetComponent<Button>().onClick.AddListener(() => groupContainer.SetActive(!groupContainer.activeSelf));

            _hierarchyGroups.Add(groupButton, groupContainer);
            _hierarchyItems.Add(groupContainer, new());

            return groupButton;
        }

        public void DeleteGroup(GameObject groupButton)
        {
            GameManager.NewCoroutine(DeleteGroupCoroutine());

            IEnumerator DeleteGroupCoroutine()
            {
                string groupName = groupButton.GetComponentInChildren<TMPro.TMP_Text>().text;

                bool result = false;
                yield return GameManager.NewCoroutine(PopupController.Instance.GetApproval((choice) => { result = choice; }, "delete_hierarchy_group", groupName));
                if (result == false) yield break;

                var groupContainer = _hierarchyGroups[groupButton];
                _hierarchyItems.Remove(groupContainer);
                _hierarchyGroups.Remove(groupButton);

                Destroy(groupContainer);
                Destroy(groupButton);
            }
        }

        #endregion

        #region Hierarchy Element

        public void LoadHierarchyElement(Prefab.Prefab prefab, string groupName)
        {
            GameObject groupButton = null;
            if (groupName != "")
            {
                groupButton = _hierarchyGroups.Keys.FirstOrDefault(g => g.GetComponentInChildren<TMPro.TMP_Text>().text == groupName);
                if (groupButton == null) groupButton = CreateGroup(groupName);
            }

            var newHierarchyElement = Instantiate(
                _hierarchyElementPrefab,
                groupName == "" ? _hierarchyContent : _hierarchyGroups[groupButton].transform);

            newHierarchyElement.GetComponent<HierarchyElement>().Prefab = prefab;
            newHierarchyElement.GetComponentInChildren<TMPro.TMP_Text>().text = prefab.Data.prefabName;

            if (groupName != "") _hierarchyItems[_hierarchyGroups[groupButton]].Add(newHierarchyElement);
        }

        public void AddHierarchyElement(Prefab.Prefab prefab)
        {
            var newHierarchyElement = Instantiate(_hierarchyElementPrefab, _hierarchyContent);
            newHierarchyElement.GetComponent<HierarchyElement>().Prefab = prefab;
            newHierarchyElement.GetComponentInChildren<TMPro.TMP_Text>().text = prefab.Data.prefabName;

            ServiceManager.GetService<ISceneService>().AddPrefabToCurrentScene(prefab);
        }

        public void ChangeHierarchyElementGroup(Transform element, GameObject targetGroup)
        {
            // If want to remove group
            if (targetGroup == _hierarchyContent.gameObject)
            {
                // Remove from previous group
                if (_hierarchyGroups.ContainsValue(element.parent.gameObject))
                {
                    _hierarchyItems[element.parent.gameObject].Remove(element.gameObject);
                    GameManager.NewCoroutine(GroupContainerRefresh(element.parent.gameObject));
                }
                
                element.SetParent(targetGroup.transform);
                element.GetComponent<HierarchyElement>().Prefab.Data.hierarchyGroupName = "";
            }
            // If targetGroup is a group button
            else if (_hierarchyGroups.ContainsKey(targetGroup))
            {
                // Remove from previous group
                if (_hierarchyGroups.ContainsValue(element.parent.gameObject))
                {
                    _hierarchyItems[element.parent.gameObject].Remove(element.gameObject);
                    GameManager.NewCoroutine(GroupContainerRefresh(element.parent.gameObject));
                }
                
                // Add to new group
                element.SetParent(_hierarchyGroups[targetGroup].transform);
                _hierarchyItems[_hierarchyGroups[targetGroup]].Add(element.gameObject);
                element.GetComponent<HierarchyElement>().Prefab.Data.hierarchyGroupName = targetGroup.name.Replace(_groupButtonPrefix, "");
                GameManager.NewCoroutine(GroupContainerRefresh(element.parent.gameObject));
            }
            
            // If targetGroup is a container
            else if (_hierarchyGroups.ContainsValue(targetGroup))
            {
                // Remove from previous group
                if (_hierarchyGroups.ContainsValue(element.parent.gameObject))
                {
                    _hierarchyItems[element.parent.gameObject].Remove(element.gameObject);
                    GameManager.NewCoroutine(GroupContainerRefresh(element.parent.gameObject));
                }

                // Add to new group
                element.SetParent(targetGroup.transform);
                _hierarchyItems[targetGroup].Add(element.gameObject);
                element.GetComponent<HierarchyElement>().Prefab.Data.hierarchyGroupName = targetGroup.name.Replace(_groupContainerPrefix, "");
                GameManager.NewCoroutine(GroupContainerRefresh(element.parent.gameObject));
            }

        }

        public void DeleteHierarchyElement(GameObject element, GameObject group = null)
        {
            GameManager.NewCoroutine(DeleteElementCoroutine());

            IEnumerator DeleteElementCoroutine()
            {
                string elementName = element.GetComponentInChildren<TMPro.TMP_Text>().text;

                bool result = false;
                yield return GameManager.NewCoroutine(PopupController.Instance.GetApproval((choice) => { result = choice; }, "delete_hierarchy_element", elementName));
                if (result == false) yield break;

                if (group == null)
                {
                    foreach (var container in _hierarchyItems.Keys)
                    {
                        if (_hierarchyItems[container].Contains(element))
                        {
                            _hierarchyItems[container].Remove(element);
                            ServiceManager.GetService<ISceneService>().DeletePrefab(element.GetComponent<HierarchyElement>().Prefab);
                            Destroy(element);
                            GameManager.NewCoroutine(GroupContainerRefresh(container));
                            yield break;
                        }
                    }
                }

                else if (_hierarchyItems.ContainsKey(group))
                {
                    _hierarchyItems[group].Remove(element);
                    ServiceManager.GetService<ISceneService>().DeletePrefab(element.GetComponent<HierarchyElement>().Prefab);
                    Destroy(element);
                    GameManager.NewCoroutine(GroupContainerRefresh(group));
                }
                
            }
        }

        IEnumerator GroupContainerRefresh(GameObject container)
        {
            container?.SetActive(false);
            yield return null;
            container?.SetActive(true);
        }

        #endregion

        private void Awake()
        {
            _newGroupButton.onClick.AddListener(CreateNewGroup);
        }
    }
}
