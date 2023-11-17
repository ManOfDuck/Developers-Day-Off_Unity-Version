using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using SimulatedComponent = SimulatedObject.SimulatedComponent;

public class inspectorController : MonoBehaviour
{
    private static inspectorController _instance;
    public static inspectorController Instance { get { return _instance; } }

    public VisualTreeAsset componentTemplate;

    private VisualElement root;
    private Label objectName, objectTag;
    private Toggle TRANSTog, SRTog, IMGTog, RB2DTog, BC2DTog;
    private Image test;

    private Sprite globalSpriteDefault, globalSprite1;
    private SimulatedObject currentObject;
    private List<SimulatedComponent> components;
    private List<VisualElement> componentDisplays = new();
    private Dictionary<Toggle, SimulatedComponent> toggleBindings = new();

    VisualElement root;
    Label objectName, objectTag;
    Button script1Button, script2Button, script3Button;


    private void OnEnable() // get ui references B-)
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        objectName = root.Q<Label>("Object_name");
        objectTag = root.Q<Label>("Tag");


        script1Button = root.Q<Button>("Script1_button");
        script2Button = root.Q<Button>("Script2_button");
        script3Button = root.Q<Button>("Script3_button");
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        root.visible = false;
    }

    void Update()
    {
        if (currentObject == null)
        {
            return;
        }

        foreach (KeyValuePair<Toggle, SimulatedComponent> kvp in toggleBindings)
        {
            Toggle toggle = kvp.Key;
            SimulatedComponent component = kvp.Value;
            if (component.realComponent is SpriteRenderer)
            {
                currentObject.GetComponent<SpriteRenderer>().sprite = toggle.value ? globalSprite1 : globalSpriteDefault;
            }
            else
            {
                currentObject.setComponentEnabledStatus(component, toggle.value);
            }
        }

        script1Button.clickable.clicked += () =>
        {
            Debug.Log("Script1 clickecd");
        };
        script2Button.clickable.clicked += () =>
        {
            Debug.Log("Script2 clickecd");
        };
        script3Button.clickable.clicked += () =>
        {
            Debug.Log("Script3 clickecd");
        };

    }

    public void DisplayObject(SimulatedObject obj, Sprite noSprite, Sprite sprite)
    {
        // Clear old elements
        foreach (VisualElement element in componentDisplays)
        {
            root.Remove(element);
        }
        // Clear old bindings
        foreach(KeyValuePair<Toggle, SimulatedComponent> kvp in toggleBindings)
        {
            toggleBindings.Remove(kvp.Key);
        }

        componentDisplays = new List<VisualElement>();
        currentObject = obj;
        components = currentObject.components;

        // Display the components
        foreach (SimulatedComponent c in components)
        {
            VisualElement componentDisplay = getComponentDisplay(c);
            componentDisplays.Add(componentDisplay);
            root.Q<VisualElement>("components").Add(componentDisplay);
        }

        globalSpriteDefault = noSprite;
        globalSprite1 = sprite;

        //SET OBJ NAME & TAG
        objectName.text = obj.gameObject.name.ToString();
        objectTag.text = obj.gameObject.tag.ToString();

        root.visible = true;
    }    

    public VisualElement getComponentDisplay(SimulatedComponent component)
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

        if (currentObject.isComponentToggleable(component))
        {
            toggle.value = currentObject.getComponentEnabledStatus(component);
            toggleBindings.Add(toggle, component);
        }
        else
        {
            toggle.style.opacity = 0;
        }

        return componentDisplay;
    }
        //SHOW SCRIPT BUTTONS
        if (obj.scripts[0] != null)
        {
            script1Button.text = obj.scripts[0].visualScript.name.ToString() + ".cs";
        }
        if (obj.scripts[1] != null)
        {
            script2Button.text = obj.scripts[1].visualScript.name.ToString() + ".cs";
        }
        if (obj.scripts[2] != null)
        {
            script3Button.text = obj.scripts[2].visualScript.name.ToString() + ".cs";
        }

        
        


    }    

        

}
