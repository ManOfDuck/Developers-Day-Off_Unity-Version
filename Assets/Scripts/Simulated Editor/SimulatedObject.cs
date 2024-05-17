using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UI.Image;

public class SimulatedObject : ComponentHolder
{
    [SerializeField] private bool _interactable = true;
    [SerializeField] private Collider2D clickTrigger;
    public bool Interactable { get => _interactable; set => _interactable = value; }



    public LayerMask Layer { get; private set; }
    public Collider2D ClickTrigger { get => clickTrigger; private set => clickTrigger = value; }

    private readonly Dictionary<System.Type, List<Component>> safeReferences = new();
    private InspectorController inspectorController;
    private InputManager inputManager;
    public Sprite defaultSprite;
    public Sprite sprite1;

    public void Awake()
    {
        
    }

    protected override void Start()
    {
        base.Start();

        inspectorController = InspectorController.Instance;
        Layer = gameObject.layer;
        inputManager = InputManager.Instance;
        inputManager.OnClick.AddListener(OnClick);

        AlignZAxis();
    }

    public void OnClick()
    {
        if (!Interactable || ClickTrigger == null ||
            (InspectorController.Instance?.displayedObject != null && Camera.main.GetComponentInChildren<SpriteRenderer>() != null) ) return;

        if (ClickTrigger.bounds.Contains(inputManager.WorldMousePosition))
        {
            inspectorController?.DisplayObject(this);
        }
    }

    public override void RegisterComponent(SimulatedComponent simulatedComponent)
    {
        base.RegisterComponent(simulatedComponent);

        if (inspectorController != null  && inspectorController.displayedObject == this)
        {
            inspectorController.RefreshDisplay();
        }

        // Get the new component
        Component newComponent = simulatedComponent.DirectComponentReference;
        if (newComponent == null)
        {
            Debug.LogError(simulatedComponent + " DCR is null!");
            return;
        }

        // Add the component to the references dictionary
        System.Type componentType = newComponent.GetType();
        if (!safeReferences.ContainsKey(componentType))
        {
            safeReferences[componentType] = new List<Component>();
        }
        safeReferences[componentType].Add(newComponent);
    }

    public void AlignZAxis()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    // If we have a component of this type, set the passed reference. Otherwise, keep it as null.
    public override T TryAssignReference<T>(ref T component)
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

    public override T AssignMandatoryReference<T>(ref T component, System.Type wrapperClass)
    {
        // Try to assign the reference
        TryAssignReference(ref component);

        // If it fails, try a direct search (its possible it exists but hasn't registered itself yet)
        if (component == null) {
            ComponentWrapper<T> wrapper = gameObject.GetComponent(wrapperClass) as ComponentWrapper<T>;
            if (wrapper != null)
            {
                component = wrapper.DirectComponentReference as T;
            }
        }

        // If that fails, we need to add a new one
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
