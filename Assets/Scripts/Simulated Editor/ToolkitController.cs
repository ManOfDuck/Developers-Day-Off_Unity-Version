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

    private InspectorController inspectorController;

    private VisualElement root;

    private VisualElement toolkitRoot;
    private VisualElement componentArea;
    private List<VisualElement> componentDisplays = new();
    private Dictionary<Toggle, SimulatedComponent> componentAddBindings;

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
        inspectorController = InspectorController.Instance;

        // Disable all components, we dont want them running on this game object
        foreach (SimulatedComponent component in components)
        {
            component.SetComponentEnabledStatus(false);
            AddComponent(component);
        }
    }

    private void OnEnable()
    {
        root = mainUIDocument.rootVisualElement;
        toolkitRoot = root.Q<VisualElement>("Toolkit");
        componentArea = toolkitRoot.Q<VisualElement>("TK_components");
    }

    public void AddComponent(SimulatedComponent component)
    {
        VisualElement componentDisplay = component.GetComponentDisplay(component, componentTemplate);
        AddComponentButton(component, componentDisplay);
        componentDisplays.Add(componentDisplay);
        componentArea.Add(componentDisplay);
    }

    private void AddComponentButton(SimulatedComponent component, VisualElement componentDisplay)
    {
        Button button = componentDisplay.Q<Button>("Button");

        button.clicked += () => component.Copy(inspectorController.displayedObject);
    }
}
