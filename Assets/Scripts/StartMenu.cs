using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private UIDocument UIDoc;
    [SerializeField] private SettingsMenu settingsMenu;


    private VisualElement root;
    private Button startButton;
    private Button settingsButton;
    private Button creditsButton;
    private Button quitButton;

    // Start is called before the first frame update
    void Start()
    {
       // GameManager.Instance.OnGameResumed.AddListener(HideMenu);
        //GameManager.Instance.OnGamePaused.AddListener(ShowMenu);

        root = UIDoc.rootVisualElement;
        root.style.visibility = Visibility.Visible;

        startButton = UIDoc.rootVisualElement.Q<Button>("Start");
        creditsButton = UIDoc.rootVisualElement.Q<Button>("Credits");
        settingsButton = UIDoc.rootVisualElement.Q<Button>("Settings");
        //quitButton = UIDoc.rootVisualElement.Q<Button>("Quit");

        startButton.clicked += StartClicked;
        creditsButton.clicked += CreditsClicked;
        settingsButton.clicked += SettingsClicked;
        
        //quitButton.clicked += quitClicked;
        //root.style.visibility = Visibility.Hidden;
    }

    private void StartClicked()
    {
        Debug.Log("start clicked");
        SceneManager.LoadScene("Map Screen");
    }

    private void CreditsClicked()
    {
        SceneManager.LoadScene("CreditsScene");
    }

    private void SettingsClicked()
    {
        settingsMenu.showMenu();
    }

    private void HideMenu()
    {
        root.style.visibility = Visibility.Hidden;
    }

    private void ShowMenu()
    {
        Debug.Log("pause");
        root.style.visibility = Visibility.Visible;
    }

    private void quitClicked()
    {
        Debug.Log("quit clicked");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
