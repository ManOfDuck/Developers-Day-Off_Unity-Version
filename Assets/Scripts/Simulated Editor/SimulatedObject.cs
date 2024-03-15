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
        SimulatedComponent newSimulatedComponent = new()
        {
            parentObject = this,
            realComponent = CopyComponent(simulatedComponent.realComponent),
            visualComponent = simulatedComponent.visualComponent
        };

        components.Add(newSimulatedComponent);
    }

    public void AddSimulatedScript(SimulatedScript simulatedScript)
    {
        SimulatedScript newSimulatedScript = (SimulatedScript) CopyComponent(simulatedScript);
        scripts.Add(newSimulatedScript);
    }

    private Component CopyComponent(Component component)
    {
        // Get the component's type and fields
        System.Type componentType = component.GetType();
        System.Reflection.FieldInfo[] componentFields = componentType.GetFields();

        // Add a new component of that type to this object
        Component copiedComponent = this.gameObject.AddComponent(componentType);

        // Iterate through each field and copy its value
        foreach (var field in componentFields)
        {
            // Get the value of the field from the original component
            object value = field.GetValue(component);

            // Set the value of the field in the added component
            field.SetValue(copiedComponent, value);
        }

        return copiedComponent;
    }
}
