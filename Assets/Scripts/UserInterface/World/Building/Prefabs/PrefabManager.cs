﻿using Prefab;
using Services;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


namespace Prefab
{
    internal class PrefabManager : MonoBehaviour
    {
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

        private void Start()
        {
            Thumbnail.OwnerUIPanelRect = GetComponent<RectTransform>();
            StartCoroutine(Initialize());
        }

        private IEnumerator Initialize()
        {
            var categories = ServiceManager.GetService<IPrefabService>().GetCategories();

            List<IEnumerator> coroutines = new();

            foreach (var category in categories)
            {
                CreateCategory(category);
                coroutines.Add(CreateContent(category));
            }

            if (coroutines.Count == 0) yield break;

            // Begin initialize the first content.
            yield return StartCoroutine(coroutines[0]);

            // Set the initial content as the first content that initialized
            _activeCategory = _thumbnailContentDict.Keys.ToList()[0];
            ChangeCategory(_activeCategory);

            // Initialize the rest of the contents
            for (int i = 1; i < coroutines.Count; i++)
                yield return StartCoroutine(coroutines[i]);

        }


        private void ChangeCategory(string category)
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
            var loadPrefabsTask = ServiceManager.GetService<IPrefabService>().LoadPrefabsInAssetBundlesForCategory(category);

            yield return new WaitUntil(() => loadPrefabsTask.IsCompleted);

            var prefabEntites = loadPrefabsTask.Result;
            if (prefabEntites == null) yield break;

            StartCoroutine(CreateThumbnails(prefabEntites, newContent.transform));
        }

        private IEnumerator CreateThumbnails(List<GameObject> prefabs, Transform content)
        {
            if (prefabs != null)
            {
                Camera camera = CreateCamera();

                foreach (var prefab in prefabs)
                {
                    // Instantiate the prefab
                    Vector3 prefabSpawnPos = camera.transform.position + Prefab.Size * 0.5f * (Vector3.forward + Vector3.down / 2);
                    GameObject instantiatedPrefab = Instantiate(prefab, prefabSpawnPos, Quaternion.identity);

                    GameObject thumbnail = Instantiate(_prefabThumbnail);
                    thumbnail.transform.SetParent(content);
                    thumbnail.transform.localScale = Vector3.one;

                    thumbnail.GetComponent<Thumbnail>().SetPrefab(prefab);

                    // Create a new RenderTexture for this prefab
                    RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
                    camera.targetTexture = renderTexture;

                    // Set image's texture as render texture
                    thumbnail.GetComponentInChildren<RawImage>().texture = renderTexture;

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

            Camera CreateCamera()
            {
                // Create a new camera and set its properties
                Camera camera = new GameObject("Prefab Viewer Camera").AddComponent<Camera>();
                camera.fieldOfView = 60;
                camera.aspect = 1;
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = Color.black;
                camera.enabled = false;

                // Set the position of the camera at some distance from the prefab
                camera.transform.localPosition = Vector3.one * -10000;
                camera.transform.parent = transform;

                return camera;
            }
        }

        
    }
}