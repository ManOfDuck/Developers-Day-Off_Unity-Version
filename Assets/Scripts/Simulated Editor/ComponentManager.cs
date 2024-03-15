using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SimulatedObject;
using UnityEngine.UIElements;

public class ComponentManager : MonoBehaviour
{
    public static ComponentManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public VisualElement GetComponentDisplay(SimulatedComponent component, VisualTreeAsset template)
    {
        VisualComponent visualComponent = component.visualComponent;

        VisualElement componentDisplay = template.CloneTree();
        VisualElement icon = componentDisplay.Q<VisualElement>("image");
        Label label = componentDisplay.Q<Label>("label");
        Label description = componentDisplay.Q<Label>("desc");
 
        icon.style.backgroundImage = visualComponent.image.texture;
        label.text = visualComponent.title;
        description.text = visualComponent.description;

        return componentDisplay;
    }
    public VisualElement GetScriptDisplay(SimulatedScript script, VisualTreeAsset template)
    {
        VisualElement scriptDisplay = template.CloneTree();

        Button button = scriptDisplay.Q<Button>("button");

        button.text = script.visualScript.title + ".cs";
        // This will need a rework to work for both toolkit and inspector, for now we'll disable it to avoid confusion
        /*
        button.clickable.clicked += () =>
        {
            Display(script.GetUIDoc(panelSettings));
        };
        */

        return scriptDisplay;
    }
}
