using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Vector2 moveInput = Vector2.zero;
    private bool jumpHeld = false;

    public Vector2 MoveInput { get => moveInput; private set => moveInput = value; }
    public bool JumpHeld { get => jumpHeld; private set => jumpHeld = value; }

    private static InputManager _instance;
    public static InputManager Instance { get { return _instance; } }

    public Vector2 ScreenMousePosition { get; private set; }
    public Vector2 WorldMousePosition => Camera.main.ScreenToWorldPoint(ScreenMousePosition);


    public UnityEvent OnJumpPressed;
    public UnityEvent OnJumpReleased;
    public UnityEvent OnClick;
    public UnityEvent OnInteract;

    [SerializeField] private CursorController _cursorController;
    public CursorController CursorController => _cursorController;

    private void Awake()
    {
        if (Instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Instance.CursorController.ShowCursor = this.CursorController.ShowCursor;
            Destroy(this.gameObject);
        }
    }

    public void SetMoveInput(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    public void UpdateMousePos(InputAction.CallbackContext context)
    {
        ScreenMousePosition = context.ReadValue<Vector2>();
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnInteract.Invoke();
        }
    }

    public void SetJumpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            JumpHeld = true;
            OnJumpPressed.Invoke();
        }
        if (context.canceled)
        {
            JumpHeld = false;
            OnJumpReleased.Invoke();
        }
    }

    public void ResetLevel(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("reseting level");
            //PlayerSpawn.Respawn();

            GameManager.Instance.ResetScene();
        }
    }

    public void ClearLevel(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameManager.Instance.ClearLevel();
        }
    }

    public void TogglePause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameManager.Instance.TogglePause();
        }
    }

    public void Click(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnClick.Invoke();
        }
    }
}
