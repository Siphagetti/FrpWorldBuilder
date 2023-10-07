using Asset;
using Services;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.World.Building.Asset
{
    internal class UI_AssetManager : MonoBehaviour
    {
        #region Category

        public void CreateNewCategory(string categoryName)
        {
            bool isFolderCreateSuccess = _assetService.CreateCategoryFolder(categoryName);

            if (!isFolderCreateSuccess) return;
            
            CreateContent(categoryName);
        }


        [SerializeField] private string _prefabCategory;

        #endregion


        #region Asset Display

        #region Content

        private async void CreateContents()
        {
            var categoryFolderPaths = _assetService.GetAllCategoryFolderPaths();

            IEnumerable<Task> tasks = new List<Task>();

            foreach (var path in categoryFolderPaths) ((List<Task>)tasks).Add(CreateContent(path));

            await Task.WhenAll(tasks);
        }

        private Task CreateContent(string ownerFolder)
        {
            GameObject newContent = Instantiate(_prefabContent, _assetDisplayViewPort.transform);
            _assetDisplayContents.Add(ownerFolder, newContent);

            // Get prefabs those will be shown in the new created content.
            var prefabs = _assetService.GetPrefabsInFolder(ownerFolder);

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

                    GameObject image = Instantiate(_prefabImage);
                    image.transform.parent = content;

                    // Calculate the desired size
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


        #region Asset Display Params

        // Scroll rect for image contents.
        [SerializeField] private ScrollRect _assetPreviewScrollRect;

        // Content container viewport.
        [SerializeField] private GameObject _assetDisplayViewPort;

        // A prefab for UI for displaying prefabs that can be placed in the world.
        [SerializeField] private GameObject _prefabImage; // It has a RawImage and a Display Name

        // A prefab for keeps created Images.
        [SerializeField] private GameObject _prefabContent;

        // Keeps contents by their owner folder.
        private Dictionary<string, GameObject> _assetDisplayContents = new();

        private string _lastActiveContent = "";

        #endregion

        #endregion

        private IAseetService _assetService;

        private void Start()
        {
            _assetService = ServiceManager.GetService<IAseetService>();

            CreateContents();
        }
    }
}
