using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatedObject : MonoBehaviour
{
    public List<SimulatedComponent> components;
    public List<SimulatedScript> scripts;
    public Collider2D clickTrigger;
    private LayerMask layer;
    private InspectorController controller;
    private GameManager gameManager;

    public Sprite defaultSprite;
    public Sprite sprite1;

    [System.Serializable]
    public class SimulatedComponent
    {
        public Component realComponent;
        public VisualComponent visualComponent;
    }

    public void Start()
    {
        controller = InspectorController.Instance;
        gameManager = GameManager.Instance;
        layer = gameObject.layer;
        foreach (SimulatedScript script in scripts)
        {
            SetScriptEnabledStatus(script, script.isActiveAndEnabled);
        }
    }

    public bool IsComponentToggleable(SimulatedComponent component)
    {
        switch (component.realComponent)
        {
            case Collider2D:
                return true;
            case SpriteRenderer:
                return true;
            case Animator:
                return true;
            default:
                return false;
        }
    }

    public bool GetComponentEnabledStatus (SimulatedComponent component)
    {
        switch (component.realComponent)
        {
            case Collider2D collider:
                return collider.enabled;
            case SpriteRenderer renderer:
                return renderer.enabled;
            case Animator animator:
                return animator.enabled;
            default:
                return true;
        }
    }

    public void SetComponentEnabledStatus(SimulatedComponent component, bool enabled)
    {
        switch (component.realComponent)
        {
            case Collider2D collider:
                collider.enabled = enabled;
                gameObject.layer = enabled ? layer : 0;
                break;
            case SpriteRenderer renderer:
                renderer.enabled = enabled;
                break;
            case Animator animator:
                animator.enabled = enabled;
                break;
            default:
                Debug.Log("hello");
                break;
        }     
    }

    public void ToggleComponent(SimulatedComponent component)
    {
        SetComponentEnabledStatus(component, !GetComponentEnabledStatus(component));
    }


    public void SetScriptEnabledStatus(SimulatedScript script, bool enabled)
    {
        script.enabled = enabled;
        script.doCoroutines = enabled;
        script.doCollisionEvents = enabled;
    }

    public void ToggleScript(SimulatedScript script)
    {
        SetScriptEnabledStatus(script, !script.enabled);
    }


    public void OnMouseDown()
    {
        Vector2 playerPos = PlayerSpawn.Player.transform.position;
        Vector2 cursorPos = controller.followCamera.controlledCamera.ScreenToWorldPoint(Input.mousePosition);
        if (!CheckBlockingObject(playerPos, cursorPos).HasValue)
        {
            controller.DisplayObject(this, defaultSprite, sprite1);
        }
    }

    public Vector2? CheckBlockingObject(Vector2 origin, Vector2 destination)
    {
        Vector2 raycastVector = destination - origin;
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, raycastVector.normalized, raycastVector.magnitude, PlayerSpawn.Player.groundLayer);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag(controller.lineOfSightBlockingTag) && !clickTrigger.bounds.Contains(hit.point) && hit.collider != clickTrigger)
            {
                Debug.Log(hit.point);
                return hit.point;
            }
        }
        return null;
    }
}
