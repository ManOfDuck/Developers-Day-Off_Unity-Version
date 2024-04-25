using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : SimulatedScript
{
    protected override string DefaultVisualComponentName => "Enemy";

    // Every object can see the Game Manager, which tracks important variables
    GameManager gameManager;

    // Start is called at the beginning of the game
    override protected void Start()
    {
        base.Start();
        // Get a reference to the Game Manager
        gameManager = GameManager.Instance;
    }

    // This method is called when we collide with another object
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!doCollisionEvents)
            return;

        Light(15);
        // Check if the game object we collided with is tagged as "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            Light(18);
            // If so, hurt the player
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player)
                player.Hurt(1);
            Light(20);
        }
    }

    public override SimulatedComponent Copy(SimulatedObject destination)
    {
        return destination.gameObject.AddComponent<Enemy>();
    }
}
