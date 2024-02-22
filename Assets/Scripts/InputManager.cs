using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] float jumpBufferTime;

    public Vector2 moveInput = Vector2.zero;
    public bool jumpPressed = false;
    public bool jumpHeld = true;

    private IEnumerator jumpBufferCoroutine;

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
            jumpBufferCoroutine = DoJumpBuffer();
            StartCoroutine(jumpBufferCoroutine);
            OnJumpPressed.Invoke();
        }
        if (context.canceled)
        {
            jumpHeld = false;
            OnJumpReleased.Invoke();
        }
    }

    public void ConsumeJumpInput()
    {
        if (jumpBufferCoroutine is not null)
        {
            StopCoroutine(jumpBufferCoroutine);
            jumpPressed = false;
        }
    }

    private IEnumerator DoJumpBuffer()
    {
        jumpPressed = true;
        yield return new WaitForSeconds(jumpBufferTime);
        jumpPressed = false;
    }
}
