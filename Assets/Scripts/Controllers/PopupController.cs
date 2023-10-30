using Language;
using Services;
using System.Collections;
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
        // Ensure there is only one instance of the PopupController
        if (Instance != null) { Destroy(this); return; }
        Instance = this;

        // Cache the Y position of the popup prefab
        _yPos = (int)_popupPrefab.GetComponent<RectTransform>().anchoredPosition.y;
    }

    // Show a popup for user confirmation
    public IEnumerator GetApproval(System.Action<bool> resultCallback, string key, params object[] args)
    {
        // Get the localized text to display in the popup
        string text = ServiceManager.GetService<ILanguageService>().GetLocalizedText(key, args);

        // Instantiate the popup prefab
        var popup = Instantiate(_popupPrefab, transform.root);
        // Set the text of the popup
        popup.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = text;
        // Add click listeners for confirmation and cancellation buttons
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

        // Reveal the popup
        RevealPopup(popup);

        // Wait until the user clicks a button
        yield return new WaitUntil(() => _isButtonClicked);
        _isButtonClicked = false;

        // Hide the popup
        HidePopup(popup);

        // Invoke the result callback with the confirmation status
        resultCallback(_isConfirmed);
    }

    #region Hide or Reveal Panel

    private int _yPos;
    private float _duration = 0.3f;

    Coroutine HideOrRevealCoroutine;

    // Hide the popup using a coroutine
    private Coroutine HidePopup(GameObject popup)
    {
        if (HideOrRevealCoroutine != null)
        {
            StopCoroutine(HideOrRevealCoroutine);
            HideOrRevealCoroutine = null;
        }

        return GameManager.NewCoroutine(HidePopupCoroutine(popup));
    }

    // Reveal the popup using a coroutine
    private Coroutine RevealPopup(GameObject popup)
    {
        if (HideOrRevealCoroutine != null)
        {
            StopCoroutine(HideOrRevealCoroutine);
            HideOrRevealCoroutine = null;
        }

        return GameManager.NewCoroutine(RevealPopupCoroutine(popup));
    }

    // Coroutine to hide the popup
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

        HideOrRevealCoroutine = null;
    }

    // Coroutine to reveal the popup
    IEnumerator RevealPopupCoroutine(GameObject popup)
    {
        RectTransform rect = popup.GetComponent<RectTransform>();

        Vector2 startPos = rect.anchoredPosition;
        Vector2 targetPos = new Vector2(startPos.x, -_yPos);

        for (float t = 0; t < 1.0f; t += Time.deltaTime / _duration)
        {
            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        rect.anchoredPosition = targetPos; // Ensure the final position is exactly at the target

        HideOrRevealCoroutine = null;
    }

    #endregion
}
