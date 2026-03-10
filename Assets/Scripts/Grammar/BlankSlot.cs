using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BlankSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    public Image background;
    public TextMeshProUGUI placeholderText;

    [Header("Colors")]
    public Color emptyColor   = new Color(0.85f, 0.85f, 0.9f, 1f);
    public Color filledColor  = new Color(0.8f,  0.95f, 0.8f, 1f);
    public Color hoverColor   = new Color(0.7f,  0.85f, 1f,   1f);
    
    [HideInInspector] public string correctAnswer;
    
    public DraggableWord CurrentChip { get; private set; }

    void Start()
    {
        SetEmpty();
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableWord chip = eventData.pointerDrag?.GetComponent<DraggableWord>();
        if (chip == null) return;
        
        if (CurrentChip != null)
            CurrentChip.ReturnToBank();
        
        if (chip.CurrentSlot != null)
            chip.CurrentSlot.ClearSlot();

        PlaceChip(chip);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.dragging)
            background.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        background.color = CurrentChip != null ? filledColor : emptyColor;
    }

    public void PlaceChip(DraggableWord chip)
    {
        CurrentChip = chip;
        chip.PlaceInSlot(this);

        if (placeholderText) placeholderText.gameObject.SetActive(false);
        background.color = filledColor;
    }

    public void ClearSlot()
    {
        CurrentChip = null;
        SetEmpty();
    }

    public bool IsCorrect()
    {
        if (CurrentChip == null) return false;
        return CurrentChip.Word.Trim() == correctAnswer.Trim();
    }

    void SetEmpty()
    {
        if (placeholderText) placeholderText.gameObject.SetActive(true);
        if (background) background.color = emptyColor;
    }
}