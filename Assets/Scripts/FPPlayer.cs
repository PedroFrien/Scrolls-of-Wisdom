using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(FPController))]
public class FPPlayer : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] FPController FPController;

    #region Input Handling


    void OnMove(InputValue value)
    {
        FPController.MoveInput = value.Get<Vector2>();
    }

    void OnLook(InputValue value)
    {
        FPController.LookInput = value.Get<Vector2>();
    }

    void OnSprint(InputValue value)
    {
        FPController.SprintInput = value.isPressed;
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            FPController.TryJump();
        }
    }

    void OnDash(InputValue value)
    {
        if (value.isPressed)
        {
            FPController.TryDash();
        }
    }

    void OnRestart(InputValue value)
    {
        if (value.isPressed)
        {
            FPController.Restart();
        }
    }

    void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            Debug.Log("Pressing Button?");
            FPController.TryInteract();
        }
    }

    #endregion


    #region Unity Methods

    private void OnValidate()
    {
        if (FPController == null)
        {
            FPController = GetComponent<FPController>();
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false;
    }

    #endregion
}
