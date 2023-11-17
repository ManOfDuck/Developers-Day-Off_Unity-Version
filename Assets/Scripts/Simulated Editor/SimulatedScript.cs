using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

public class SimulatedScript : MonoBehaviour
{
    public VisualScript visualScript;
    private UIDocument scriptUI;

    private VisualElement root;
    private Button xButton;

    private Dictionary<VisualElement, IEnumerator> lightCoroutines = new();

    protected void Light(int lineNumber)
    {
        string lineName = "Line" + lineNumber;
        VisualElement line = root?.Q<VisualElement>(lineName);
        if (line is null) return;
        if (lightCoroutines.ContainsKey(line))
        {
            StopCoroutine(lightCoroutines[line]);
            lightCoroutines.Remove(line);
        }
        lightCoroutines.Add(line, LightCoroutine(line, Color.white));
        StartCoroutine(lightCoroutines[line]);
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
            a = inspectorController.Instance.flashCurve.Evaluate(time);

            line.style.backgroundColor = new Color(r, g, b, a);
            time += Time.deltaTime;
            yield return null;
        }
    }


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

}