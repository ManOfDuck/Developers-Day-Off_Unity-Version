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
    Toggle TRANSTog, SRTog, IMGTog, RB2DTog, BC2DTog;
    Image test;
    private void OnEnable() // get ui references B-)
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        objectName = root.Q<Label>("Object_name");
        objectTag = root.Q<Label>("Tag");


        SRTog = root.Q<Toggle>("SR_toggle");

        //IMGTog = root.Q<Toggle>("IMG_toggle");

        BC2DTog = root.Q<Toggle>("RB2D_toggle");

        BC2DTog = root.Q<Toggle>("BC2D_toggle");



        test = root.Q<Image>("SR_image");


    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void DisplayObject(SimulatedObject obj)
    {

        //SET TOGGLES
        if (obj.getComponentEnabledStatus(obj.components[1]) == true) //check if Sprite Renderer = true
        {
            SRTog.value = true; // change the box collider toggle to true
                                  //IN THEORY
            Debug.Log("component " + 1 + "should be being toggled");
        }

     
            
    }    
        
      


    //this one is just for testing
    public void SetNameTag(string name, string tag)
    {
        objectName.text = name;
        objectTag.text = tag;
    }
}
