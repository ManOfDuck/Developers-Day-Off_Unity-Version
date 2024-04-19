
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class InspectorController : MonoBehaviour
{
    private static InspectorController _instance;
    public static InspectorController Instance { get { return _instance; } }

    private ComponentManager componentManager;

    public UIDocument mainUIDocument;

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

    public Material highlightMaterial;
    public Material defaultMaterial;

    public SimulatedObject displayedObject;

    private Sprite globalSpriteDefault, globalSprite1;
    private List<VisualElement> componentDisplays = new();
    private List<VisualElement> scriptDisplays = new();
    private Dictionary<Toggle, SimulatedComponent> componentToggleBindings;
    private Dictionary<Toggle, SimulatedScript> scriptToggleBindings;
    private UIDocument currentDisplay;
    private Renderer targetRenderer;


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

    private void Start()
    {
        StopDisplaying();
        componentManager = ComponentManager.Instance;
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

    public void StopDisplaying()
    {
        displayedObject = null;
        if (root != null)
        {
            root.visible = false;
        }
        followCamera.shift = 0;
    }

    void Update()
    {
        if (root.visible == false)
        {
            return;
        }

        foreach (KeyValuePair<Toggle, SimulatedComponent> kvp in componentToggleBindings)
        {
            Toggle toggle = kvp.Key;
            SimulatedComponent component = kvp.Value;
            componentManager.SetComponentEnabledStatus(component, toggle.value);
        }

        foreach(KeyValuePair<Toggle,SimulatedScript> kvp in scriptToggleBindings)
        {
            Toggle toggle = kvp.Key;
            SimulatedScript script = kvp.Value;
            componentManager.SetScriptEnabledStatus(script, toggle.value);
        }

    }

    public void DisplayObject(SimulatedObject objectToDisplay, Sprite noSprite, Sprite sprite)
    {
        // Clear old elements
        while (componentDisplays.Count > 0)
        {
            VisualElement element = componentDisplays[0];
            root.Q<VisualElement>("components").Remove(element);
            componentDisplays.Remove(element);
        }
        while (scriptDisplays.Count > 0)
        {
            VisualElement element = scriptDisplays[0];
            Debug.Log(element);
            root.Q<VisualElement>("scripts").Remove(element);
            scriptDisplays.Remove(element);
        }

        //Clear old bindings
        componentToggleBindings = new();
        scriptToggleBindings = new();

        //Remove the current display
        Display(null);
        try
        {
            targetRenderer.material = defaultMaterial;
        }
        catch
        {
            Debug.Log("we do not have a targetRenderer");
        }

        componentDisplays = new List<VisualElement>();
        scriptDisplays = new List<VisualElement>();
        List<SimulatedComponent> components = objectToDisplay.Components;
        List<SimulatedScript> scripts = objectToDisplay.Scripts;

        targetRenderer = objectToDisplay.GetComponent<Renderer>();
        targetRenderer.material = highlightMaterial;

        // Display the components
        foreach (SimulatedComponent component in components)
        {
            VisualElement componentDisplay = componentManager.GetComponentDisplay(component, componentTemplate);
            AddComponentToggle(component, componentDisplay);
            componentDisplays.Add(componentDisplay);
            root.Q<VisualElement>("components").Add(componentDisplay);
        }

        // Display the scripts as buttons
        foreach (SimulatedScript script in scripts){
            VisualElement scriptDisplay = componentManager.GetScriptDisplay(script, scriptTemplate);
            AddScriptToggle(script, scriptDisplay);
            scriptDisplays.Add(scriptDisplay);
            root.Q<VisualElement>("scripts").Add(scriptDisplay);
        }

        globalSpriteDefault = noSprite;
        globalSprite1 = sprite;

        //SET OBJ NAME & TAG
        objectName.text = objectToDisplay.gameObject.name.ToString();
        objectTag.text = objectToDisplay.gameObject.tag.ToString();

        //Show the inspector
        root.visible = true;
        if (followCamera.controlledCamera.WorldToScreenPoint(objectToDisplay.transform.position).x > shiftDistance)
            followCamera.shift = cameraShiftAmount;

        this.displayedObject = objectToDisplay;
    }

    private void AddComponentToggle(SimulatedComponent component, VisualElement componentDisplay)
    {
        Toggle toggle = componentDisplay.Q<Toggle>("toggle");

        // Not all components are toggleable
        if (componentManager.IsComponentToggleable(component) && toggle is not null)
        {
            toggle.value = componentManager.GetComponentEnabledStatus(component);
            componentToggleBindings.Add(toggle, component);
        }
        else if (toggle is not null)
        {
            toggle.style.opacity = 0;
        }
    }

    private void AddScriptToggle(SimulatedScript script, VisualElement scriptDisplay)
    {
        Toggle toggle = scriptDisplay.Q<Toggle>("toggle");

        toggle.value = script.enabled;
        scriptToggleBindings.Add(toggle, script);
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
