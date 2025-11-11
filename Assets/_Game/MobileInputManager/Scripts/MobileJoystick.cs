using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MobileJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private RectTransform _joystickBackground;
    [SerializeField] private RectTransform _joystickHandle;

    private Vector2 _inputVector;
    public Vector2 InputDirection => _inputVector;

    private Vector2 _startPosition;
    private float _radius;

    private void Start()
    {
        _radius = _joystickBackground.sizeDelta.x * 0.5f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _startPosition = eventData.position;
        _joystickBackground.position = _startPosition;
        _joystickHandle.gameObject.SetActive(true);
        _joystickBackground.gameObject.SetActive(true);

        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _joystickBackground, eventData.position, eventData.pressEventCamera, out pos))
        {
            pos = Vector2.ClampMagnitude(pos, _radius);
            _joystickHandle.localPosition = pos;
            _inputVector = pos / _radius;}
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _joystickHandle.gameObject.SetActive(false);
        _joystickBackground.gameObject.SetActive(false);

        _inputVector = Vector2.zero;
        _joystickHandle.localPosition = Vector2.zero;
    }
}
