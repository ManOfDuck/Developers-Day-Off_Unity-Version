using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MessageUIScript : MonoBehaviour
{

    [SerializeField] UIDocument UIDoc;
    [SerializeField] string[] conversationArray;

    private int conversationPosition = 0;
    private VisualElement root;
    private VisualElement image;
    private Label text;


    // Start is called before the first frame update
    void Start()
    {
        root = UIDoc.rootVisualElement;
        image = root.Q<VisualElement>("speakerImage");
        text = root.Q<Label>("speakerText");
        text.text = conversationArray[0];
        root.style.visibility = Visibility.Hidden;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void conversation()
    {
        conversationPosition++;
        if(conversationArray[conversationPosition] != null)
        {
            text.text = conversationArray[conversationPosition];
        }
        else
        {
            root.style.visibility = Visibility.Hidden;
        }
    }

}
