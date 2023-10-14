using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UserInterface.World.Building.Prefab;

public class UI_Thumbnail : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static RectTransform OwnerUIPanelRect { get; set; }

    [SerializeField] private RawImage _image;
    [SerializeField] private TMPro.TMP_Text _text;

    private GameObject _prefab;
    private GameObject _spawnedPrefab;

    // Content that UI element belongs to.
    private Transform _parentTransform;

    // Keep sibling index to put in its beginning place 
    private int _initialSiblingIndex; 

    // Offset for drag the UI element from where its been hold.
    private Vector3 _mouseOffset;

    // Distance between camera and spawned prefab.
    private float _camDist;

    // Hide UI when cursor should drag the spawned prefab.
    private bool _hideUI;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _parentTransform = transform.parent;

        // Store the initial sibling index
        _initialSiblingIndex = transform.GetSiblingIndex();

        transform.SetParent(transform.root);

        _mouseOffset = transform.position - Input.mousePosition;

        _hideUI = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // If prefab not spawned, just move the ui element
        if (!_hideUI) transform.position = Input.mousePosition + _mouseOffset;

        // else move the spawned object with raycast.
        else SetPrefabPos();
        

        // -------------- Control UI Position --------------

        // Convert the screen space position to the owner UI panel's local position
        Vector2 localMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(OwnerUIPanelRect, Input.mousePosition, null, out localMousePosition);

        // Compare the y-coordinate to determine if the mouse is higher or lower
        float elementHeight = OwnerUIPanelRect.rect.height;

        if (localMousePosition.y > elementHeight / 2)
        {
            if (_hideUI) return;

            _spawnedPrefab = Instantiate(_prefab);
            _camDist = Vector3.Distance(Camera.main.transform.position, _spawnedPrefab.transform.position);
            SetPrefabPos();

            _image.gameObject.SetActive(false);
            _text.gameObject.SetActive(false);

            _hideUI = true;
        }
        else
        {
            if (!_hideUI) return;

            if (_spawnedPrefab != null)
            {
                Destroy(_spawnedPrefab);
                _spawnedPrefab = null;
            }

            _image.gameObject.SetActive(true);
            _text.gameObject.SetActive(true);

            _hideUI = false;
        }

        void SetPrefabPos()
        {
            // Calculate a new position for the spawned prefab based on mouse movement
            Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _camDist);
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            _spawnedPrefab.transform.position = newPosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_hideUI)
        {
            _image.gameObject.SetActive(true);
            _text.gameObject.SetActive(true);
            _spawnedPrefab?.transform.SetParent(null);
        }

        // Seperate the prefab from the UI element.
        _spawnedPrefab = null;

        // Set UI's parent as its beginning parent.
        transform.SetParent(_parentTransform);

        // Set the element's sibling index back to the initial index
        transform.SetSiblingIndex(_initialSiblingIndex);
    }

    public void SetThumbnail(PrefabEntity entity)
    {
        _prefab = entity.prefab;
        _text.text = entity.prefab.name;
        _image.texture = entity.prefabThumbnail;
    }
}