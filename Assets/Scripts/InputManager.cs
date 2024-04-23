using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public Vector2 moveInput = Vector2.zero;
    public bool jumpHeld = false;

    private static InputManager _instance;
    public static InputManager Instance { get { return _instance; } }

    public UnityEvent OnJumpPressed;
    public UnityEvent OnJumpReleased;

    private void Awake()
    {

        if (Instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void SetMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>() * Vector2.right;
    }

    public void SetJumpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            jumpHeld = true;
            OnJumpPressed.Invoke();
        }
        if (context.canceled)
        {
            jumpHeld = false;
            OnJumpReleased.Invoke();
        }
    }

    public void ResetLevel(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameManager.Instance.ResetScene();
        }
    }

    public void TogglePause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameManager.Instance.TogglePause();
        }
    }
}
