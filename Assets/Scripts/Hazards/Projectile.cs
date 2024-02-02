using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : SimulatedScript
{
    [SerializeField] LayerMask groundLayer;
    [SerializeField] public Rigidbody2D projectileBody;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.IsTouchingLayers(groundLayer))
        {
            Destroy(this.gameObject);
        }
    }

}
