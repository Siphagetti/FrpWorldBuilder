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
        // Value:   asset bundle button container
        private Dictionary<string, Transform> _bundleContainers = new();

        public void CreateCategory(string category)
        {
            var categoryBtn = Instantiate(CategoryButtonPrefab, CategoryContent);
            var bundleContainer = Instantiate(AssetBundlesButtonContainer, CategoryContent);

            categoryBtn.GetComponent<Button>().onClick.AddListener(() => CategoryButtonOnClick(bundleContainer));
            categoryBtn.GetComponentInChildren<TMPro.TMP_Text>().text = category;

            _bundleContainers.Add(category, bundleContainer.transform);
        }
        
        public void CreateAssetBundleButton(string category, string bundleName, GameObject relatedThumbnailContent) 
        {
            if (!_bundleContainers.ContainsKey(category)) return;

            var bundleBtn = Instantiate(AssetBundleButtonPrefab, _bundleContainers[category]);
            bundleBtn.GetComponent<Button>().onClick.AddListener(() => AssetBundleButtonOnClick(relatedThumbnailContent));
            bundleBtn.GetComponentInChildren<TMPro.TMP_Text>().text = bundleName;
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
