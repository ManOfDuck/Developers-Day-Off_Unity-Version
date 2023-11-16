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
    Toggle BC2DTog;
    private void OnEnable() // get ui references B-)
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        objectName = root.Q<Label>("Object_name");
        objectTag = root.Q<Label>("Tag");
        BC2DTog = root.Q<Toggle>("BC2D_toggle");

    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void DisplayObject(SimulatedObject obj)
    {
        Debug.Log("WAH");
        
        if (obj.getComponentEnabledStatus(obj.components[0]) == true) //check if boxcolider = true
        {
            BC2DTog.value = true; // change the box collider toggle to true
            //IN THEORY
        }
       


    }


    //this one is just for testing
    public void SetNameTag(string name, string tag)
    {
        objectName.text = name;
        objectTag.text = tag;
    }
}
