using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using Component = UnityEngine.Component;

public abstract class ComponentWrapper<T> : SimulatedComponent where T : Component
{
    [SerializeField] T _wrappedComponent;
    public T WrappedComponent { get => _wrappedComponent; set => _wrappedComponent = value; }

    public override Component DirectComponentReference => WrappedComponent;
    public override bool IsComponentToggleable => WrappedComponent is Behaviour || WrappedComponent is Renderer;
    public override bool ComponentEnabledStatus
    {
        get
        {
            switch (WrappedComponent)
            {
                case Behaviour b:
                    return b.isActiveAndEnabled;
                case Renderer r:
                    return r.enabled;
                default:
                    return true;
            }
        }
    }

    public override bool ToggleComponent()
    {
        return SetComponentEnabledStatus(!ComponentEnabledStatus);
    }

    public override bool SetComponentEnabledStatus(bool status)
    {
        switch (WrappedComponent)
        {
            case Behaviour b:
                b.enabled = status;
                return true;
            case Renderer r:
                r.enabled = status;
                return true;
            default:
                return false;
        }
    }

    // Override in child classes
    public abstract void SetDefaultValues();

    virtual public T CreateComponent()
    {
        if (WrappedComponent != null)
        {
            Debug.LogError("JESSE: Component Wrapper cannot create a second component");
        }

        T component = gameObject.AddComponent(typeof(T)) as T;
        WrappedComponent = component;
        SetDefaultValues();
        return component;
    }

    public override SimulatedComponent Copy(ComponentHolder destination)
    {
        ComponentWrapper<T> copiedWrapper = destination.gameObject.AddComponent(this.GetType()) as ComponentWrapper<T>;

        // Get the component's type and public properties
        System.Type componentType = typeof(T);
        BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
        PropertyInfo[] componentProperties = componentType.GetProperties(flags);

        // Discard properties that have no set method or are declared too high up
        IEnumerable<PropertyInfo> settableProperties = from property in componentProperties
                                                       where property.CanWrite
                                                       where property.DeclaringType != typeof(Component)
                                                       where property.DeclaringType != typeof(Object)
                                                       select property;


        // Add a new Component of type T to the destination
        T copiedComponent = copiedWrapper.CreateComponent();

        // Iterate through each property and copy its value to the copied component
        foreach (PropertyInfo property in settableProperties)
        {
            // Get the value of the property from the original component
            object value = property.GetValue(WrappedComponent);

            // Set the value of the property in the added component
            property.SetValue(copiedComponent, value);
        }

        copiedWrapper.SetComponentEnabledStatus(true);
        return copiedWrapper;
    }
}
