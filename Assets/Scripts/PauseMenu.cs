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
        root = UIDoc.rootVisualElement;

        resumeButton = UIDoc.rootVisualElement.Q<Button>("Resume");
        quitButton = UIDoc.rootVisualElement.Q<Button>("Quit");

        resumeButton.clicked += resumeClicked;
        quitButton.clicked += quitClicked;

        //root.style.visibility = Visibility.Hidden;
    }

    private void resumeClicked()
    {
        Debug.Log("resume clicked");
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
