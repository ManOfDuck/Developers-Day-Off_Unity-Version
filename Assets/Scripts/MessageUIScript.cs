using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MessageUIScript : MonoBehaviour
{

    [SerializeField] UIDocument UIDoc;
    [SerializeField] string speakerName;
    [SerializeField] BoxCollider2D collider;
    [SerializeField] string[] conversationArray;

    private int conversationPosition = 0;
    private VisualElement root;
    private VisualElement image;
    private Label name, text;


    // Start is called before the first frame update
    void Start()
    {
        root = UIDoc.rootVisualElement;
        image = root.Q<VisualElement>("speakerImage");
        text = root.Q<Label>("speakerText");
        name = root.Q<Label>("speakerName");
        //text.text = conversationArray[0];
        name.text = speakerName;
        root.style.visibility = Visibility.Hidden;
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("trigger hit");
        root.style.visibility = Visibility.Visible;
        conversation();
    }

    void conversation()
    {
        //conversationPosition++;
        //if(conversationArray[conversationPosition] != null)
        if(conversationPosition < conversationArray.Length)
        {
            text.text = conversationArray[conversationPosition];
        }
        else
        {
            root.style.visibility = Visibility.Hidden;
        }
        conversationPosition++;
    }

}
