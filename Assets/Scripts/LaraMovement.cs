using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class LaraMovement : MonoBehaviour
{
    private Vector2 movementInput;

    [SerializeField] private float moveSpeed = 8f; //top speed over time
    [SerializeField] private float accelSpeed = 49f;
    private float accelForce;
    [SerializeField] private float frictionAmount = .8f;
    [SerializeField] private float jumpForce = 300f;

    [SerializeField] private bool isGrounded = true;
    [Tooltip("The layer that the game considers 'ground' for jumping purposes")]
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Rigidbody2D playerRigidBody;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private Animator animator;

    private void Awake()
    {
        accelForce = playerRigidBody.mass * accelSpeed; //only need to calculate once

    }
    void OnValidate() //edge case, when change value in editor
    {
        accelForce = playerRigidBody.mass * accelSpeed;
    }
    private void FixedUpdate()
    {
        Move(movementInput);
        CheckIsGrounded();
        CheckIsRunning();
    }
    void Update()
    {

    }

    private void Move(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > 0.01f)
        {
            //Debug.Log("Move: " + direction);
            //Applyign forces to reach target speed & accounting for overshoot
            float targetSpeed = moveSpeed - Mathf.Abs(playerRigidBody.velocity.x);
            if (!Mathf.Approximately(targetSpeed, 0f))
            {
                //if speed dif is +, we can still accelerate! go fast!!
                if (targetSpeed > 0)
                {
                    float accelCap = Mathf.Min(targetSpeed / Time.fixedDeltaTime * playerRigidBody.mass, accelForce);
                    // now apply this maximum acceleration to our object
                    playerRigidBody.AddForce(direction * Vector2.right * accelCap, ForceMode2D.Force);
                }
                else //target speed is -
                {
                    playerRigidBody.AddForce(targetSpeed * Mathf.Sign(playerRigidBody.velocity.x) * playerRigidBody.mass * Vector2.right, ForceMode2D.Impulse);

                }
            }

            //Flipping Sprite
            if (direction.x > 0.01)
            {
                spriteRenderer.flipX = false;
            }
            else if (direction.x < -0.01)
                spriteRenderer.flipX = true;


        }
        else if (isGrounded)
        {
            float amount = Mathf.Min(Mathf.Abs(playerRigidBody.velocity.x), Mathf.Abs(frictionAmount));
            amount *= Mathf.Sign(playerRigidBody.velocity.x);

            playerRigidBody.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
        animator.SetBool("IsFalling", (playerRigidBody.velocity.y < 0f));

    } //END OF MOVE FUNCTION

    private void CheckIsGrounded()
    {
        float groundCheckDistance = .2f;
        RaycastHit2D boxCastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = (boxCastHit.collider != null); // returns false if raycast failed

        //Debug.Log(isGrounded);
        animator.SetBool("IsGrounded", isGrounded);
    } // GROUNDED CHECK END

    private void CheckIsRunning()
    {
        //Debug.Log("movespeed: " + Mathf.Abs(playerRigidBody.velocity.x));
        animator.SetFloat("movingSpeed", Mathf.Abs(playerRigidBody.velocity.x));
    } // RUNNING CHECK END


    public void MoveAction(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>() * Vector2.right;
    }// MOVE ACTION END

    public void JumpAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            
            if (isGrounded)
            {
                
                playerRigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

                animator.SetTrigger("Jump");
            }
        }
        else if (context.canceled)
        {
            

            if (playerRigidBody.velocity.y > 0)
            {
                playerRigidBody.AddForce(Vector2.down * playerRigidBody.velocity.y * .5f * playerRigidBody.mass, ForceMode2D.Impulse);
            }
        }
    } // JUMP ACTION END
}
