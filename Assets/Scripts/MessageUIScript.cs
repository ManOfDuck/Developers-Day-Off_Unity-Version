using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MessageUIScript : MonoBehaviour
{

    [SerializeField] UIDocument UIDoc;
    [SerializeField] string speakerName;
    [SerializeField] BoxCollider2D triggerCollider;
    [SerializeField] string[] conversationArray;
    [SerializeField] float timeDelay;
    [SerializeField] GameObject objectToEnable;

    private int conversationPosition = 0;
    private VisualElement root;
    private VisualElement image;
    private Label conversationName, text;
    private bool isTalking = false;


    // Start is called before the first frame update
    void Start()
    {
        root = UIDoc.rootVisualElement;
        image = root.Q<VisualElement>("speakerImage");
        text = root.Q<Label>("speakerText");
        conversationName = root.Q<Label>("speakerName");
        conversationName.text = speakerName;
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
        if(other.tag == "UnRootable" || other.tag == "Player")
        {
            Debug.Log("trigger hit");
            StartCoroutine(DoTimer());
        }
    }

    private IEnumerator DoTimer()
    {
        yield return new WaitForSeconds(timeDelay);
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
            Destroy(triggerCollider);
            isTalking = false;
            objectToEnable.SetActive(true);
            return;
        }

    }

}
