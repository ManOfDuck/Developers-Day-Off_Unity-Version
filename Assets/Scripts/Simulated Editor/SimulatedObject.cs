using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UI.Image;

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

    public void AddSimulatedComponent(SimulatedComponent simulatedComponent)
    {
        SimulatedComponent newSimulatedComponent = componentManager.AddSimulatedComponentToObject(simulatedComponent, this);
        components.Add(newSimulatedComponent);
    }

    public void AddSimulatedScript(SimulatedScript simulatedScript)
    {
        SimulatedScript newSimulatedScript = componentManager.AddSimulatedScriptToObject(simulatedScript, this);
        scripts.Add(newSimulatedScript);
    }
}
