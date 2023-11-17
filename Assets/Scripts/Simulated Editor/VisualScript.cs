using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "newVisualScript", menuName = "createVisualScript")]
public class VisualScript : ScriptableObject
{
    public string title;
    public string description;
    public UIDocument scriptUI;

    protected void lightUpLine(string line)
    {
        //Get the visual element
        //StartCoroutine(lightCoroutine(visualElement))
    }

    private IEnumerator lightCoroutine(VisualElement line)
    {
        float alpha = 255;
        float time = 0;
        //get animation curve
        //while animation curve has value
        // alpha = animationCurve.eval(time)
        // time += deltaTime
        yield return null;
    }
}
