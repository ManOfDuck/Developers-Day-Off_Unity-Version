using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollision : SimulatedScript
{
    [SerializeField] LayerMask groundLayer;
    [SerializeField] public Rigidbody2D projectileBody;

    protected override string DefaultVisualComponentName => "DestroyOnCollision";

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!doCollisionEvents)
            return;

        Destroy(this.gameObject);
    }

    // TODO: Implement this
    public override SimulatedComponent Copy(SimulatedObject destination)
    {
        throw new System.NotImplementedException();
    }
}
