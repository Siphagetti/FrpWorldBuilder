using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UserInterface.World.Building.Prefab;

public class UI_Thumbnail : MonoBehaviour
{
    private GameObject _prefab;
    [SerializeField] private RawImage _image;
    [SerializeField] private TMPro.TMP_Text _text;

    public void SetThumbnail(PrefabEntity entity)
    {
        _prefab = entity.prefab;
        _text.text = entity.prefab.name;
        _image.texture = AssetPreview.GetAssetPreview(entity.prefab);
    }
}