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
    private bool isTalking = false;


    // Start is called before the first frame update
    void Start()
    {
        root = UIDoc.rootVisualElement;
        image = root.Q<VisualElement>("speakerImage");
        text = root.Q<Label>("speakerText");
        name = root.Q<Label>("speakerName");
        name.text = speakerName;
        root.style.visibility = Visibility.Hidden;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isTalking)
        {
            Debug.Log("conversatin");
            conversationPosition++;
            conversation();
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("trigger hit");
        root.style.visibility = Visibility.Visible;
        conversation();
    }

    void conversation()
    {
        isTalking = true;
        Time.timeScale = 0;
        if(conversationPosition < conversationArray.Length)
        {
            text.text = conversationArray[conversationPosition];
        }
        else
        {
            root.style.visibility = Visibility.Hidden;
            Time.timeScale = 1;
            Destroy(collider);
            isTalking = false;
            return;
        }

    }

}
