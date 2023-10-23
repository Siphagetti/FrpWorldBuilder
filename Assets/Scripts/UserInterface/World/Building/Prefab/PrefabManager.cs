using Services;
using UnityEngine;


namespace Prefab
{
    internal class PrefabManager : MonoBehaviour
    {
        private CategoryComponent _categoryComponent;
        private ThumbnailsComponent _thumbnailsComponent;

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
                    var thumbnailContainer = _thumbnailsComponent.CreateContent(assetBundle.Value);
                    _categoryComponent.CreateAssetBundleButton(category, assetBundle.Key, thumbnailContainer);
                }
            }

            _thumbnailsComponent.FillContents();
        }
    }
}
