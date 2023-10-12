using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UserInterface.World.Building.Prefab;

public class UI_Thumbnail : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RawImage _image;
    [SerializeField] private TMPro.TMP_Text _text;


    private GameObject _prefab;

    Transform _parentTransform;

    private int initialSiblingIndex; // Store the initial sibling index when drag begins

    public void OnBeginDrag(PointerEventData eventData)
    {
        _parentTransform = transform.parent;

        // Store the initial sibling index
        initialSiblingIndex = transform.GetSiblingIndex();

        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
       transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(_parentTransform);

        // Set the element's sibling index back to the initial index
        transform.SetSiblingIndex(initialSiblingIndex);
    }

    public void SetThumbnail(PrefabEntity entity)
    {
        _prefab = entity.prefab;
        _text.text = entity.prefab.name;
        _image.texture = entity.prefabThumbnail;
    }
}