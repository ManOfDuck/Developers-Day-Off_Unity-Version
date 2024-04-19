using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UI.Image;

public class SimulatedObject : MonoBehaviour
{
    [SerializeField] private bool interactable = true;

    [SerializeField] private List<SimulatedComponent> components;
    public List<SimulatedComponent> Components { get { return components; } }

    [SerializeField] private List<SimulatedScript> scripts;
    public List<SimulatedScript> Scripts { get { return scripts; } }

    private readonly Dictionary<System.Type, Component> safeReferences = new();


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
            safeReferences.Add(component.realComponent.GetType(), component.realComponent);
        }
        foreach (SimulatedScript script in scripts)
        {
            script.ParentObject = this;
            componentManager.SetScriptEnabledStatus(script, script.isActiveAndEnabled);
            safeReferences.Add(script.GetType(), script);
        }
    }

    public void OnMouseDown()
    {
        if (interactable)
            inspectorController.DisplayObject(this, defaultSprite, sprite1);
    }

    public void ComponentAdded(SimulatedComponent simulatedComponent)
    {
        components.Add(simulatedComponent);
        safeReferences.Add(simulatedComponent.realComponent.GetType(), simulatedComponent.realComponent);
    }

    public void ScriptAdded(SimulatedScript simulatedScript)
    {
        scripts.Add(simulatedScript);
    }

    // If we have a component of this type, set the passed reference. Otherwise, keep it as null.
    public T TryAssignReference<T>(ref T component) where T : Component
    {
        // Only assign a reference if we don't already have one
        if (component == null)
        {
            // Try to get a reference from the dictionary
            safeReferences.TryGetValue(typeof(T), out Component foundReference);
            component = foundReference as T;
        }

        return component;
    }
}
