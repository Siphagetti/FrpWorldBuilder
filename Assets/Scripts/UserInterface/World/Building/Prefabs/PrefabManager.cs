using Prefab;
using Services;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Managers.WorldBuilding;

#if UNITY_EDITOR

using UnityEditor;

class AssetModifier
{
    [MenuItem("Modify Assets/Make Textures Readable")]
    public static void MakeTexturesReadable()
    {
        /*
            If textures of a prefab is unreadable, you can use this function on editor
        */

        Texture2D[] textures = Resources.FindObjectsOfTypeAll<Texture2D>();

        foreach (Texture2D texture in textures)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer != null)
            {
                importer.isReadable = true;
                AssetDatabase.ImportAsset(path);
            }
        }
    }

    [MenuItem("Modify Assets/Make Meshes Readable")]
    public static void MakeMeshesReadable()
    {
        /*
            If model of a prefab is unreadable, you can use this function on editor
        */

        MeshFilter[] meshFilters = Resources.FindObjectsOfTypeAll<MeshFilter>();

        foreach (MeshFilter meshFilter in meshFilters)
        {
            Mesh mesh = meshFilter.sharedMesh;

            if (mesh != null)
            {
                string path = AssetDatabase.GetAssetPath(mesh);
                ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;

                if (importer != null)
                {
                    importer.isReadable = true;
                    AssetDatabase.ImportAsset(path);
                }
            }
        }
    }
}

#endif


namespace UserInterface.World.Building.Prefab
{
    internal class PrefabManager : MonoBehaviour
    {
        #region GameObject

        public static GameObject SpawnPrefab(GameObject prefab)
        {
            /*
                 Instantiate given prefab and creates mesh collider for it.
            */
            var spawnPos = Camera.main.transform.position + Camera.main.transform.forward * DragManager.CamDist;
            var spawnedPrefab = Instantiate(prefab, spawnPos, Quaternion.identity);
            CreateMeshCollider(spawnedPrefab);
            ScaleToVolume(spawnedPrefab);
            spawnedPrefab.layer = LayerMask.NameToLayer("Draggable");
            return spawnedPrefab;
        }

        private static void CreateMeshCollider(GameObject spawnedPrefab)
        {
            // If root has mesh directly creates a mesh collider and sets its mesh as owned mesh
            if (spawnedPrefab.GetComponent<MeshFilter>())
            {
                MeshCollider meshCollider = spawnedPrefab.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = spawnedPrefab.GetComponent<MeshFilter>().sharedMesh;
            }

            // Else get meshes in the children then combine them for mesh collider
            else
            {
                MeshFilter[] meshFilters = spawnedPrefab.GetComponentsInChildren<MeshFilter>();
                CombineInstance[] combine = new CombineInstance[meshFilters.Length];

                List<Material> materials = new List<Material>(); // Store materials of the sub-meshes

                for (int i = 0; i < meshFilters.Length; i++)
                {
                    MeshFilter meshFilter = meshFilters[i];

                    if (meshFilter.sharedMesh.isReadable)
                    {
                        combine[i].mesh = meshFilter.sharedMesh;
                        combine[i].transform = meshFilter.transform.localToWorldMatrix;
                        Destroy(meshFilter.gameObject); // Disable child objects

                        // Store the material of the sub-mesh
                        Renderer renderer = meshFilter.GetComponent<Renderer>();
                        if (renderer != null && renderer.material != null) materials.Add(renderer.material);
                    }
                    else Debug.LogWarning("Mesh is not readable: " + meshFilter.name);
                }

                Mesh combinedMesh = new Mesh();
                combinedMesh.CombineMeshes(combine, false, true, false);

                // Create a new MeshFilter for the spawnedPrefab and assign the combined mesh
                MeshFilter newMeshFilter = spawnedPrefab.AddComponent<MeshFilter>();
                newMeshFilter.sharedMesh = combinedMesh;

                // Create a new MeshRenderer for the spawnedPrefab
                MeshRenderer meshRenderer = spawnedPrefab.AddComponent<MeshRenderer>();

                // Assign the materials to the new MeshRenderer
                meshRenderer.materials = materials.ToArray();

                MeshCollider meshCollider = spawnedPrefab.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = combinedMesh;
            }
        }

        private static void ScaleToVolume(GameObject spawnedPRefab)
        {
            float size = 5f;

            MeshRenderer[] meshRenderers = spawnedPRefab.GetComponentsInChildren<MeshRenderer>();

            float totalVolume = 0f;

            // Calculate the total volume of the selected model
            foreach (MeshRenderer renderer in meshRenderers)
            {
                Mesh mesh = renderer.gameObject.GetComponent<MeshFilter>().sharedMesh;
                Vector3 scale = renderer.transform.lossyScale;

                float meshVolume = mesh.bounds.size.x * mesh.bounds.size.y * mesh.bounds.size.z;
                float scaledVolume = meshVolume * scale.x * scale.y * scale.z;

                totalVolume += scaledVolume;
            }

            // Calculate the scaling factor to achieve a volume of 1
            float scaleFactor = size / totalVolume;

            // Apply the scaling factor to the selected model
            spawnedPRefab.transform.localScale *= Mathf.Pow(scaleFactor, 1f / 3f); // Cube root to maintain proportions
        }

        #endregion

        #region UI

        [Header("Category Parameters")]

        [SerializeField] private ScrollRect _categoryScrollRect;
        [SerializeField] private GameObject _prefabCategory;

        private string _activeCategory = "";

        [Header("Thumbnail Parameters")]

        // Scroll rect for image contents.
        [SerializeField] private ScrollRect _thumbnailScrollRect;

        // A prefab for UI for displaying prefabs that can be placed in the world.
        [SerializeField] private GameObject _prefabThumbnail; // It has a RawImage and a Display Name

        // A prefab for keeps created Images.
        [SerializeField] private GameObject _prefabThumbnailContent;

        // Keeps contents by their owner folder.
        private Dictionary<string, GameObject> _thumbnailContentDict = new();

        private IPrefabService _prefabService;

        public void ChangeCategory(string category)
        {
            // Check the category exists.
            if (!_thumbnailContentDict.ContainsKey(category)) { global::Log.Logger.Log_Error("category_not_exists", category); return; }

            // Disappear previous active thumbnail content.
            _thumbnailContentDict[_activeCategory].SetActive(false);

            // Update active category
            _thumbnailContentDict[category].SetActive(true);
            _activeCategory = category;

            // Set scroll rect's content as current category content
            _thumbnailScrollRect.content = _thumbnailContentDict[category].GetComponent<RectTransform>();
        }

        private void CreateCategory(string category)
        {
            var category_btn = Instantiate(_prefabCategory, _categoryScrollRect.content);
            category_btn.GetComponentInChildren<TMPro.TMP_Text>().text = category;
            category_btn.GetComponent<Button>().onClick.AddListener(() => ChangeCategory(category));
        }


        private IEnumerator CreateContent(string category)
        {
            /*
                Creates a content that keeps thumbnails of 3D assets for the category.
            */

            GameObject newContent = Instantiate(_prefabThumbnailContent, _thumbnailScrollRect.viewport);
            newContent.SetActive(false);

            _thumbnailContentDict.Add(category, newContent);

            // Get prefabs those will be shown in the new created content.
            var prefabEntites = _prefabService.GetPrefabEntitiesInFolder(category);
            if (prefabEntites == null) yield break;

            foreach (var prefabEntity in prefabEntites)
            {
                GameObject thumbnail = Instantiate(_prefabThumbnail, newContent.transform);
                thumbnail.GetComponent<UI_Thumbnail>().SetThumbnail(prefabEntity);
            }
        }

        #endregion


        private IEnumerator Initialize()
        {
            var categories = _prefabService.GetCategories();

            List<IEnumerator> coroutines = new();

            foreach (var category in categories)
            {
                CreateCategory(category);
                coroutines.Add(CreateContent(category));
            }

            if(coroutines.Count == 0) yield break;

            // Begin initialize the first content.
            yield return StartCoroutine(coroutines[0]);

            // Set the initial content as the first content that initialized
            _activeCategory = _thumbnailContentDict.Keys.ToList()[0];
            ChangeCategory(_activeCategory);

            // Initialize the rest of the contents
            for (int i = 1; i < coroutines.Count; i++)
                yield return StartCoroutine(coroutines[i]);
            
        }

        private void Start()
        {
            _prefabService = ServiceManager.GetService<IPrefabService>();
            UI_Thumbnail.OwnerUIPanelRect = GetComponent<RectTransform>();

            StartCoroutine(Initialize());
        }
    }
}
