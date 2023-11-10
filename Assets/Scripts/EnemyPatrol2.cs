using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol2 : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Rigidbody2D enemyRB;

    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Transform groundCheckGizmoL;


    private bool isFacingRight = true;
    private RaycastHit2D hitL;
    private RaycastHit2D hitW;

    private void Awake()
    {
        
            
        
    }

    private void Update()
    {
        hitL = Physics2D.Raycast(groundCheckGizmoL.position, Vector2.down, 1f, layerMask);
        hitW = Physics2D.Raycast(groundCheckGizmoL.position, Vector2.right, 1f, layerMask);

    }

    private void FixedUpdate()
    {
        if (hitL.collider != false && hitW == false)
        {
            Debug.Log("Hit Ground");
            Debug.Log(isFacingRight);
            if (isFacingRight)
            {
                enemyRB.velocity = new Vector2(moveSpeed, enemyRB.velocity.y);
            }
            else
            {
                enemyRB.velocity = new Vector2(-moveSpeed, enemyRB.velocity.y);
            }
        }
        else
        {
            
            if(hitL.collider == false)
            {
                Debug.Log("Not Ground");
                isFacingRight = !isFacingRight;
                transform.localScale = new Vector3(-transform.localScale.x, 1f, 1f);
            }
            else if (hitW.collider != false)
            {
                Debug.Log("Wall");
                isFacingRight = !isFacingRight;
                transform.localScale = new Vector3(-transform.localScale.x, 1f, 1f);
            }
            
        }
    }

}
