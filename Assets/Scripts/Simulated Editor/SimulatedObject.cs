using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UI.Image;

public class SimulatedObject : MonoBehaviour
{
    [SerializeField] private bool interactable = true;
    [SerializeField] private Collider2D clickTrigger;

    public List<SimulatedComponent> _components = new();
    public List<SimulatedComponent> Components { get => _components; private set => _components = value; }
    public LayerMask Layer { get; private set; }

    private readonly Dictionary<System.Type, List<Component>> safeReferences = new();
    private InspectorController inspectorController;
    private GameManager gameManager;
    public Sprite defaultSprite;
    public Sprite sprite1;

    public void Awake()
    {
        
    }

    public void Start()
    {
        inspectorController = InspectorController.Instance;
        gameManager = GameManager.Instance;
        Layer = gameObject.layer;
    }

    public void OnMouseDown()
    {
        if (interactable)
            inspectorController.DisplayObject(this);
    }

    public void RegisterComponent(SimulatedComponent simulatedComponent)
    {
        Components.Add(simulatedComponent);
        if (inspectorController != null)
        {
            inspectorController.RefreshDisplay();
        }

        // Get the new component
        Component newComponent = simulatedComponent.DirectComponentReference;

        // Add the component to the references dictionary
        System.Type componentType = newComponent.GetType();
        if (!safeReferences.ContainsKey(componentType))
        {
            safeReferences[componentType] = new List<Component>();
        }
        safeReferences[componentType].Add(newComponent);
    }

    // If we have a component of this type, set the passed reference. Otherwise, keep it as null.
    public T TryAssignReference<T>(ref T component) where T : Component
    {
        // Only assign a reference if our existing one is null
        if (component == null)
        {
            // Search for a list of this type
            System.Type type = typeof(T);
            if (safeReferences.ContainsKey(type))
            {
                List<Component> referenceList = safeReferences[type];
                Component foundReference = null;
                // Walk through the list until we find a non-null reference
                foreach(Component c in referenceList)
                {
                    if (c != null)
                    {
                        foundReference = c;
                        break;
                    }
                }

                // Remove the null elements
                int foundIndex = referenceList.IndexOf(foundReference);
                referenceList.RemoveRange(0, foundIndex);

                // Assign component to our found reference, which is null if we found nothing
                component = foundReference as T;
            }
        }

        return component;
    }

    public T AssignMandatoryReference<T>(ref T component, System.Type wrapperClass) where T : Component
    {
        // Try to assign the reference
        TryAssignReference(ref component);

        // If it fails, we need to add a new one
        if (component == null)
        {
            // Check to make sure the passed wrapper is valid
            if (wrapperClass.IsSubclassOf(typeof(ComponentWrapper<T>)))
            {
                // Create a wrapper of the passed type
                ComponentWrapper<T> wrapper = gameObject.AddComponent(wrapperClass) as ComponentWrapper<T>;
                // Ask the wrapper to create a component
                component = wrapper.CreateComponent();
            }
            else
            {
                throw new ArgumentException("wrapperClass must inherit from ComponentWrapper<" + typeof(T).Name + ">");
            }
        }

        return component;
    }
}
