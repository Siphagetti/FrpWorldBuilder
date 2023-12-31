﻿using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Prefab
{
    internal class ThumbnailsComponent : MonoBehaviour
    {
        private GameObject _currentThumbnailContent;

        [SerializeField] private ScrollRect _thumbnailScroller; // Keeps all thumbnail containers in its view child
        [SerializeField] private GameObject _thumbnailContentPrefab; // Keeps all thumbnails of an asset bundle
        [SerializeField] private GameObject _prefabThumbnail; // A thumbnail for a prefab in an asset bundle.
        [SerializeField] private GameObject _thumbnailPhotoShoot; // A temporary game object for taking an image of the prefab for the thumbnail.

        private Queue<IEnumerator> _createThumbnails = new();

        public GameObject CreateContent(string bundleName, Prefab[] prefabs)
        {
            GameObject content = Instantiate(_thumbnailContentPrefab, _thumbnailScroller.viewport);
            content.name = bundleName + " Content";
            if (_currentThumbnailContent == null) ChangeCurrentThumbnailContent(content);
            _createThumbnails.Enqueue(CreateThumbnails(prefabs, content.transform));
            return content;
        }

        public void FillContents()
        {
            GameManager.NewCoroutine(RunCreateThumbnailsCoroutines());

            IEnumerator RunCreateThumbnailsCoroutines()
            {
                while (_createThumbnails.Count > 0)
                    yield return GameManager.NewCoroutine(_createThumbnails.Dequeue());
            }
        }

        private IEnumerator CreateThumbnails(Prefab[] prefabs, Transform content)
        {
            if (prefabs != null)
            {
                var photoShoot = Instantiate(_thumbnailPhotoShoot);
                Camera camera = photoShoot.transform.GetChild(0).GetComponent<Camera>();
                camera.aspect = 1;
                Transform prefabLocation = photoShoot.transform.GetChild(1);

                foreach (var prefab in prefabs)
                {
                    GameObject instantiatedPrefab = Instantiate(prefab.gameObject, prefabLocation);

                    GameObject thumbnail = Instantiate(_prefabThumbnail);
                    thumbnail.transform.SetParent(content);
                    thumbnail.transform.localScale = Vector3.one;
                    thumbnail.GetComponent<Thumbnail>().SetPrefab(prefab.gameObject);

                    RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
                    camera.targetTexture = renderTexture;
                    thumbnail.GetComponentInChildren<RawImage>().texture = renderTexture;

                    yield return null;
                    camera.Render();
                    yield return null;
                    Destroy(instantiatedPrefab);
                    camera.targetTexture = null;
                }

                Destroy(photoShoot);
            }
        }

        public void ChangeCurrentThumbnailContent(GameObject thumbnailContent)
        {
            if (_currentThumbnailContent != null && !_currentThumbnailContent.IsDestroyed())
                _currentThumbnailContent.SetActive(false);

            thumbnailContent.SetActive(true);
            _currentThumbnailContent = thumbnailContent;
            _thumbnailScroller.content = _currentThumbnailContent.GetComponent<RectTransform>();
        }

        public void DeleteContents(string[] assetBundles)
        {
            Transform viewportTransform = _thumbnailScroller.viewport;

            for (int i = 0; i < viewportTransform.childCount; i++)
            {
                GameObject content = viewportTransform.GetChild(i).gameObject;

                for (int j = 0; j < assetBundles.Length; j++)
                {
                    if (content.name.Replace(" Content", "") == assetBundles[j])
                    {
                        Destroy(content);
                        break;
                    }
                }
            }
        }

        private void Awake()
        {
            Thumbnail.OwnerUIPanelRect = GetComponent<RectTransform>();
        }
    }
}
