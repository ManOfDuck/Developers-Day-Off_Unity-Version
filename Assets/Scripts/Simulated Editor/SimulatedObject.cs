using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatedObject : MonoBehaviour
{
    public bool interactable = true;
    public List<SimulatedComponent> components;
    public List<SimulatedScript> scripts;
    public Collider2D clickTrigger;
    public LayerMask layer;
    private InspectorController inspectorController;
    private ComponentManager componentManager;
    private GameManager gameManager;

    public Sprite defaultSprite;
    public Sprite sprite1;

  

    public void Start()
    {
        inspectorController = InspectorController.Instance;
        componentManager = ComponentManager.Instance;
        gameManager = GameManager.Instance;
        layer = gameObject.layer;
        foreach (SimulatedComponent component in components)
        {
            component.parentObject = this;
        }
        foreach (SimulatedScript script in scripts)
        {
            componentManager.SetScriptEnabledStatus(script, script.isActiveAndEnabled);
        }
    }


    public void OnMouseDown()
    {
        if (interactable)
            inspectorController.DisplayObject(this, defaultSprite, sprite1);
    }
}
