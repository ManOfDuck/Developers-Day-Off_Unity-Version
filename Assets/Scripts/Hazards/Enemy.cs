using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : SimulatedScript
{
    // The values of serialized fields are set it the editor
    [SerializeField] Collider2D enemyCollider;

    // Every object can see the Game Manager, which tracks important variables
    GameManager gameManager;

    // Start is called at the beginning of the game
    private void Start()
    {
        // Get a reference to the Game Manager
        gameManager = GameManager.Instance;
    }

    // This method is called when we collide with another object
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the game object we collided with is tagged as "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            // If so, tell the Game Manager to hurt the player
            gameManager.HurtPlayer();
        }
    }
}
