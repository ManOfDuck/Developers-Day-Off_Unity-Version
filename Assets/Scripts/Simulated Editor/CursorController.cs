using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField] private Texture2D normalCursor;
    [SerializeField] private Vector2 normalOffset;

    [SerializeField] private Texture2D hoverCursor;
    [SerializeField] private Vector2 hoverOffset;

    [SerializeField] private Texture2D cantSelectCursor;
    [SerializeField] private Vector2 cantSelectOffset;

    [SerializeField] private int _interactableLayer;
    [SerializeField] private int _noninteractableLayer;

    [SerializeField] private bool _showCursor = true;
    public int InteractableLayer { get => _interactableLayer;}
    public int NoninteractableLayer { get => _noninteractableLayer;}
    public bool ShowCursor { get => _showCursor; set => _showCursor = value; }

    private InputManager inputManager;
    private InspectorController inspectorController;


    // Start is called before the first frame update
    void Start()
    {
        inputManager = InputManager.Instance;
        inspectorController = InspectorController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("hi");
        Collider2D[] interactableHits = Physics2D.OverlapPointAll(inputManager.WorldMousePosition, 1 << InteractableLayer);
        Collider2D[] noninteractableHits = Physics2D.OverlapPointAll(inputManager.WorldMousePosition, 1 << NoninteractableLayer);

        if (interactableHits.Length > 0 && inspectorController.displayedObject == null)
        {
            Cursor.SetCursor(hoverCursor, hoverOffset, CursorMode.ForceSoftware);
        }
        else if (noninteractableHits.Length > 0 && inspectorController.displayedObject == null)
        {
            Cursor.SetCursor(cantSelectCursor, cantSelectOffset, CursorMode.ForceSoftware);
        }
        else
        {
            Cursor.SetCursor(normalCursor, normalOffset, CursorMode.ForceSoftware);
        }

        Cursor.visible = ShowCursor;
    }
}
