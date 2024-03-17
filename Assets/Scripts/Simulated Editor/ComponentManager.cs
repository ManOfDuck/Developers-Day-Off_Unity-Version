using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SimulatedObject;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ComponentManager : MonoBehaviour
{
    public static ComponentManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public SimulatedComponent AddSimulatedComponentToObject(SimulatedComponent simulatedComponent, SimulatedObject simulatedObject)
    {
        Component newComponent = AddComponentToObject(simulatedComponent.realComponent, simulatedObject.gameObject);

        SimulatedComponent newSimulatedComponent = new()
        {
            parentObject = simulatedObject,
            realComponent = newComponent,
            visualComponent = simulatedComponent.visualComponent
        };

        return newSimulatedComponent;
    }

    public SimulatedScript AddSimulatedScriptToObject(SimulatedScript simulatedScript, SimulatedObject simulatedObject)
    {
        return (SimulatedScript) AddComponentToObject(simulatedScript, simulatedObject.gameObject); 
    }

    private Component AddComponentToObject(Component component, GameObject gameObject)
    {
        // Get the component's type and fields
        System.Type componentType = component.GetType();
        System.Reflection.FieldInfo[] componentFields = componentType.GetFields();

        // Add a new component of that type to this object
        Component copiedComponent = gameObject.AddComponent(componentType);

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

    public VisualElement GetComponentDisplay(SimulatedComponent component, VisualTreeAsset template)
    {
        VisualComponent visualComponent = component.visualComponent;

        VisualElement componentDisplay = template.CloneTree();
        VisualElement icon = componentDisplay.Q<VisualElement>("image");
        Label label = componentDisplay.Q<Label>("label");
        Label description = componentDisplay.Q<Label>("desc");
 
        icon.style.backgroundImage = visualComponent.image.texture;
        label.text = visualComponent.title;
        description.text = visualComponent.description;

        return componentDisplay;
    }
    public VisualElement GetScriptDisplay(SimulatedScript script, VisualTreeAsset template)
    {
        VisualElement scriptDisplay = template.CloneTree();

        Button button = scriptDisplay.Q<Button>("button");

        button.text = script.visualScript.title + ".cs";
        // This will need a rework to work for both toolkit and inspector, for now we'll disable it to avoid player confusion
        /*
        button.clickable.clicked += () =>
        {
            Display(script.GetUIDoc(panelSettings));
        };
        */

        return scriptDisplay;
    }

    public bool IsComponentToggleable(SimulatedComponent component)
    {
        switch (component.realComponent)
        {
            case Collider2D:
                return true;
            case SpriteRenderer:
                return true;
            case Animator:
                return true;
            default:
                return false;
        }
    }

    public bool GetComponentEnabledStatus(SimulatedComponent component)
    {
        switch (component.realComponent)
        {
            case Collider2D collider:
                return collider.enabled;
            case SpriteRenderer renderer:
                return renderer.enabled;
            case Animator animator:
                return animator.enabled;
            default:
                return true;
        }
    }

    public void SetComponentEnabledStatus(SimulatedComponent component, bool enabled)
    {
        switch (component.realComponent)
        {
            case Collider2D collider:
                collider.enabled = enabled;
                gameObject.layer = enabled ? component.parentObject.layer : 0;
                break;
            case SpriteRenderer renderer:
                renderer.enabled = enabled;
                break;
            case Animator animator:
                animator.enabled = enabled;
                break;
            default:
                Debug.Log("hello");
                break;
        }
    }

    public void ToggleComponent(SimulatedComponent component)
    {
        SetComponentEnabledStatus(component, !GetComponentEnabledStatus(component));
    }


    public void SetScriptEnabledStatus(SimulatedScript script, bool enabled)
    {
        script.enabled = enabled;
        script.doCoroutines = enabled;
        script.doCollisionEvents = enabled;
    }

    public void ToggleScript(SimulatedScript script)
    {
        SetScriptEnabledStatus(script, !script.enabled);
    }
}
