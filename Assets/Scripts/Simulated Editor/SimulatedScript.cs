using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

public abstract class SimulatedScript : SimulatedComponent
{
    public override Component DirectComponentReference => this;
    public override bool IsComponentToggleable => true;
    public override bool ComponentEnabledStatus => enabled;

    private UIDocument scriptUI;

    private VisualElement root;
    private Button xButton;

    private readonly Dictionary<VisualElement, IEnumerator> lightCoroutines = new();

    protected bool DoCoroutines => enabled;
    protected bool doCollisionEvents = true;

    public override bool ToggleComponent()
    {
        return SetComponentEnabledStatus(!ComponentEnabledStatus);
    }

    public override bool SetComponentEnabledStatus(bool status)
    {
        if (IsComponentToggleable) enabled = status;
        return IsComponentToggleable;
    }

    // Returns true if all passed references are valid, false otherwise
    protected bool ValidateReferences(params Component[] components)
    {
        foreach (Component c in components)
        {
            if (c == null)
            {
                return false;
            }
        }

        // TODO: Modify this code to support a simulated stack-trace

        return true;
    }

    protected void Light(int lineNumber)
    {
        Light(lineNumber, Color.white);
    }

    protected void Light(int lineNumber, Color color)
    {
        string lineName = "Line" + lineNumber;
        VisualElement line = root?.Q<VisualElement>(lineName);
        if (line is null) return;
        if (lightCoroutines.ContainsKey(line))
        {
            StopCoroutine(lightCoroutines[line]);
            lightCoroutines.Remove(line);
        }
        lightCoroutines.Add(line, LightCoroutine(line, color));
        StartCoroutine(lightCoroutines[line]);
    }

    private IEnumerator LightCoroutine(VisualElement line, Color color)
    {
        float time = 0;
        float a = 100;
        while (a > 0)
        {
            float r = color.r;
            float g = color.g;
            float b = color.b;
            a = InspectorController.Instance.flashCurve.Evaluate(time);

            line.style.backgroundColor = new Color(r, g, b, a);
            time += Time.deltaTime;
            yield return null;
        }
    }

    /*
    public UIDocument GetUIDoc(PanelSettings panelSettings)
    {
        scriptUI = gameObject.GetComponent<UIDocument>();
        if (!scriptUI)
        {
            scriptUI = gameObject.AddComponent<UIDocument>();
        }

        scriptUI.visualTreeAsset = visualScript.UI;
        scriptUI.panelSettings = panelSettings;
        root = scriptUI.rootVisualElement;

        xButton = root.Q<Button>("x_button");
        xButton.clickable.clicked += () =>
        {
            root.visible = false;
        };

        return scriptUI;
    }
    */

}