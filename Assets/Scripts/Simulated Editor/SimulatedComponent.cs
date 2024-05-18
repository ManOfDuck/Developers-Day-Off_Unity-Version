using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public abstract class SimulatedComponent : MonoBehaviour
{
    [SerializeField] private VisualComponent visualComponent;
    public VisualComponent VisualComponent { get => visualComponent; set => visualComponent = value; }

    public ComponentHolder Holder => gameObject.GetComponent<ComponentHolder>();

    public abstract Component DirectComponentReference { get; }
    public abstract bool IsComponentToggleable { get; }
    public abstract bool ComponentEnabledStatus { get; }
    protected abstract string DefaultVisualComponentName { get; }

    public abstract SimulatedComponent Copy(ComponentHolder destination);
    public abstract bool ToggleComponent();
    public abstract bool SetComponentEnabledStatus(bool status);

    virtual protected void Start()
    {
        // Check the default component string for validity
        VisualComponent defaultComponent = Resources.Load<ScriptableObject>("Visual Components/" + DefaultVisualComponentName) as VisualComponent;
        if (!defaultComponent)
        {
            Debug.LogError("JESSE: Visual Components/" + DefaultVisualComponentName + " is not a valid Visual Component");
        }

        // Load the default Visual Component for the class if none has been set in the inspector
        if (VisualComponent == null)
        {
            // Do NOT change the filepath please please please please please please please
            VisualComponent = defaultComponent;
        }
        if (Holder != null)
        {
            Holder.RegisterComponent(this);
        }
    }

    public virtual VisualElement GetComponentDisplay(SimulatedComponent component, VisualTreeAsset template)
    {
        VisualComponent visualComponent = component.VisualComponent;

        VisualElement componentDisplay = template.CloneTree();
        VisualElement icon = componentDisplay.Q<VisualElement>("image");
        Label label = componentDisplay.Q<Label>("label");
        Label description = componentDisplay.Q<Label>("desc");

        icon.style.backgroundImage = visualComponent.image.texture;
        label.text = visualComponent.title;
        description.text = visualComponent.description;

        return componentDisplay;
    }

    protected T TryAssignReference<T>(ref T component) where T : Component
    {
        return Holder == null ? null : Holder.TryAssignReference(ref component) as T;
    }

    protected T AssignMandatoryReference<T>(ref T component, System.Type wrapperClass) where T : Component
    {
        return Holder == null ? null : Holder.AssignMandatoryReference(ref component, wrapperClass) as T;
    }
}
