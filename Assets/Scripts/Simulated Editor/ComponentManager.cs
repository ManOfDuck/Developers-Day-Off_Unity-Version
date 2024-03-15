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
        // This will need a rework to work for both toolkit and inspector, for now we'll disable it to avoid confusion
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
