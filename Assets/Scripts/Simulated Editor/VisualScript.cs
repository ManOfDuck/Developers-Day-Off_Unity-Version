using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newVisualScript", menuName = "createVisualScript")]
public class VisualScript : ScriptableObject
{
    public string title;
    public string description;
    public List<string> lines;
}
