using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private UIDocument UIDoc;
    private Button resumeButton;

    private bool paused;

    private void Awake()
    {
        resumeButton = UIDoc.rootVisualElement.Q<Button>("ResumeButton");
        resumeButton.clicked += OnPlayerUnPaused;
    }

    private void Start()
    {
        paused = false;
        UIDoc.rootVisualElement.style.visibility = Visibility.Hidden;
    }
    public void OnPlayerPaused()
    {
        //show the pause menu, hide w Visibility.Hidden;
        UIDoc.rootVisualElement.style.visibility = Visibility.Visible;
        paused = true;
        GameManager.Instance.PauseGame();
    }
    public void OnPlayerUnPaused()
    {
        //show the pause menu, hide w Visibility.Hidden;
        UIDoc.rootVisualElement.style.visibility = Visibility.Hidden;
        paused = false;
        GameManager.Instance.ResumeGame();
    }
    public void OpenClosePause(InputAction.CallbackContext context)
    {
        if (paused)
        {
            OnPlayerUnPaused();
        }
        else if (!paused)
        {
            OnPlayerPaused();
        }
    }

}
