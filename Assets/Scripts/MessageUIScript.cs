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
        if (Input.GetKeyDown(KeyCode.E) && GameManager.Instance.CurrentGameState == GameManager.GameState.Cutscene)
        {
            conversationPosition++;
            conversation();
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "UnRootable" || other.tag == "Player")
        {
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
        GameManager.Instance.EnterCutscene();
        if(conversationPosition < conversationArray.Length)
        {
            text.text = conversationArray[conversationPosition];
        }
        else
        {
            root.style.visibility = Visibility.Hidden;
            GameManager.Instance.ExitCutscene();
            Destroy(triggerCollider);
            objectToEnable.SetActive(true);
            return;
        }

    }

}
