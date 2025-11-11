using UnityEngine;

public class MobileInputDebug : MonoBehaviour
{
    private MobileInputActions _input;

    private void Awake()
    {
        _input = new MobileInputActions();
    }

    private void OnEnable()
    {
        _input.Player.Enable();
    }

    private void OnDisable()
    {
        _input.Player.Disable();
    }

    private void Update()
    {
        Vector2 move = _input.Player.Move.ReadValue<Vector2>();
        if (move != Vector2.zero)
            Debug.Log($"[MobileInput] Move: {move}");
    }
}
