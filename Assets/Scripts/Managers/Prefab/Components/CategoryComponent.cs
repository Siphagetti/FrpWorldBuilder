using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Prefab
{
    internal class CategoryComponent : MonoBehaviour
    {
        // Keeps all category elements.
        [SerializeField] private Transform CategoryContent;

        // Toggles related asset bundles button container.
        [SerializeField] private GameObject CategoryButtonPrefab;

        // Keeps asset bundles' buttons
        [SerializeField] private GameObject AssetBundlesButtonContainer;

        // Apeears asset bundle's prefabs in ui container.
        [SerializeField] private GameObject AssetBundleButtonPrefab;

        // Key:     category
        // Value:   Category button and asset bundle button container
        private Dictionary<string, (GameObject, GameObject)> _bundleContainers = new();

        public void CreateCategory(string category)
        {
            var categoryBtn = Instantiate(CategoryButtonPrefab, CategoryContent);
            var bundleContainer = Instantiate(AssetBundlesButtonContainer, CategoryContent);

            categoryBtn.GetComponent<Button>().onClick.AddListener(() => CategoryButtonOnClick(bundleContainer));
            categoryBtn.GetComponentInChildren<TMPro.TMP_Text>().text = category;

            _bundleContainers.Add(category, (categoryBtn, bundleContainer));
        }
        
        public GameObject CreateAssetBundleButton(string category, string bundleName, GameObject relatedThumbnailContent) 
        {
            if (!_bundleContainers.ContainsKey(category)) return null;

            var bundleBtn = Instantiate(AssetBundleButtonPrefab, _bundleContainers[category].Item2.transform);
            bundleBtn.GetComponent<Button>().onClick.AddListener(() => AssetBundleButtonOnClick(relatedThumbnailContent));
            bundleBtn.GetComponentInChildren<TMPro.TMP_Text>().text = bundleName;

            return _bundleContainers[category].Item2;
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

        public GameObject RemoveAssetBundle(string category, string bundleName)
        {
            Transform container = _bundleContainers[category].Item2.transform;

            for (int i = 0; i < container.childCount; i++)
            {
                var assetBundle = container.GetChild(i);

                if (assetBundle.GetComponentInChildren<TMPro.TMP_Text>().text == bundleName)
                {
                    Destroy(assetBundle.gameObject);
                    return container.gameObject;
                }
            }

            return null;
        }

        private void CategoryButtonOnClick(GameObject assetBundleContainer)
        {
            assetBundleContainer.SetActive(!assetBundleContainer.activeSelf);
        }

        private void AssetBundleButtonOnClick(GameObject thumbnailContent)
        {
            GetComponent<ThumbnailsComponent>().ChangeCurrentThumbnailContent(thumbnailContent);
        }
        
    }
}
