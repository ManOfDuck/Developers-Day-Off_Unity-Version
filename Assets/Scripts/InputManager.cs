using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] float jumpBufferTime;

    public Vector2 moveInput = Vector2.zero;
    public bool jumpBufferActive = false;
    public bool jumpHeld = false;
    public Vector2 mousePos;

    private IEnumerator jumpBufferCoroutine;

    private static InputManager _instance;
    public static InputManager Instance { get { return _instance; } }

    public UnityEvent OnJumpPressed;
    public UnityEvent OnJumpReleased;
    public UnityEvent OnClick;

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

    public void SetMousePos(InputAction.CallbackContext context)
    { 
        mousePos = context.ReadValue<Vector2>();
    }

    public void MouseClicked(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnClick.Invoke();
        }
    }

    public void ConsumeJumpInput()
    {
        if (jumpBufferCoroutine is not null)
        {
            StopCoroutine(jumpBufferCoroutine);
            jumpBufferActive = false;
        }
    }

    private IEnumerator DoJumpBuffer()
    {
        jumpBufferActive = true;
        yield return new WaitForSeconds(jumpBufferTime);
        jumpBufferActive = false;
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
