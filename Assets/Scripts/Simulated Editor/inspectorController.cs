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

        IMGTog = root.Q<Toggle>("IMG_toggle");

        BC2DTog = root.Q<Toggle>("RB2D_toggle");

        BC2DTog = root.Q<Toggle>("BC2D_toggle");



        test = root.Q<Image>("SR_image");


    }

    void Start()
    {
        root.visible = false;
    }

    void Update()
    {
        
    }

    public void DisplayObject(SimulatedObject obj, Sprite noSprite, Sprite sprite)
    {
        root.visible = true;

        //SET TOGGLES
        if (obj.getComponentEnabledStatus(obj.components[1]) == true) //check if Sprite Renderer = true
        {
            SRTog.value = true;
        } else
        {
            SRTog.value = false;
        }
       
        if( obj.GetComponent<SpriteRenderer>().sprite.name == noSprite.name) //check to see if sprite is default or not
        {
            IMGTog.value = false;
        }else
        {
            IMGTog.value = true;
        }

        if (obj.getComponentEnabledStatus(obj.components[2]) == true) //check if BoxCollider = true
        {
            BC2DTog.value = true;
        }
        else
        {
            BC2DTog.value = false;
        }



    }    
        
      


    //this one is just for testing
    public void SetNameTag(string name, string tag)
    {
        objectName.text = name;
        objectTag.text = tag;
    }
}
