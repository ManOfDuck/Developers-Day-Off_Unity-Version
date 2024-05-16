using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class TempToggle : MonoBehaviour
{
    [SerializeField] private UIDocument UIDoc;

    private bool lechecked = false;

    private VisualElement root, toggleBG;
    private Button toggleBall;

    // Start is called before the first frame update
    void Start()
    {
        
        root = UIDoc.rootVisualElement;

        toggleBG = UIDoc.rootVisualElement.Q<VisualElement>("Toggle_BG");

        toggleBall = UIDoc.rootVisualElement.Q<Button>("Toggle_Ball");

        toggleBall.clicked += togClicked;

        toggleBall.RemoveFromClassList("togballchecked");
        toggleBG.RemoveFromClassList("togbgchecked");
        toggleBall.AddToClassList("togball");
        toggleBG.AddToClassList("togbg");
        //root.style.visibility = Visibility.Hidden;
    }

    private void togClicked()
    {
        if (!lechecked){ //activate
            toggleBall.RemoveFromClassList("togballchecked");
            toggleBG.RemoveFromClassList("togbgchecked");
            toggleBall.AddToClassList("togball");
            toggleBG.AddToClassList("togbg");
            lechecked = true;
            Debug.Log(lechecked);
        }
        else { //deactivate
            toggleBall.AddToClassList("togballchecked");
            toggleBG.AddToClassList("togbgchecked");
            toggleBall.RemoveFromClassList("togball");
            toggleBG.RemoveFromClassList("togbg");
            lechecked = false;
            Debug.Log(lechecked);

            //could also easily change component bg to be darker to easily signify disabled
        }
    }

    void Update()
    {
        
    }
}
