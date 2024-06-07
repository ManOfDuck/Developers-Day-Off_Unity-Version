using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableComponent : ComponentHolder
{
    [SerializeField] Collider2D collectTrigger;

    private ToolkitController toolkitController;
    private bool collected = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        toolkitController = ToolkitController.Instance;

        DestroyIfCollected();
    }

    private void DestroyIfCollected()
    {
        if (toolkitController.Components.Count == 0) return; // There are no components, dont delete

        bool alreadyHave = false;

        foreach (SimulatedComponent newComponent in Components)
        {
            foreach (SimulatedComponent existingComponent in toolkitController.Components)
            {
                if (newComponent.GetType() == existingComponent.GetType())
                {
                    Debug.Log("we got " + newComponent.GetType());
                    alreadyHave = true;
                }
            }
            if (!alreadyHave)
            {
                return; // We have something new! Persist
            }
        }

        Destroy(this.gameObject); // You should kill yourself NOW
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collected)
        {
            collected = true;
            Collect();
        }
    }

    private void Collect()
    {
        foreach(SimulatedComponent c in Components)
        {
            c.Copy(toolkitController);
        }
  
        Destroy(this.gameObject);
        collectTrigger.enabled = false;
    }
}
