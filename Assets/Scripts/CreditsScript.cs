using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class CreditsScript : MonoBehaviour
{

    [SerializeField] UIDocument UIDoc;

    private VisualElement root;
    private Button mainMenuButton;

    // Start is called before the first frame update
    void Start()
    {
        root = UIDoc.rootVisualElement;

        mainMenuButton = UIDoc.rootVisualElement.Q<Button>("Menu");
        mainMenuButton.clicked += MainMenuClicked;
    }

    private void MainMenuClicked()
    {
        Debug.Log("main menu clicked");
        SceneManager.LoadScene("Main Menu");
    }
}
