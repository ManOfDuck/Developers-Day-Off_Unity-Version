using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ToolkitController : ComponentHolder
{
    public static ToolkitController Instance { get; private set; }
    

    [SerializeField] protected UIDocument mainUIDocument;
    [SerializeField] private VisualTreeAsset componentTemplate;
    [SerializeField] private bool _overrideComponents = false;
    public bool OverrideComponents { get => _overrideComponents; }

    private InspectorController inspectorController;

    private VisualElement root;
    private VisualElement toolkitRoot;
    private VisualElement componentArea;
    private readonly List<VisualElement> componentDisplays = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if (OverrideComponents)
            {
                StartCoroutine(DelayDestroy());
            }
            else
            {
                Destroy(this.gameObject);
            }

        }
    }

    private IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(1);
        foreach (SimulatedComponent c in Components)
        {
            c.Copy(Instance);
        }
        Destroy(this.gameObject);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        inspectorController = InspectorController.Instance;
    }

    public override void RegisterComponent(SimulatedComponent simulatedComponent)
    {
        base.RegisterComponent(simulatedComponent);
        AddComponent(simulatedComponent);
        simulatedComponent.enabled = false; 
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
