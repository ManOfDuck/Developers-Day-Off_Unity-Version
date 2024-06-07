using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ComponentHolder : MonoBehaviour
{
    public List<SimulatedComponent> _components = new();
    public List<SimulatedComponent> Components { get => _components; private set => _components = value; }
    private GameManager gameManager;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        gameManager = GameManager.Instance;
    }

    public virtual void RegisterComponent(SimulatedComponent simulatedComponent)
    {
        Components.Add(simulatedComponent);
    }

    public virtual T TryAssignReference<T>(ref T component) where T : Component
    {
        return component;
    }

    public virtual T AssignMandatoryReference<T>(ref T component, System.Type wrapperClass) where T : Component
    {
        return component;
    }

    public virtual bool RequestBody(SimulatedScript requestingScript)
    {
        return false;
    }

    public virtual bool LockBody(SimulatedScript newOwner)
    {
        return false;
    }

    public virtual void ReleaseBody(SimulatedScript currentOwner)
    {
        return;
    }
}
