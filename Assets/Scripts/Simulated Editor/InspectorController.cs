
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class InspectorController : MonoBehaviour
{
    private static InspectorController _instance;
    public static InspectorController Instance { get { return _instance; } }

    public UIDocument mainUIDocument;

    public VisualTreeAsset componentTemplate;
    [SerializeField] private VisualTreeAsset _vectorFieldTemplate;
    public VisualTreeAsset VectorFieldTemplate => _vectorFieldTemplate;
    [SerializeField] private VisualTreeAsset _floatFieldTemplate;
    public VisualTreeAsset FloatFieldTemplate => _floatFieldTemplate;
    [SerializeField] private bool overrideUIDoc;

    public Color trueColor;
    public Color falseColor;
    public AnimationCurve flashCurve;

    public FollowCamera followCamera;
    public float cameraShiftAmount;
    public float shiftDistance;

    private VisualElement root;
    private Button xButton, notInspector, alsoNotInspector;
    private Label objectName, xValue, yValue;
    private VisualElement icon;

    public Material highlightMaterial;
    public Material defaultMaterial;

    public SimulatedObject displayedObject;
    private int displayedObjectLayer;

    [SerializeField] private Sprite defaultSprite;
    private List<VisualElement> componentDisplays = new();
    private Dictionary<(VisualElement, Button), SimulatedComponent> toggleBindings = new();
    private Dictionary<TextField, (SimulatedComponent, PropertyInfo)> floatBindings = new();
    private Dictionary<(TextField, TextField), (SimulatedComponent, PropertyInfo)> vectorArrayBindings = new();
    private Dictionary<TextField, string> lastFrameFieldValues = new();
    private UIDocument currentDisplay;
    private Renderer targetRenderer;

    public bool objectHasBeenClicked;
    public bool isDisplaying;

    private GameManager gameManager;
    private InputManager inputManager;

    [Header("Audio")]
    [SerializeField] AudioSource toggleComponentSound;
    [SerializeField] AudioSource addComponentSound;
    [SerializeField] AudioSource inspectorOpenSound;
    [SerializeField] AudioSource inspectorCloseSound;
    //[SerializeField] AudioSource misclickSoundSound; // Cant do here

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if (overrideUIDoc)
            {
                Destroy(Instance.gameObject);
                _instance = this;
            }
            else Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.OnPlayerHurt.AddListener(StopDisplaying);
        StopDisplaying();
    }

    private void OnEnable() // get ui references B-)
    {
        root = mainUIDocument.rootVisualElement;
        icon = root.Q<VisualElement>("image");
        objectName = root.Q<Label>("Object_name");
        xValue = root.Q<Label>("x_value");
        yValue = root.Q<Label>("y_value");
        xButton = root.Q<Button>("x_button");
        notInspector = root.Q<Button>("not_inspector");
        alsoNotInspector = root.Q<Button>("also_not_inspector");
        xButton.clickable.clicked += () =>
        {
            StopDisplaying();
        };
        notInspector.clickable.clicked += () =>
        {
            if (Camera.main.GetComponentInChildren<SpriteRenderer>())
            {
                StopDisplaying();
            }
        };
        alsoNotInspector.clickable.clicked += () =>
        {
            if (Camera.main.GetComponentInChildren<SpriteRenderer>())
            {
                StopDisplaying();
            }
        };
    }

    public void StopDisplaying()
    {
        Unfocus();

        displayedObject = null;

        try
        {
            targetRenderer.material = defaultMaterial;
        }
        catch
        {
            Debug.Log("there is no target renderer");
        }


        if (root != null)
        {
            if (root.visible)
            {
                inspectorCloseSound.Play();
            }
            root.visible = false;
        }
        if (followCamera != null)
        {
            followCamera.shift = 0;
        }
    }

    public void RefreshDisplay()
    {
        if (!displayedObject) return;
        Unfocus();
        DisplayObject(displayedObject, false);
        addComponentSound.Play(); // Not exactly the right spot for this, but it works well enough
    }

    private void Unfocus()
    {
        if (displayedObject != null)
        {
            SpriteRenderer greyBox = Camera.main.GetComponentInChildren<SpriteRenderer>();
            if (greyBox)
            {
                Debug.Log(targetRenderer);
                greyBox.enabled = false;
                if (displayedObject.GetComponent<Renderer>() && displayedObject.GetComponent<Renderer>().sortingOrder > 100)
                    displayedObject.GetComponent<Renderer>().sortingOrder -= 1000;
                if (PlayerSpawn.Player?.GetComponent<Renderer>() && PlayerSpawn.Player?.GetComponent<Renderer>().sortingOrder > 100)
                    PlayerSpawn.Player.GetComponent<Renderer>().sortingOrder -= 1000;
            }
            else
            {
                if (targetRenderer) targetRenderer.material = defaultMaterial;
            }
        }
    }

    void Update()
    {
        if (root.visible == false)
        {
            return;
        }

        /*
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Debug.Log("mouse clicked");
            root.style.visibility = Visibility.Hidden;
            targetRenderer.material = defaultMaterial;
        }
        */

        if (displayedObject)
        {
            xValue.text = displayedObject.gameObject.transform.position.x.ToString("F");
            yValue.text = displayedObject.gameObject.transform.position.y.ToString("F");
        }

        #region Kindly Ignore :)
        // Hello welcome to my code
        foreach (KeyValuePair<TextField, (SimulatedComponent, PropertyInfo)> kvp in floatBindings)
        {
            TextField field = kvp.Key;

            if (!lastFrameFieldValues.ContainsKey(field) || lastFrameFieldValues[field] != field.value)
            {
                SimulatedComponent component = kvp.Value.Item1;
                PropertyInfo property = kvp.Value.Item2;

                try
                {
                    float value = field.value == "" ? 0 : float.Parse(field.value);
                    property.SetValue(component, value);
                    lastFrameFieldValues[field] = field.value;
                }
                catch (System.Exception)
                {
                    if (field.value != "-")
                        field.value = lastFrameFieldValues[field];
                }

                Debug.Log(field.value);

            }
        }

        // Hello welcome to my code (2)
        foreach (KeyValuePair<(TextField, TextField), (SimulatedComponent, PropertyInfo)> kvp in vectorArrayBindings)
        {
            TextField xField = kvp.Key.Item1;
            TextField yField = kvp.Key.Item2;

            if (!lastFrameFieldValues.ContainsKey(xField) || lastFrameFieldValues[xField] != xField.value)
            {
                SimulatedComponent component = kvp.Value.Item1;
                PropertyInfo property = kvp.Value.Item2;

                try
                {
                    float value = float.Parse(xField.value);
                    lastFrameFieldValues[xField] = xField.value;
                    property.SetValue(component, new List<Vector2> { new Vector2(value, (property.GetValue(component) as List<Vector2>)[0].y) });
                }
                catch (System.Exception)
                {
                    // Allowed temp values
                    if (xField.value != "" && xField.value != "-")
                        xField.value = lastFrameFieldValues[xField];
                }

                Debug.Log("X " + xField.value);
            }

            // TODO add more copy + pasted code
            if (!lastFrameFieldValues.ContainsKey(yField) || lastFrameFieldValues[yField] != yField.value)
            {
                SimulatedComponent component = kvp.Value.Item1;
                PropertyInfo property = kvp.Value.Item2;

                try
                {
                    float value = float.Parse(yField.value);
                    lastFrameFieldValues[yField] = yField.value;
                    property.SetValue(component, new List<Vector2> { new Vector2((property.GetValue(component) as List<Vector2>)[0].x, value) });
                }
                catch (System.Exception)
                {
                    // Allowed temp values
                    if (yField.value != "" && yField.value != "-")
                        yField.value = lastFrameFieldValues[yField];
                }

                Debug.Log(yField.value);
            }

        }
        #endregion
    }


    public void DisplayObject(SimulatedObject objectToDisplay, bool playSound = true)
    {
        this.displayedObject = objectToDisplay;

        if (playSound) inspectorOpenSound.Play();
        objectHasBeenClicked = true;
        // Clear old elements
        while (componentDisplays.Count > 0)
        {
            VisualElement element = componentDisplays[0];
            root.Q<VisualElement>("components").Remove(element);
            componentDisplays.Remove(element);
        }

        //Clear old bindings
        toggleBindings = new();
        floatBindings = new();
        vectorArrayBindings = new();

        //Remove existing highlight, if any
        if (targetRenderer)
        {
            targetRenderer.material = defaultMaterial;
        }


        componentDisplays = new List<VisualElement>();
        List<SimulatedComponent> components = objectToDisplay.Components;

        // Display the components
        foreach (SimulatedComponent component in components)
        {
            if (component is Rigidbody2DWrapper) continue;

            VisualElement componentDisplay = component.GetComponentDisplay(component, componentTemplate);
            AddComponentToggle(component, componentDisplay);
            if (gameManager && gameManager.Upgrades.Contains("Fields"))
            {
                AddComponentFields(component, componentDisplay);
            }
            componentDisplays.Add(componentDisplay);
            root.Q<VisualElement>("components").Add(componentDisplay);
        }

        SpriteRenderer greyBox = Camera.main.GetComponentInChildren<SpriteRenderer>();
        if (greyBox)
        {
            greyBox.enabled = true;
        }
        else
        {
            targetRenderer = objectToDisplay.GetComponent<Renderer>();
            if (targetRenderer) targetRenderer.material = highlightMaterial;
        }

        if (PlayerSpawn.Player) PlayerSpawn.Player.GetComponent<Renderer>().sortingOrder += 1000;
        if (objectToDisplay.TryGetComponent(out Renderer renderer)) renderer.sortingOrder += 1000;

        //SET OBJ NAME & IMG
        if (objectToDisplay.TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            icon.style.backgroundImage = new StyleBackground(spriteRenderer.sprite);
            icon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
        }
        else
        {
            icon.style.backgroundImage = defaultSprite.texture;
        }
        objectName.text = objectToDisplay.gameObject.name.ToString();

        //Show the inspector
        root.visible = true;
        // if (followCamera.controlledCamera.WorldToScreenPoint(objectToDisplay.transform.position).x > shiftDistance)
        //    followCamera.shift = cameraShiftAmount;
    }

    private void AddComponentToggle(SimulatedComponent component, VisualElement componentDisplay)
    {
        VisualElement toggle = componentDisplay.Q<VisualElement>("Toggle");

        // If there is no toggle, do nothing
        if (toggle is null) return;

        // If the component isn't toggleable, turn the toggle off
        if (!component.IsComponentToggleable)
        {
            toggle.style.opacity = 0;
            return;
        }

        // Get the button and ball and set up their class lists
        Button toggleBG = toggle.Q<Button>("Toggle_BG");
        Button toggleBall = toggle.Q<Button>("Toggle_Ball");

        if (component.ComponentEnabledStatus)
        {
            toggleBall.RemoveFromClassList("togball");
            toggleBG.RemoveFromClassList("togbg");
            toggleBall.AddToClassList("togballchecked");
            toggleBG.AddToClassList("togbgchecked");
        }
        else
        {
            toggleBall.RemoveFromClassList("togballchecked");
            toggleBG.RemoveFromClassList("togbgchecked");
            toggleBall.AddToClassList("togball");
            toggleBG.AddToClassList("togbg");
        }

        toggleBindings.Add((toggleBG, toggleBall), component);

        toggleBG.clicked += () =>
        {
            ToggleClicked(toggleBG, toggleBall);
        };

        toggleBall.clicked += () =>
        {
            ToggleClicked(toggleBG, toggleBall);
        };
    }

    private void ToggleClicked(VisualElement toggleBG, Button toggleBall)
    {
        toggleComponentSound.Play();
        toggleBindings[(toggleBG, toggleBall)].ToggleComponent();
        bool enabled = toggleBindings[(toggleBG, toggleBall)].ComponentEnabledStatus;
        if (!enabled)
        { //deactivate
            toggleBall.RemoveFromClassList("togballchecked");
            toggleBG.RemoveFromClassList("togbgchecked");
            toggleBall.AddToClassList("togball");
            toggleBG.AddToClassList("togbg");
        }
        else
        { //activate
            toggleBall.RemoveFromClassList("togball");
            toggleBG.RemoveFromClassList("togbg");
            toggleBall.AddToClassList("togballchecked");
            toggleBG.AddToClassList("togbgchecked");

            //could also easily change component bg to be darker to easily signify disabled
        }
    }

    #region Ignore this also
    private void AddComponentFields(SimulatedComponent component, VisualElement componentDisplay)
    {
        VisualElement fieldSpace = componentDisplay.Q<VisualElement>("Fields");
        if (fieldSpace == null) return;
        if (component is not SimulatedScript) return;

        BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
        PropertyInfo[] componentProperties = component.GetType().GetProperties(flags);

        foreach (PropertyInfo property in componentProperties)
        {
            // Kindly ignore
            if (property.PropertyType == typeof(List<Vector2>))
            {
                VisualElement vectorField = InspectorController.Instance.VectorFieldTemplate.CloneTree();
                fieldSpace.Add(vectorField);

                TextField XField = vectorField.Q<TextField>("X");
                XField.value = (property.GetValue(component) as List<Vector2>)[0].x.ToString();
                TextField YField = vectorField.Q<TextField>("Y");
                YField.value = (property.GetValue(component) as List<Vector2>)[0].y.ToString();
                vectorArrayBindings.Add((XField, YField), (component, property));
            }
            if (property.PropertyType == typeof(float))
            {
                VisualElement floatField = InspectorController.Instance.FloatFieldTemplate.CloneTree();
                fieldSpace.Add(floatField);

                TextField input = floatField.Q<TextField>("input");

                input.value = property.GetValue(component).ToString();
                input.label = property.Name;
                floatBindings.Add(input, (component, property));
            }
        }
    }
    #endregion

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

    /*public void OnClick()
    {
        Debug.Log("onclickcalled");
        //wait a moment
        Invoke("CheckNothingClicked", 0);
    }

    private void CheckNothingClicked()
    {
        Debug.Log("checking nothing clicked");

        if (objectHasBeenClicked == false)
        {
            Debug.Log("close display");
            StopDisplaying();
        }
        else
        {
            Debug.Log("keep display");
        }
        objectHasBeenClicked = false;
    }
    */
}
