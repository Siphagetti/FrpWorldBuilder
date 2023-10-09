using Asset;
using Services;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.World.Building.Asset
{
    internal class UI_AssetManager : MonoBehaviour
    {
        #region Category

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

        public void CreateNewCategory(string categoryName)
        {
            // Try to create new folder for the new category.
            bool isFolderCreateSuccess = _assetService.CreateCategoryFolder(categoryName);
            if (!isFolderCreateSuccess) { global::Log.Logger.Log_Error("category_exists", categoryName); return; }

            CreateCategory(categoryName);

            // Create new content for the new category.
            GameObject newContent = Instantiate(_prefabThumbnailContent, _thumbnailViewport);

            // Keep the new created content in the '_thumbnailContentDict'
            _thumbnailContentDict.Add(categoryName, newContent);
        }

        private void CreateCategory(string category)
        {
            var category_btn = Instantiate(_prefabCategory, _categoryContent);
            category_btn.GetComponentInChildren<TMPro.TMP_Text>().text = category;
            category_btn.GetComponent<Button>().onClick.AddListener(() => ChangeCategory(category));
        }

        [Header("Category Parameters")]

        [SerializeField] private Transform _categoryContent;
        [SerializeField] private GameObject _prefabCategory;

        private string _activeCategory = "";

        #endregion


        #region Thumbnail

        #region Content

        public void AddAssetToContent(string category)
        {
            bool isImported = _assetService.ImportFolder(category);

            if (!isImported) return;

            // Get prefabs those will be shown in the new created content.
            var prefabs = _assetService.GetPrefabsInFolder(category);

            // Get category content
            var categoryContent = _thumbnailContentDict[category];

            // Create Images of the prefabs.
            StartCoroutine(CreateImages(prefabs, categoryContent.transform));
        }

        private Task CreateContent(string category)
        {
            /*
                Creates a content that keeps thumbnails of 3D assets for the category.
            */

            GameObject newContent = Instantiate(_prefabThumbnailContent, _thumbnailViewport);
            newContent.SetActive(false);

            _thumbnailContentDict.Add(category, newContent);

            // Get prefabs those will be shown in the new created content.
            var prefabs = _assetService.GetPrefabsInFolder(category);
            if (prefabs == null) return Task.CompletedTask;

            // Create Images of the prefabs.
            StartCoroutine(CreateImages(prefabs, newContent.transform));
            return Task.CompletedTask;
        }

        #endregion

        #region Image

        // Creates prefabs' images. You need to create an empty List<GameObject> and send it as images to get images.
        private IEnumerator CreateImages(List<GameObject> prefabs, Transform content)
        {
            if (prefabs != null)
            {
                Camera camera = CreateCamera();

                foreach (var prefab in prefabs)
                {
                    // Instantiate the prefab
                    GameObject instantiatedPrefab = Instantiate(prefab);

                    GameObject image = Instantiate(_prefabThumbnail);
                    image.transform.SetParent(content);
                    image.transform.localScale = Vector3.one;

                    // Calculate the desired size for prefab
                    float distanceToCamera = Vector3.Distance(instantiatedPrefab.transform.position, camera.transform.position);
                    float desiredSize = Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView / 2f) * distanceToCamera * 2f * camera.aspect;

                    // Create a new RenderTexture for this prefab
                    RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
                    camera.targetTexture = renderTexture;

                    // Set image's texture as render texture
                    image.GetComponentInChildren<RawImage>().texture = renderTexture;

                    // Calculate the original size of the prefab (you may need a custom function)
                    float originalSizeOfPrefab = CalculateOriginalSize(prefab);

                    // Calculate the scale factor
                    float scaleFactor = desiredSize / originalSizeOfPrefab;

                    // Apply the scale to the prefab
                    instantiatedPrefab.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

                    // Calculate the offset due to the pivot and adjust the position
                    Vector3 pivotOffset = CalculatePivotOffset(prefab, scaleFactor);
                    instantiatedPrefab.transform.position -= pivotOffset;

                    // Add padding around the prefab
                    float padding = 0.3f; // Adjust the padding value as needed
                    Vector3 paddingOffset = camera.transform.forward * padding;
                    instantiatedPrefab.transform.position += paddingOffset;

                    // Render the camera manually into the render texture
                    camera.Render();

                    // Explicitly destroy the instantiated prefab after rendering is complete
                    Destroy(instantiatedPrefab);

                    // Wait for the next frame
                    yield return null;

                    // Clear the camera's target texture
                    camera.targetTexture = null;
                }

                Destroy(camera.gameObject);
            }
            else
            {
                Debug.LogError("Prefab not found in Resources folder.");
            }
        }

        private Camera CreateCamera()
        {
            // Create a new camera and set its properties
            Camera camera = new GameObject("Prefab Viewer Camera").AddComponent<Camera>();
            camera.fieldOfView = 60;
            camera.aspect = 1;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
            camera.enabled = false;

            // Set the position of the camera at some distance from the prefab
            camera.transform.position = new Vector3(0f, 0f, -10f); // Example: -10 units along the Z-axis
            camera.transform.parent = transform;

            return camera;
        }


        private float CalculateOriginalSize(GameObject prefab)
        {
            // Find the mesh renderer in the prefab
            MeshRenderer meshRenderer = prefab.GetComponentInChildren<MeshRenderer>();

            if (meshRenderer != null)
            {
                // Calculate the size based on the mesh bounds
                Bounds bounds = meshRenderer.bounds;
                float originalSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
                return originalSize;
            }
            else
            {
                // If no mesh renderer is found, return a default size
                return 1.0f;
            }
        }

        // Calculate the offset due to the pivot
        private Vector3 CalculatePivotOffset(GameObject prefab, float scaleFactor)
        {
            // Find the mesh renderer in the prefab
            MeshRenderer meshRenderer = prefab.GetComponentInChildren<MeshRenderer>();

            if (meshRenderer != null)
            {
                // Calculate the pivot offset based on the bounds center and scale
                Bounds bounds = meshRenderer.bounds;
                Vector3 pivotOffset = bounds.center - prefab.transform.position;
                pivotOffset *= scaleFactor; // Apply scale factor
                return pivotOffset;
            }
            else
            {
                // If no mesh renderer is found, assume pivot is at the center
                return Vector3.zero;
            }
        }

        #endregion


        #region Thumbnail Params

        [Header("Thumbnail Parameters")]
        
        // Scroll rect for image contents.
        [SerializeField] private ScrollRect _thumbnailScrollRect;

        // Content container viewport.
        [SerializeField] private Transform _thumbnailViewport;

        // A prefab for UI for displaying prefabs that can be placed in the world.
        [SerializeField] private GameObject _prefabThumbnail; // It has a RawImage and a Display Name

        // A prefab for keeps created Images.
        [SerializeField] private GameObject _prefabThumbnailContent;

        // Keeps contents by their owner folder.
        private Dictionary<string, GameObject> _thumbnailContentDict = new();

        #endregion

        #endregion

        private IAseetService _assetService;

        private async void Initialize()
        {
            var categories = _assetService.GetAllCategories();
            IEnumerable<Task> tasks = new List<Task>();

            foreach (var category in categories)
            {
                CreateCategory(category);
                ((List<Task>)tasks).Add(CreateContent(category));
            }

            await Task.WhenAll(tasks);
        }

        private void Start()
        {
            _assetService = ServiceManager.GetService<IAseetService>();
            Initialize();

            if (_thumbnailContentDict.Count == 0) return;

            _activeCategory = _thumbnailContentDict.Keys.ToList()[0];

            ChangeCategory(_activeCategory);
        }
    }
}
