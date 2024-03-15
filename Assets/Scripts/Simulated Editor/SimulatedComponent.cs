using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SimulatedComponent
{
    public SimulatedObject parentObject;
    public Component realComponent;
    public VisualComponent visualComponent;
}
