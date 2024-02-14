using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollision : SimulatedScript
{
    [SerializeField] LayerMask groundLayer;
    [SerializeField] public Rigidbody2D projectileBody;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!doCollisionEvents)
            return;

        Destroy(this.gameObject);
    }

}
