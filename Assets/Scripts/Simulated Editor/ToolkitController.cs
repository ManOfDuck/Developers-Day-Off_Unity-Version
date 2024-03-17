using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ToolkitController : MonoBehaviour
{
    public static ToolkitController Instance { get; private set; }

    public UIDocument mainUIDocument;

    public VisualTreeAsset componentTemplate;
    public VisualTreeAsset scriptTemplate;

    public List<SimulatedComponent> components;
    public List<SimulatedScript> scripts;

    private ComponentManager componentManager;
    private InspectorController inspectorController;

    private VisualElement root;

    private VisualElement toolkitRoot;
    private VisualElement componentArea;
    private List<VisualElement> componentDisplays = new();
    private List<VisualElement> scriptDisplays = new();
    private Dictionary<Toggle, SimulatedComponent> componentAddBindings;
    private Dictionary<Toggle, SimulatedScript> scriptAddBindings;

    private void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        componentManager = ComponentManager.Instance;
        inspectorController = InspectorController.Instance;

        // Disable all components and scripts, we dont want them running on this game object
        foreach (SimulatedComponent component in components)
        {
            componentManager.SetComponentEnabledStatus(component, false);
        }
        foreach (SimulatedScript script in scripts)
        {
            componentManager.SetScriptEnabledStatus(script, false);
        }
    }

    private void OnEnable()
    {
        root = mainUIDocument.rootVisualElement;
        toolkitRoot = root.Q<VisualElement>("Toolkit");
        componentArea = toolkitRoot.Q<VisualElement>("TK_components");
    }

    // Update is called once per frame
    void Update()
    {
        foreach (SimulatedComponent component in components)
        {
            VisualElement componentDisplay = componentManager.GetComponentDisplay(component, componentTemplate);
            AddComponentButton(component, componentDisplay);
            componentDisplays.Add(componentDisplay);
            root.Q<VisualElement>("components").Add(componentDisplay);
        }

        // Display the scripts as buttons
        foreach (SimulatedScript script in scripts)
        {
            VisualElement scriptDisplay = componentManager.GetScriptDisplay(script, scriptTemplate);
            AddScriptButton(script, scriptDisplay);
            scriptDisplays.Add(scriptDisplay);
            root.Q<VisualElement>("scripts").Add(scriptDisplay);
        }
    }

    private void AddComponentButton(SimulatedComponent component, VisualElement componentDisplay)
    {
        Button button = componentDisplay.Q<Button>("Button");

        button.clicked += () => componentManager.AddSimulatedComponentToObject(component, inspectorController.displayedObject);
    }

    private void AddScriptButton(SimulatedScript script, VisualElement sciptDisplay)
    {
        Button button = sciptDisplay.Q<Button>("Button");

        button.clicked += () => componentManager.AddSimulatedScriptToObject(script, inspectorController.displayedObject);
    }
}
