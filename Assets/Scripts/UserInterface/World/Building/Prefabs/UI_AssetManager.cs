using Prefab;
using Services;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.World.Building.Prefab
{
    internal class UI_AssetManager : MonoBehaviour
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

        private IEnumerator Initialize()
        {
            var categories = _prefabService.GetAllCategories();

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

            StartCoroutine(Initialize());
        }
    }
}
