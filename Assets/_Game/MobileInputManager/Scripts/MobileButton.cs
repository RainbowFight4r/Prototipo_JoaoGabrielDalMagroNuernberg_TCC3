using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MobileButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [Header("Events")]
    public UnityEvent onClick;
    public UnityEvent onPressStart;
    public UnityEvent onPressEnd;

    [Header("Settings")]
    public bool invokeClickOnPointerUp = true;

    public bool IsBeingPressed { get; private set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsBeingPressed = true;
        onPressStart?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsBeingPressed = false;
        onPressEnd?.Invoke();

        if (invokeClickOnPointerUp)
            onClick?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!invokeClickOnPointerUp)
            onClick?.Invoke();
    }
}