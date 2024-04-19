using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SimulatedObject;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;
using System.Reflection;
using System.Linq;

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

        simulatedObject.ComponentAdded(newSimulatedComponent);
        return newSimulatedComponent;
    }

    public SimulatedScript AddSimulatedScriptToObject(SimulatedScript simulatedScript, SimulatedObject simulatedObject)
    {
        SimulatedScript addedScript = (SimulatedScript)AddComponentToObject(simulatedScript, simulatedObject.gameObject);
        addedScript.ParentObject = simulatedObject;
        simulatedObject.ScriptAdded(addedScript);
        return addedScript;
    }

    private Component AddComponentToObject(Component component, GameObject gameObject)
    {
        // Get the component's type and properties
        System.Type componentType = component.GetType();
        BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
        PropertyInfo[] componentProperties = componentType.GetProperties(flags);

        // Discard properties that have no set method or are declared too high up
        IEnumerable<PropertyInfo> settableProperties = from property in componentProperties
                                            where property.CanWrite
                                            where property.DeclaringType != typeof(Component)
                                            where property.DeclaringType != typeof(Object)
                                            select property;

        // Add a new component of that type to this object
        Component copiedComponent = gameObject.AddComponent(componentType);

        // Iterate through each property and copy its value
        foreach (PropertyInfo property in settableProperties)
        {
            Debug.Log(property.Name);

            // Get the value of the property from the original component
            object value = property.GetValue(component);

            // Set the value of the property in the added component
            property.SetValue(copiedComponent, value);
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

        if (button is not null)
        {
            button.text = script.visualScript.title + ".cs";
        }

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
