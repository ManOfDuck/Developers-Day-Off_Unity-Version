using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] private UIDocument UIDoc;


    private VisualElement root, pauseParent;
    private Button resumeButton;
    private Button quitButton;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnGameResumed.AddListener(HideMenu);
        GameManager.Instance.OnGamePaused.AddListener(ShowMenu);

        root = UIDoc.rootVisualElement;
        root.style.visibility = Visibility.Hidden;

        pauseParent = UIDoc.rootVisualElement.Q<VisualElement>("PauseParent");

        resumeButton = UIDoc.rootVisualElement.Q<Button>("Resume");
        quitButton = UIDoc.rootVisualElement.Q<Button>("Quit");

        resumeButton.clicked += ResumeClicked;
        quitButton.clicked += quitClicked;

        pauseParent.AddToClassList("menuUp");
        //root.style.visibility = Visibility.Hidden;
    }

    private void ResumeClicked()
    {
        HideMenu();
        GameManager.Instance.ResumeGame();
    }

    private void HideMenu()
    {
        pauseParent.AddToClassList("menuUp");
        root.style.visibility = Visibility.Hidden;
        
    }

    private void ShowMenu()
    {
        root.style.visibility = Visibility.Visible;
        pauseParent.RemoveFromClassList("menuUp");
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
