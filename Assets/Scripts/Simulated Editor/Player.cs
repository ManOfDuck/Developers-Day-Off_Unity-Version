using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public SimulatedObject simulatedObject;
    public inspectorController controller;

    public Sprite defaultSprite;
    public Sprite sprite1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnMouseDown()
    {
        controller.DisplayObject(simulatedObject, defaultSprite, sprite1);
    }
    // Update is called once per frame
    private void Update()
    {
        
      
    }


}
