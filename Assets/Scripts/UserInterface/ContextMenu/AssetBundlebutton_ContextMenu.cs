using Prefab;
using UnityEngine.UI;

namespace ContextMenu
{
    internal class AssetBundlebutton_ContextMenu : ContextMenu
    {
        private void Start()
        {
            var prefabManager = FindFirstObjectByType<PrefabManager>();
            string bundleName = _owner.GetComponentInChildren<TMPro.TMP_Text>().text;

            // As there is no direct access to the category information for asset bundle buttons, 
            // accessing the previous element in the asset bundle container of the context menu's owner is required.
            int siblingIndex = _owner.transform.parent.GetSiblingIndex();
            var category = _owner.transform.parent.parent.GetChild(siblingIndex - 1).GetComponentInChildren<TMPro.TMP_Text>().text;

            var deleteButton = transform.GetChild(0).GetComponent<Button>();
            deleteButton.onClick.AddListener(() => prefabManager.DeleteAssetBundle(category, bundleName));
            deleteButton.onClick.AddListener(() => Destroy(gameObject));
        }
    }
}
