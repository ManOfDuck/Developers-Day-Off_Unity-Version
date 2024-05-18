using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollision : SimulatedScript
{
    protected override string DefaultVisualComponentName => "DestroyOnCollision";

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!DoCollisionEvents)
            return;

        if (collision.gameObject.CompareTag("Player")) return;

        Destroy(this.gameObject);
    }

    public override SimulatedComponent Copy(ComponentHolder destination)
    {
        DestroyOnCollision copy = destination.gameObject.AddComponent<DestroyOnCollision>();
        return copy;
    }
}
