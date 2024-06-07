using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
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

    //for toolkit slidee
    private Button tkSpacer;
    Vector2 pointerScreenPos, pointerUiPos;

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
                Destroy(Instance.gameObject);
                Instance = this;
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
        tkSpacer = root.Q<Button>("also_not_inspector");
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

    private void Update()
    {
        //open tk on hover
        pointerScreenPos = Input.mousePosition;
        pointerUiPos = new Vector2 { x = pointerScreenPos.x, y = Screen.height - pointerScreenPos.y };

        //Debug.Log("picked : " + mainUIDocument.rootVisualElement.panel.Pick(pointerUiPos)); //this is so evil unity why would u make me do this

        if (mainUIDocument.rootVisualElement.panel.Pick(pointerUiPos) == toolkitRoot)
        {
            //open the TK
            tkSpacer.RemoveFromClassList("tk_spacer_closed");
            tkSpacer.AddToClassList("tk_spacer_open");
            tkSpacer.RemoveFromClassList("tk_spacer_hidden");
        }
        else
        {
            //close the TK
            tkSpacer.RemoveFromClassList("tk_spacer_open");
            tkSpacer.AddToClassList("tk_spacer_closed");
            tkSpacer.RemoveFromClassList("tk_spacer_hidden");
        }
    }
}
