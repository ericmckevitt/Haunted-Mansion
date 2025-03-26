using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;

    private PlayerMotor motor;
    private PlayerLook look;

    private bool inputEnabled = true;

    void Awake()
    {
        Instance = this;

        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;

        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();

        onFoot.Jump.performed += ctx => {
            if (inputEnabled)
                motor.Jump();
        };
    }

    void FixedUpdate()
    {
        if (inputEnabled)
        {
            motor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
        }
    }

    private void LateUpdate()
    {
        if (inputEnabled)
        {
            look.ProcesssLook(onFoot.Look.ReadValue<Vector2>());
        }
    }

    private void OnEnable()
    {
        onFoot.Enable();
    }

    private void OnDisable()
    {
        onFoot.Disable();
    }

    public static void DisableInput()
    {
        Instance.inputEnabled = false;
    }

    public static void EnableInput()
    {
        Instance.inputEnabled = true;
    }
}
