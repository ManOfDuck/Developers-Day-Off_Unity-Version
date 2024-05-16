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
        Debug.Log("hi");
        Destroy(this.gameObject);
        collectTrigger.enabled = false;
    }
}
