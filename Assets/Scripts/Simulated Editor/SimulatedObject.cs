using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatedObject : MonoBehaviour
{
    public List<SimulatedComponent> components;
    public List<SimulatedScript> scripts;
    private inspectorController controller;

    public Sprite defaultSprite;
    public Sprite sprite1;

    [System.Serializable]
    public class SimulatedComponent
    {
        public Component realComponent;
        public VisualComponent visualComponent;
    }

    public void Start()
    {
        controller = inspectorController.Instance;
    }

    public bool isComponentToggleable(SimulatedComponent component)
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

    public bool getComponentEnabledStatus (SimulatedComponent component)
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

    public void setComponentEnabledStatus(SimulatedComponent component, bool enabled)
    {
        switch (component.realComponent)
        {
            case Collider2D collider:
                collider.enabled = enabled;
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

    public void toggleComponent(SimulatedComponent component)
    {
        setComponentEnabledStatus(component, !getComponentEnabledStatus(component));
    }


    public void setScriptEnabledStatus(SimulatedScript script, bool enabled)
    {
        script.enabled = enabled;
        script.doCoroutines = enabled;
    }

    public void toggleScript(SimulatedScript script)
    {
        setScriptEnabledStatus(script, !script.enabled);
    }


    public void OnMouseDown()
    {
        controller.DisplayObject(this, defaultSprite, sprite1);
    }
}
