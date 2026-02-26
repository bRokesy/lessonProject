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
    [SerializeField] private float halfFlipDuration = 0.15f; // 0.15 + 0.15 = 0.3s
    [SerializeField] private AnimationCurve easing = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private RectTransform rect;
    private bool isFront = true;
    private bool isFlipping = false;

    private void Awake()
    {
        rect = (RectTransform)transform;

        // Подпишемся на Button, чтобы не ловить клики вручную
        var btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(TryFlip);
    }

    public void SetData(string foreignWord, string translation, Sprite img)
    {
        if (foreignText) foreignText.text = foreignWord;
        if (translationText) translationText.text = translation;
        if (picture) picture.sprite = img;

        isFront = true;
        if (frontSide) frontSide.SetActive(true);
        if (backSide) backSide.SetActive(false);

        // гарантируем нормальный масштаб
        rect.localScale = Vector3.one;
    }

    public void TryFlip()
    {
        if (isFlipping) return;
        StartCoroutine(FlipRoutine());
    }

    private IEnumerator FlipRoutine()
    {
        isFlipping = true;

        // 1) схлопываем по X: 1 -> 0
        yield return ScaleX(1f, 0f, halfFlipDuration);

        // 2) меняем сторону в "нуле"
        isFront = !isFront;
        if (frontSide) frontSide.SetActive(isFront);
        if (backSide) backSide.SetActive(!isFront);

        // 3) раскрываем: 0 -> 1
        yield return ScaleX(0f, 1f, halfFlipDuration);

        isFlipping = false;
    }

    private IEnumerator ScaleX(float from, float to, float duration)
    {
        float t = 0f;

        Vector3 s = rect.localScale;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime; // чтобы работало даже при паузе/таймскейле
            float p = Mathf.Clamp01(t / duration);
            float e = easing.Evaluate(p);

            float x = Mathf.Lerp(from, to, e);
            rect.localScale = new Vector3(x, s.y, s.z);

            yield return null;
        }

        rect.localScale = new Vector3(to, s.y, s.z);
    }
}