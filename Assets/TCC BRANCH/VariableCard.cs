using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VariableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Card Components")]
    public TMP_Text variableName;
    public Image cardImage;

    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private Transform originalParent;

    private string cardText;

    void Start()
    {
        // Configurar componentes necessários
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        originalPosition = transform.position;
        originalParent = transform.parent;

        // Configurar texto se disponível
        if (variableName != null)
            cardText = variableName.text;
    }

    public void Initialize(string text)
    {
        cardText = text;
        if (variableName != null)
            variableName.text = text;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.7f;
            canvasGroup.blocksRaycasts = false;
        }

        if (canvas != null)
            transform.SetParent(canvas.transform); // Mover para frente
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        // Verificar se foi solto em uma zona de drop válida
        SimpleDropZone dropZone = GetDropZoneUnderPointer(eventData);

        if (dropZone != null)
        {
            // Carta foi solta numa zona válida
            Debug.Log($"Carta {cardText} solta na zona: {dropZone.zoneName}");
            dropZone.OnCardDropped(this);
        }
        else
        {
            // Retornar à posição original
            ReturnToOriginalPosition();
        }
    }

    SimpleDropZone GetDropZoneUnderPointer(PointerEventData eventData)
    {
        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        foreach (var result in raycastResults)
        {
            SimpleDropZone dropZone = result.gameObject.GetComponent<SimpleDropZone>();
            if (dropZone != null)
            {
                return dropZone;
            }
        }

        return null;
    }

    void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent);
        transform.position = originalPosition;
    }

    public string GetCardText()
    {
        return cardText;
    }
}

// Classe simples para zonas de drop
public class SimpleDropZone : MonoBehaviour
{
    [Header("Drop Zone Configuration")]
    public string zoneName = "Drop Zone";
    public Color highlightColor = Color.yellow;

    private Image backgroundImage;
    private Color originalColor;

    void Start()
    {
        backgroundImage = GetComponent<Image>();
        if (backgroundImage != null)
            originalColor = backgroundImage.color;
    }

    public void OnCardDropped(VariableCard card)
    {
        Debug.Log($"Carta '{card.GetCardText()}' foi solta na zona '{zoneName}'");

        // Aqui você pode implementar a lógica de verificação se a resposta está correta
        // Por exemplo, verificar se o nome da carta corresponde ao tipo correto para esta zona

        // Por enquanto, apenas um feedback visual
        if (backgroundImage != null)
        {
            backgroundImage.color = Color.green;
            // Voltar à cor original após 1 segundo
            Invoke("ResetColor", 1f);
        }
    }

    void ResetColor()
    {
        if (backgroundImage != null)
            backgroundImage.color = originalColor;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<VariableCard>() != null)
        {
            if (backgroundImage != null)
                backgroundImage.color = highlightColor;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<VariableCard>() != null)
        {
            if (backgroundImage != null)
                backgroundImage.color = originalColor;
        }
    }
}