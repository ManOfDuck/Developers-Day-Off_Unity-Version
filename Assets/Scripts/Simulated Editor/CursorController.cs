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
    public int InteractableLayer { get => _interactableLayer;}
    public int NoninteractableLayer { get => _noninteractableLayer;}

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
        Collider2D[] interactableHits = Physics2D.OverlapPointAll(inputManager.WorldMousePosition, 1 << InteractableLayer);
        Collider2D[] noninteractableHits = Physics2D.OverlapPointAll(inputManager.WorldMousePosition, 1 << NoninteractableLayer);

        if (interactableHits.Length > 0 && inspectorController.displayedObject == null)
        {
            Cursor.SetCursor(hoverCursor, hoverOffset, CursorMode.Auto);
        }
        else if (noninteractableHits.Length > 0 && inspectorController.displayedObject == null)
        {
            Cursor.SetCursor(cantSelectCursor, cantSelectOffset, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(normalCursor, normalOffset, CursorMode.Auto);
        }
    }
}
