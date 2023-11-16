using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatedObject : MonoBehaviour
{
    public List<SimulatedComponent> components;
    public List<SimulatedScript> scripts;
    private inspectorController controller;

    [System.Serializable]
    public class SimulatedComponent
    {
        public Component realComponent;
        public VisualComponent visualComponent;
    }

    public bool isComponentToggleable(SimulatedComponent component)
    {
        return component.realComponent as Behaviour is not null;
    }

    public bool getComponentEnabledStatus (SimulatedComponent component)
    {
        return !isComponentToggleable(component) || (component.realComponent as Behaviour).enabled;
    }

    public void setComponentEnabledStatus(SimulatedComponent component, bool enabled)
    {
        if (isComponentToggleable(component))
        {
            (component.realComponent as Behaviour).enabled = enabled;
        }        
    }

    public void toggleComponent(SimulatedComponent component)
    {
        setComponentEnabledStatus(component, !getComponentEnabledStatus(component));
    }


    public void setScriptEnabledStatus(SimulatedScript script, bool enabled)
    {
        script.enabled = enabled;
    }

    public void toggleScript(SimulatedScript script)
    {
        setScriptEnabledStatus(script, !script.enabled);
    }

   
}
