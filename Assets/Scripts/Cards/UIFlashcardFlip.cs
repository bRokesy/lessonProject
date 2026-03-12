using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFlashcardFlip : MonoBehaviour
{
    [Header("Sides")]
    [SerializeField] private GameObject frontSide;
    [SerializeField] private GameObject backSide;

    [Header("Front")]
    [SerializeField] private TextMeshProUGUI foreignText;

    [Header("Back")]
    [SerializeField] private TextMeshProUGUI translationText;
    [SerializeField] private Image picture;

    [Header("Flip Settings")]
    [SerializeField] private float halfFlipDuration = 0.15f;
    [SerializeField] private AnimationCurve easing = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private RectTransform rect;
    private bool isFront = true;
    private bool isFlipping = false;

    void Awake()
    {
        rect = (RectTransform)transform;
        var btn = GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(TryFlip);
    }

    public void SetData(string foreignWord, string translation, Sprite img)
    {
        if (foreignText)     foreignText.text     = foreignWord;
        if (translationText) translationText.text = translation;
        if (picture)         picture.sprite       = img;

        ResetToFront();
    }

    /// <summary>Сбросить карточку на лицевую сторону без анимации.</summary>
    public void ResetToFront()
    {
        isFront = true;
        if (frontSide) frontSide.SetActive(true);
        if (backSide)  backSide.SetActive(false);
        if (rect)      rect.localScale = Vector3.one;
        isFlipping = false;
    }

    public void TryFlip()
    {
        if (isFlipping) return;
        StartCoroutine(FlipRoutine());
    }

    IEnumerator FlipRoutine()
    {
        isFlipping = true;
        yield return ScaleX(1f, 0f, halfFlipDuration);

        isFront = !isFront;
        if (frontSide) frontSide.SetActive(isFront);
        if (backSide)  backSide.SetActive(!isFront);

        yield return ScaleX(0f, 1f, halfFlipDuration);
        isFlipping = false;
    }

    IEnumerator ScaleX(float from, float to, float duration)
    {
        float t = 0f;
        Vector3 s = rect.localScale;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float x = Mathf.Lerp(from, to, easing.Evaluate(Mathf.Clamp01(t / duration)));
            rect.localScale = new Vector3(x, s.y, s.z);
            yield return null;
        }
        rect.localScale = new Vector3(to, s.y, s.z);
    }
}