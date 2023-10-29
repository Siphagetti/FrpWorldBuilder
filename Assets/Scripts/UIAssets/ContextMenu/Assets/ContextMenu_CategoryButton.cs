using Prefab;
using UnityEngine.UI;

namespace ContextMenu
{
    internal class ContextMenu_CategoryButton : ContextMenu
    {
        private void Start()
        {
            var prefabManager = FindFirstObjectByType<PrefabManager>();
            string category = _owner.GetComponentInChildren<TMPro.TMP_Text>().text;

            var importButton = transform.GetChild(0).GetComponent<Button>();
            importButton.onClick.AddListener(() => prefabManager.ImportAssetBundle(category));
            importButton.onClick.AddListener(() => Destroy(gameObject));

            var deleteButton = transform.GetChild(1).GetComponent<Button>();
            deleteButton.onClick.AddListener(() => prefabManager.DeleteCategory(category));
            deleteButton.onClick.AddListener(() => Destroy(gameObject));
        }
    }
}
