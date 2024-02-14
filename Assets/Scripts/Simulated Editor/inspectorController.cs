using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using SimulatedComponent = SimulatedObject.SimulatedComponent;

public class InspectorController : MonoBehaviour
{
    private static InspectorController _instance;
    public static InspectorController Instance { get { return _instance; } }

    public UIDocument mainUIDocument;
    public PanelSettings panelSettings;

    public string lineOfSightBlockingTag;

    public VisualTreeAsset componentTemplate;
    public VisualTreeAsset scriptTemplate;

    public Color trueColor;
    public Color falseColor;
    public AnimationCurve flashCurve;

    public FollowCamera followCamera;
    public float cameraShiftAmount;
    public float shiftDistance;

    private VisualElement root;
    private Button xButton;
    private Label objectName, objectTag;

    private Sprite globalSpriteDefault, globalSprite1;
    private SimulatedObject currentObject;
    private List<VisualElement> componentDisplays = new();
    private List<VisualElement> scriptDisplays = new();
    private Dictionary<Toggle, SimulatedComponent> componentToggleBindings;
    private Dictionary<Toggle, SimulatedScript> scriptToggleBindings;
    private UIDocument currentDisplay;


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(_instance.gameObject);
            _instance = this;
        }
    }
    
    private void OnEnable() // get ui references B-)
    {
        root = mainUIDocument.rootVisualElement;
        objectName = root.Q<Label>("Object_name");
        objectTag = root.Q<Label>("Tag");
        xButton = root.Q<Button>("x_button");
        xButton.clickable.clicked += () =>
        {
            StopDisplaying();
        };
    }

    void Start()
    {
        StopDisplaying();
    }

    public void StopDisplaying()
    {
        if (root != null)
        {
            root.visible = false;
        }
        followCamera.shift = 0;
    }

    void Update()
    {
        if (currentObject == null)
        {
            return;
        }

        foreach (KeyValuePair<Toggle, SimulatedComponent> kvp in componentToggleBindings)
        {
            Toggle toggle = kvp.Key;
            SimulatedComponent component = kvp.Value;
            currentObject.SetComponentEnabledStatus(component, toggle.value);
        }

        foreach(KeyValuePair<Toggle,SimulatedScript> kvp in scriptToggleBindings)
        {
            Toggle toggle = kvp.Key;
            SimulatedScript script = kvp.Value;
            currentObject.SetScriptEnabledStatus(script, toggle.value);
        }

    }

    public void DisplayObject(SimulatedObject obj, Sprite noSprite, Sprite sprite)
    {
        Debug.Log("1");
        // Clear old elements
        while (componentDisplays.Count > 0)
        {
            VisualElement element = componentDisplays[0];
            root.Q<VisualElement>("components").Remove(element);
            componentDisplays.Remove(element);
        }
        while (scriptDisplays.Count > 0)
        {
            Debug.Log("2");
            VisualElement element = scriptDisplays[0];
            Debug.Log(element);
            root.Q<VisualElement>("scripts").Remove(element);
            scriptDisplays.Remove(element);
        }
        Debug.Log("3");

        //Clear old bindings
        componentToggleBindings = new();
        scriptToggleBindings = new();

        //Remove the current display
        Display(null);

        componentDisplays = new List<VisualElement>();
        scriptDisplays = new List<VisualElement>();
        this.currentObject = obj;
        List<SimulatedComponent> components = currentObject.components;
        List<SimulatedScript> scripts = currentObject.scripts;

        // Display the components
        foreach (SimulatedComponent component in components)
        {
            VisualElement componentDisplay = getComponentDisplay(component);
            componentDisplays.Add(componentDisplay);
            root.Q<VisualElement>("components").Add(componentDisplay);
        }

        // Display the scripts as buttons
        foreach (SimulatedScript script in scripts){
            VisualElement scriptDisplay = GetScriptDisplay(script);
            scriptDisplays.Add(scriptDisplay);
            root.Q<VisualElement>("scripts").Add(scriptDisplay);
        }

        globalSpriteDefault = noSprite;
        globalSprite1 = sprite;

        //SET OBJ NAME & TAG
        objectName.text = obj.gameObject.name.ToString();
        objectTag.text = obj.gameObject.tag.ToString();

        //Show the inspector
        root.visible = true;
        if (followCamera.controlledCamera.WorldToScreenPoint(obj.transform.position).x > shiftDistance)
            followCamera.shift = cameraShiftAmount;
    }    

    private VisualElement getComponentDisplay(SimulatedComponent component)
    {
        VisualComponent visualComponent = component.visualComponent;

        VisualElement componentDisplay = componentTemplate.CloneTree();
        VisualElement icon = componentDisplay.Q<VisualElement>("image");
        Label label = componentDisplay.Q<Label>("label");
        Label description = componentDisplay.Q<Label>("desc");
        Toggle toggle = componentDisplay.Q<Toggle>("toggle");

        Debug.Log(icon);
        Debug.Log(visualComponent.image);

        icon.style.backgroundImage = visualComponent.image.texture;
        label.text = visualComponent.title;
        description.text = visualComponent.description;

        if (currentObject.IsComponentToggleable(component))
        {
            toggle.value = currentObject.GetComponentEnabledStatus(component);
            componentToggleBindings.Add(toggle, component);
        }
        else
        {
            toggle.style.opacity = 0;
        }

        return componentDisplay;
    }
    private VisualElement GetScriptDisplay(SimulatedScript script)
    {
        VisualElement scriptDisplay = scriptTemplate.CloneTree();

        Button button = scriptDisplay.Q<Button>("button");
        Toggle toggle = scriptDisplay.Q<Toggle>("toggle");

        button.text = script.visualScript.title + ".cs";
        button.clickable.clicked += () =>
        {
            Display(script.GetUIDoc(panelSettings));
        };

        toggle.value = script.enabled;
        scriptToggleBindings.Add(toggle, script);

        return scriptDisplay;
    }

    private void Display(UIDocument displayedUI)
    {
        if (currentDisplay)
        {
            currentDisplay.rootVisualElement.visible = false;
        }

        currentDisplay = displayedUI;
        if (displayedUI)
        {
            displayedUI.rootVisualElement.visible = true;
        }
    }
}
