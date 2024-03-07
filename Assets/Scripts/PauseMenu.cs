using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] private UIDocument UIDoc;


    private VisualElement root;
    private Button resumeButton;
    private Button quitButton;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnGameResumed.AddListener(HideMenu);
        GameManager.Instance.OnGamePaused.AddListener(ShowMenu);

        root = UIDoc.rootVisualElement;
        root.style.visibility = Visibility.Hidden;

        resumeButton = UIDoc.rootVisualElement.Q<Button>("Resume");
        quitButton = UIDoc.rootVisualElement.Q<Button>("Quit");

        resumeButton.clicked += ResumeClicked;
        quitButton.clicked += quitClicked;

        //root.style.visibility = Visibility.Hidden;
    }

    private void ResumeClicked()
    {
        Debug.Log("resume clicked");
        HideMenu();
        GameManager.Instance.ResumeGame();
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
