using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartMenuController : MonoBehaviour
{
    [SerializeField] private UIDocument UIDoc;
    [SerializeField] private GameObject laraImage;

    private Button startButton;
    private Button creditsButton;
    private Button quitButton;
    private VisualElement credits;

    private bool creditsOpen;


    private void Awake()
    {
        //set up the visual elements & buttons in UI
        startButton = UIDoc.rootVisualElement.Q<Button>("start-button");
        startButton.clicked += GameStart;

        creditsButton = UIDoc.rootVisualElement.Q<Button>("credits-button");
        creditsButton.clicked += Credits;

        quitButton = UIDoc.rootVisualElement.Q<Button>("quit-button");
        quitButton.clicked += QuitGame;

        credits = UIDoc.rootVisualElement.Q<VisualElement>("credits");
    }

    private void OnDestroy()
    {
        startButton.clicked -= GameStart;
        creditsButton.clicked -= Credits;
        quitButton.clicked -= QuitGame;

    }

    private void Update()
    {
        
    }

    private void Start()
    {
        creditsOpen = false;
        credits.visible = false;
    }

    public void GameStart()
    {
        Debug.Log("Start Pressed");
        //UIDoc.rootVisualElement.style.visibility = Visibility.Hidden;
        Destroy(laraImage);
        Destroy(UIDoc);
        SceneManager.LoadScene("Milestone 1");
    }
    public void Credits()
    {
        //Debug.Log("Credits Pressed");
        if (creditsOpen)
        {
            Debug.Log("closeme");
            credits.visible = false;
            creditsOpen = false;
        }
        else if (!creditsOpen)
        {
            Debug.Log("openme!");
            credits.visible = true;
            creditsOpen = true;
        }
    }
    public void QuitGame()
    {
        Debug.Log("Bye Bye");
        UIDoc.rootVisualElement.style.visibility = Visibility.Hidden;
        Destroy(laraImage);
        Application.Quit();
    }
   
}
