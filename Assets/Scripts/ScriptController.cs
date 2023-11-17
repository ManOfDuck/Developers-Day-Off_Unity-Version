using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ScriptController : MonoBehaviour
{
    public GameObject Patrol;

    void Start()
    {
        Patrol.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowScript(string scriptName)
    {

        if (scriptName == "Patrol.cs")
        {
            //this.GetComponent<inspectorController>().HideInspector(); <-- add HideInspector() to inspectorController it just needs a root.visible == false lol, should probably make a ShowInspector() too
            Patrol.SetActive(true);
        }
    }
}

