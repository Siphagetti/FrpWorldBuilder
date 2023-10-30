using Services;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Prefab
{
    internal class CategoryComponent : MonoBehaviour
    {
        [SerializeField] private Transform CategoryContent; // Keeps all category elements.

        [SerializeField] private GameObject CategoryButtonPrefab; // Toggles related asset bundles button container.

        [SerializeField] private GameObject AssetBundlesButtonContainer; // Keeps asset bundles' buttons.

        [SerializeField] private GameObject AssetBundleButtonPrefab; // Appears asset bundle's prefabs in the UI container.

        [SerializeField] private GameObject NewCategoryPanel; // While adding a new category, this panel takes a new category name via an input field.

        // Key:     category
        // Value:   Category button and asset bundle button container
        private Dictionary<string, (GameObject, GameObject)> _bundleContainers = new();

        #region Asset Bundle

        public GameObject CreateAssetBundleButton(string category, string bundleName, GameObject relatedThumbnailContent)
        {
            if (!_bundleContainers.ContainsKey(category)) return null;

            // Create an asset bundle button in the specified category container.
            var bundleBtn = Instantiate(AssetBundleButtonPrefab, _bundleContainers[category].Item2.transform);
            bundleBtn.GetComponent<Button>().onClick.AddListener(() =>
                GetComponent<ThumbnailsComponent>().ChangeCurrentThumbnailContent(relatedThumbnailContent));
            bundleBtn.GetComponentInChildren<TMPro.TMP_Text>().text = bundleName;

            // Refresh the asset bundle container to ensure it updates correctly.
            GameManager.NewCoroutine(AssetBundleContainerRefresh(_bundleContainers[category].Item2));

            return _bundleContainers[category].Item2;
        }

        public void RemoveAssetBundle(string category, string bundleName)
        {
            Transform container = _bundleContainers[category].Item2.transform;

            for (int i = 0; i < container.childCount; i++)
            {
                var assetBundle = container.GetChild(i);

                if (assetBundle.GetComponentInChildren<TMPro.TMP_Text>().text == bundleName)
                {
                    // Remove the specified asset bundle button from the container.
                    Destroy(assetBundle.gameObject);

                    // Refresh the asset bundle container to ensure it updates correctly.
                    GameManager.NewCoroutine(AssetBundleContainerRefresh(container.gameObject));
                    return;
                }
            }
        }

        IEnumerator AssetBundleContainerRefresh(GameObject container)
        {
            container?.SetActive(false);
            yield return null;
            container?.SetActive(true);
        }

        #endregion

        #region Category

        Coroutine newCategoryCoroutine;

        private void NewCategory()
        {
            if (newCategoryCoroutine != null) StopCoroutine(newCategoryCoroutine);
            newCategoryCoroutine = GameManager.NewCoroutine(AddNewCategory());
        }

        private IEnumerator AddNewCategory()
        {
            NewCategoryPanel.SetActive(true);
            var inputFiled = NewCategoryPanel.GetComponentInChildren<TMPro.TMP_InputField>();

            string newCategory = "";
            var prefabService = ServiceManager.GetService<IPrefabService>();

            while (!Input.GetKeyDown(KeyCode.Escape))
            {
                if (Input.GetKeyUp(KeyCode.Return))
                {
                    newCategory = inputFiled.text;

                    if (prefabService.GetCategories().Contains(newCategory))
                        Log.Logger.Log_Error("category_exists", newCategory);
                    else
                    {
                        // Create a new category and add it to the UI.
                        prefabService.NewCategory(newCategory);
                        inputFiled.text = "";
                        NewCategoryPanel.SetActive(false);
                        CreateCategory(newCategory);
                        yield break;
                    }
                }
                yield return null;
            }
            inputFiled.text = "";
            NewCategoryPanel.SetActive(false);
        }

        public void CreateCategory(string category)
        {
            // Create a new category button and its associated asset bundle button container.
            var categoryBtn = Instantiate(CategoryButtonPrefab, CategoryContent);
            var bundleContainer = Instantiate(AssetBundlesButtonContainer, CategoryContent);

            categoryBtn.GetComponent<Button>().onClick.AddListener(() => bundleContainer.SetActive(!bundleContainer.activeSelf));
            categoryBtn.GetComponentInChildren<TMPro.TMP_Text>().text = category;

            _bundleContainers.Add(category, (categoryBtn, bundleContainer));
        }

        public string[] RemoveCategory(string category)
        {
            var bundleContainer = _bundleContainers[category];
            Destroy(bundleContainer.Item1);

            int assetBundleCount = bundleContainer.Item2.transform.childCount;
            string[] deletedAssetBundles = new string[assetBundleCount];

            Transform container = bundleContainer.Item2.transform;
            for (int i = 0; i < assetBundleCount; i++)
                deletedAssetBundles[i] = container.GetChild(i).GetComponentInChildren<TMPro.TMP_Text>().text;

            Destroy(bundleContainer.Item2);
            _bundleContainers.Remove(category);

            return deletedAssetBundles;
        }

        #endregion

        private void Awake()
        {
            // Get the new category button and add its onClick event to the NewCategory() function.
            NewCategoryPanel.transform.parent.GetChild(2).GetComponent<Button>().onClick.AddListener(NewCategory);
        }
    }
}
