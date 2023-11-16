using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public SimulatedObject simulatedObject;
    public inspectorController controller;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        controller.DisplayObject(simulatedObject);
      
    }


}
