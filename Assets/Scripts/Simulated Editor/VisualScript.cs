using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "newVisualScript", menuName = "createVisualScript")]
public class VisualScript : ScriptableObject
{
    public string title;
    public string description;
    [SerializeField] public VisualTreeAsset UI;
}

