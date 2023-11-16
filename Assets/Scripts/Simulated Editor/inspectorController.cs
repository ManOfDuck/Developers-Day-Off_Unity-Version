using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class inspectorController : MonoBehaviour
{
    private static inspectorController _instance;
    public static inspectorController Instance { get { return _instance; } }

    VisualElement root;
    Label objectName, objectTag;
    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        objectName = root.Q<Label>("Object_name");
        objectTag = root.Q<Label>("Tag");
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void DisplayObject(SimulatedObject obj)
    {
        Debug.Log(obj.components.ToString());


    }


    //this one is just for testing
    public void SetNameTag(string name, string tag)
    {
        objectName.text = name;
        objectTag.text = tag;
    }
}
