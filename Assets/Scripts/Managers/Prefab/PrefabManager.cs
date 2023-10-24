using Services;
using System.Collections;
using UnityEngine;


namespace Prefab
{
    internal class PrefabManager : MonoBehaviour
    {
        private CategoryComponent _categoryComponent;
        private ThumbnailsComponent _thumbnailsComponent;

        public Shader rimShader;

        private void Awake()
        {
            _categoryComponent = GetComponent<CategoryComponent>();
            _thumbnailsComponent = GetComponent<ThumbnailsComponent>();

            Initialize();
        }

        private void Initialize()
        {
            IPrefabService prefabService = ServiceManager.GetService<IPrefabService>();
            var categories = prefabService.GetCategories();

            foreach (var category in categories) 
            {
                _categoryComponent.CreateCategory(category);
                var assetBundles = prefabService.GetRepository().GetAssetBundles(category);
                foreach (var assetBundle in assetBundles)
                {
                    var thumbnailContainer = _thumbnailsComponent.CreateContent(assetBundle.Key, assetBundle.Value);
                    _categoryComponent.CreateAssetBundleButton(category, assetBundle.Key, thumbnailContainer);
                }
            }

            _thumbnailsComponent.FillContents();
        }

        public async void ImportAssetBundle(string category)
        {
            IPrefabService prefabService = ServiceManager.GetService<IPrefabService>();
            var response = await prefabService.ImportAssetBundle(category);

            if (response.Success)
            {
                var (bundleName, prefabs) = response.Result;
                var thumbnailContainer = _thumbnailsComponent.CreateContent(bundleName, prefabs);
                _thumbnailsComponent.FillContents();
                var assetBundleContainer = _categoryComponent.CreateAssetBundleButton(category, bundleName, thumbnailContainer);
            }
        }

        public void DeleteCategory(string category)
        {
            GameManager.NewCoroutine(DeleteCategoryCoroutine(category));
        }

        private IEnumerator DeleteCategoryCoroutine(string category)
        {
            yield return GameManager.NewCoroutine(PopupController.Instance.InstantiatePopup("delete_category", category));

            var enumerator = PopupController.Instance.InstantiatePopup("delete_category", category);
            bool result = false;

            while (enumerator.MoveNext())
            {
                if (enumerator.Current is bool)
                {   
                    result = (bool)enumerator.Current;
                    break;
                }
            }

            if (result == false) yield break;

            IPrefabService prefabService = ServiceManager.GetService<IPrefabService>();
            prefabService.DeleteCategory(category);
            var assetBundles = _categoryComponent.RemoveCategory(category);
            _thumbnailsComponent.DeleteContents(assetBundles);
        }

        public void DeleteAssetBundle(string category, string bundleName)
        {
            GameManager.NewCoroutine(DeleteAssetBundleCoroutine(category, bundleName));
        }

        private IEnumerator DeleteAssetBundleCoroutine(string category, string bundleName)
        {
            yield return GameManager.NewCoroutine(PopupController.Instance.InstantiatePopup("delete_asset_bundle", bundleName));

            var enumerator = PopupController.Instance.InstantiatePopup("delete_asset_bundle", bundleName);
            bool result = false;

            while (enumerator.MoveNext())
            {
                if (enumerator.Current is bool)
                {
                    result = (bool)enumerator.Current;
                    break;
                }
            }

            if (result == false) yield break;

            IPrefabService prefabService = ServiceManager.GetService<IPrefabService>();
            prefabService.DeleteAssetBundle(category, bundleName);
            _categoryComponent.RemoveAssetBundle(category, bundleName);
            _thumbnailsComponent.DeleteContents(new string[] { bundleName });
        }
    }
}
