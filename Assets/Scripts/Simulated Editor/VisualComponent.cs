using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newVisualComponent", menuName = "createVisualComponent")]
public class VisualComponent : ScriptableObject
{
    public string title;
    public Sprite image;
    public string description;
}
