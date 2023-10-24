﻿using Language;
using Log;
using Services;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

internal class PopupController : MonoBehaviour
{
    [SerializeField] private GameObject _popupPrefab;

    private bool _isButtonClicked = false;
    private bool _isConfirmed = false;

    public static PopupController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(this); return; }
        Instance = this;
        _yPos = (int)_popupPrefab.GetComponent<RectTransform>().anchoredPosition.y;
    }

    public IEnumerator InstantiatePopup(string key, params object[] args)
    {
        yield return GameManager.NewCoroutine(GetText(key, args));
        var enumerator = GetText(key, args);

        string text = "";

        while (enumerator.MoveNext())
        {
            var result = enumerator.Current;
            if (result is string) text = result as string;
        }

        var popup = Instantiate(_popupPrefab, transform.root);
        popup.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = text;
        popup.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
        {
            _isButtonClicked = true;
            _isConfirmed = false;
        });
        popup.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() =>
        {
            _isButtonClicked = true;
            _isConfirmed = true;
        });

        RevealPopup(popup);

        yield return new WaitUntil(() =>  _isButtonClicked);
        _isButtonClicked = false;

        HidePopup(popup);

        yield return _isConfirmed;
    }

    private IEnumerator GetText(string key, params object[] args)
    {
        yield return GameManager.NewCoroutine(ServiceManager.GetService<ILanguageService>().GetLocalizedValue(key));

        // Get the result from the IEnumerator
        var enumerator = ServiceManager.GetService<ILanguageService>().GetLocalizedValue(key);
        while (enumerator.MoveNext())
        {
            var result = enumerator.Current;
            if (result is string)
            {
                var msg = result as string;
                yield return string.Format(msg, args);
            }
        }
    }

    #region Hide or Reveal Panel

    private int _yPos;
    private float _duration = 0.3f;

    Coroutine HideOrRevealCoorutine;

    private Coroutine HidePopup(GameObject popup)
    {
        if (HideOrRevealCoorutine != null)
        {
            StopCoroutine(HideOrRevealCoorutine);
            HideOrRevealCoorutine = null;
        }

        return GameManager.NewCoroutine(HidePopupCoroutine(popup));
    }

    private Coroutine RevealPopup(GameObject popup)
    {
        if (HideOrRevealCoorutine != null)
        {
            StopCoroutine(HideOrRevealCoorutine);
            HideOrRevealCoorutine = null;
        }

        return GameManager.NewCoroutine(RevealPopupCoroutine(popup));
    }

    IEnumerator HidePopupCoroutine(GameObject popup)
    {
        RectTransform rect = popup.GetComponent<RectTransform>();

        Vector2 startPos = rect.anchoredPosition;
        Vector2 targetPos = new Vector2(startPos.x, _yPos);

        for (float t = 0; t < 1.0f; t += Time.deltaTime / _duration)
        {
            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        rect.anchoredPosition = targetPos; // Ensure the final position is exactly at the target

        HideOrRevealCoorutine = null;
    }

    IEnumerator RevealPopupCoroutine(GameObject popup)
    {
        RectTransform rect = popup.GetComponent<RectTransform>();

        Vector2 startPos = rect.anchoredPosition;
        Vector2 targetPos = new Vector2(startPos.x, -_yPos);
        // Adjust the duration as needed

        for (float t = 0; t < 1.0f; t += Time.deltaTime / _duration)
        {
            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        rect.anchoredPosition = targetPos; // Ensure the final position is exactly at the target

        HideOrRevealCoorutine = null;
    }

    #endregion
}

