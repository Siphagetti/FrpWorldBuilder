using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.World.Building
{
    internal class UI_PrefabPreviewController : MonoBehaviour
    {
        void Start()
        {
            // Create a new camera and set its properties
            _dynamicCamera = new GameObject("Prefab Viewer Camera").AddComponent<Camera>();
            _dynamicCamera.fieldOfView = 60;
            _dynamicCamera.aspect = 1;
            _dynamicCamera.clearFlags = CameraClearFlags.SolidColor;
            _dynamicCamera.backgroundColor = Color.black;
            _dynamicCamera.enabled = false;

            // Set the position of the camera at some distance from the prefab
            _dynamicCamera.transform.position = new Vector3(0f, 0f, -10f); // Example: -10 units along the Z-axis
            _dynamicCamera.transform.parent = transform;

            // Temporary
            StartCoroutine(CreatePrefabPreviews(new List<string>
            {
                "chair",
                "TestMesh",
                "Tree/Tree"
            }));
        }

        IEnumerator CreatePrefabPreviews(List<string> prefabPaths)
        {
            foreach (string prefabPath in prefabPaths)
                yield return CreatePrefabPreview(prefabPath);

            Destroy(gameObject);
        }

        private IEnumerator CreatePrefabPreview(string prefabPath)
        {
            GameObject prefab = Resources.Load<GameObject>(prefabPath);
            GameObject prefabPreview = Instantiate(_prefabPreviewComp);
            prefabPreview.transform.parent = _canvas.transform;
            prefabPreview.transform.localPosition = Vector3.zero;

            prefabPreview.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text = Path.GetFileName(prefabPath);

            yield return StartCoroutine(PrepareImage(prefab, prefabPreview.transform.GetChild(0).GetComponent<RawImage>()));
        }

        // Displays prefab on the incoming rawimage
        private IEnumerator PrepareImage(GameObject prefab, RawImage image)
        {
            if (prefab != null)
            {
                // Instantiate the prefab
                GameObject instantiatedPrefab = Instantiate(prefab);

                // Calculate the desired size
                float distanceToCamera = Vector3.Distance(instantiatedPrefab.transform.position, _dynamicCamera.transform.position);
                float desiredSize = Mathf.Tan(Mathf.Deg2Rad * _dynamicCamera.fieldOfView / 2f) * distanceToCamera * 2f * _dynamicCamera.aspect;

                // Create a new RenderTexture for this prefab
                RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
                _dynamicCamera.targetTexture = renderTexture;

                image.texture = renderTexture;

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
                Vector3 paddingOffset = _dynamicCamera.transform.forward * padding;
                instantiatedPrefab.transform.position += paddingOffset;

                // Render the camera manually into the render texture
                _dynamicCamera.Render();

                // Explicitly destroy the instantiated prefab after rendering is complete
                Destroy(instantiatedPrefab);

                // Wait for the next frame
                yield return null;

                // Clear the camera's target texture
                _dynamicCamera.targetTexture = null;
            }
            else
            {
                Debug.LogError("Prefab not found in Resources folder.");
            }
        }

        // ---------------------------------- Helpers ----------------------------------

        // Implement your function to calculate the original size of the prefab
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

        // A prefab for UI that purpose display prefabs that can be placed in the world.
        // It has a RawImage and a Display Name
        [SerializeField] private GameObject _prefabPreviewComp;
        [SerializeField] private GameObject _canvas;
        private Camera _dynamicCamera;
    }
}
