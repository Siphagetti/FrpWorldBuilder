using Services;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


namespace Prefab
{
    internal class PrefabManager : MonoBehaviour
    {

        [SerializeField] Button _importButton;

        [Header("Category Parameters")]

        [SerializeField] private ScrollRect _categoryScrollRect;
        [SerializeField] private GameObject _prefabCategory;

        private string _activeCategory = "";

        [Header("Thumbnail Parameters")]

        [SerializeField] private GameObject ThumbnailPhotoShoot;

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
            GameManager.NewCoroutine(Initialize());
            _importButton.onClick.AddListener(ImportAssetBundle);
        }

        private IEnumerator Initialize()
        {
            var categories = ServiceManager.GetService<IPrefabService>().GetCategories();

            List<IEnumerator> coroutines = new List<IEnumerator>();

            foreach (var category in categories)
            {
                CreateCategory(category);
                coroutines.Add(CreateContent(category));
            }

            if (coroutines.Count == 0) yield break;

            // Begin initializing the first content.
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
            if (!_thumbnailContentDict.ContainsKey(category))
            {
                Log.Logger.Log_Error("category_not_exists", category);
                return;
            }

            // Hide the previously active thumbnail content.
            _thumbnailContentDict[_activeCategory].SetActive(false);

            // Update active category
            _thumbnailContentDict[category].SetActive(true);
            _activeCategory = category;

            // Set scroll rect's content as the current category content
            _thumbnailScrollRect.content = _thumbnailContentDict[category].GetComponent<RectTransform>();
        }

        private void CreateCategory(string category)
        {
            var categoryBtn = Instantiate(_prefabCategory, _categoryScrollRect.content);
            categoryBtn.GetComponentInChildren<TMPro.TMP_Text>().text = category;
            categoryBtn.GetComponent<Button>().onClick.AddListener(() => ChangeCategory(category));
        }

        private IEnumerator CreateContent(string category)
        {
            GameObject newContent = Instantiate(_prefabThumbnailContent, _thumbnailScrollRect.viewport);
            newContent.SetActive(false);

            _thumbnailContentDict.Add(category, newContent);

            var loadPrefabsTask = ServiceManager.GetService<IPrefabService>().LoadPrefabsInAssetBundlesForCategory(category);

            yield return new WaitUntil(() => loadPrefabsTask.IsCompleted);

            var prefabEntities = loadPrefabsTask.Result;
            if (prefabEntities == null) yield break;

            yield return CreateThumbnails(prefabEntities, newContent.transform);
        }

        private IEnumerator CreateThumbnails(GameObject[] prefabs, Transform content)
        {
            if (prefabs != null)
            {
                var photoShoot = Instantiate(ThumbnailPhotoShoot);
                Camera camera = photoShoot.transform.GetChild(0).GetComponent<Camera>();
                camera.aspect = 1;
                Transform prefabLocation = photoShoot.transform.GetChild(1);

                foreach (var prefab in prefabs)
                {
                    GameObject instantiatedPrefab = Instantiate(prefab, prefabLocation);

                    GameObject thumbnail = Instantiate(_prefabThumbnail);
                    thumbnail.transform.SetParent(content);
                    thumbnail.transform.localScale = Vector3.one;
                    thumbnail.GetComponent<Thumbnail>().SetPrefab(prefab);

                    RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
                    camera.targetTexture = renderTexture;
                    thumbnail.GetComponentInChildren<RawImage>().texture = renderTexture;
                    camera.Render();
                    yield return null;
                    Destroy(instantiatedPrefab);
                    camera.targetTexture = null;
                    yield return null;
                }

                Destroy(photoShoot);
            }
            else
            {
                Debug.LogError("Prefab not found in Resources folder.");
            }
        }

        private async void ImportAssetBundle()
        {
            var prefabsInBundle = await ServiceManager.GetService<IPrefabService>().ImportAssetBundle(_activeCategory);
            if (prefabsInBundle == null) return;

            GameManager.NewCoroutine(CreateThumbnails(prefabsInBundle, _thumbnailContentDict[_activeCategory].transform));
        }
    }
}
