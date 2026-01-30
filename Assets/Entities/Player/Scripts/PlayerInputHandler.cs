using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    PlayerInput playerInput;

    InputAction LookAction;
    InputAction MoveAction;
    InputAction SprintAction;

    [SerializeField] UnityEvent<Vector2> onMove;

    [SerializeField] UnityEvent<float> onVerticalLook;
    [SerializeField] UnityEvent<float> onHorizontalLook;

    [SerializeField] UnityEvent onSprintPressed;

    [SerializeField] float lookDeadzone = 0.001f;
    [SerializeField] float moveDeadzone = 0.001f;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        LookAction = playerInput.actions["Look"];
        MoveAction = playerInput.actions["Move"];
        SprintAction = playerInput.actions["Sprint"];
    }

    private void Update()
    {
        // LOOK
        Vector2 look = LookAction.ReadValue<Vector2>() * UserData.Instance.sensitiviy;

        // Horizontal look = X (yaw), Vertical look = Y (pitch)
        if (Mathf.Abs(look.y) > lookDeadzone)
            onVerticalLook.Invoke(look.y);
        else
            onVerticalLook.Invoke(0f);

        if (Mathf.Abs(look.x) > lookDeadzone)
            onHorizontalLook.Invoke(look.x);
        else
            onHorizontalLook.Invoke(0f);

        // MOVE
        Vector2 move = MoveAction.ReadValue<Vector2>();

        if (move.sqrMagnitude > moveDeadzone * moveDeadzone)
            onMove.Invoke(move);
        else
            onMove.Invoke(Vector2.zero);

        // SPRINT
        if (SprintAction.IsInProgress())
            onSprintPressed.Invoke();
    }

    private void OnEnable()
    {
        LookAction.Enable();
        MoveAction.Enable();
        SprintAction.Enable();
    }

    private void OnDisable()
    {
        LookAction.Disable();
        MoveAction.Disable();
        SprintAction.Disable();
    }
}

