using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    Sprite globalSpriteDefault, globarSprite1;
    SimulatedObject currentObject;

    
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
        if (currentObject == null)
        {
            //catch exeption;
        } 

        if (SRTog.value == true) // set spriterender based on toggle
        {
            //currentObject.
            currentObject.setComponentEnabledStatus(currentObject.components[1],true);
            //Debug.Log("yea");
        }
        else if (SRTog.value == false)
        {
            currentObject.setComponentEnabledStatus(currentObject.components[1], false);
            //currentObject.toggleComponent(currentObject.components[1]);
            //Debug.Log("Awr Nawr");
        }

        if (IMGTog.value == true) //set image based on toggle
        {
            currentObject.GetComponent<SpriteRenderer>().sprite = globarSprite1;
        } else if (IMGTog.value == false)
        {
            currentObject.GetComponent<SpriteRenderer>().sprite = globalSpriteDefault;
        }

    }

    public void DisplayObject(SimulatedObject obj, Sprite noSprite, Sprite sprite)
    {
        root.visible = true;
        currentObject = obj;
        globalSpriteDefault = noSprite;
        globarSprite1 = sprite;

        //SET OBJ NAME & TAG
        objectName.text = obj.gameObject.name.ToString();
        objectTag.text = obj.gameObject.tag.ToString();

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


        

}
