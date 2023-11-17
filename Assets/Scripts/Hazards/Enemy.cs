using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : SimulatedScript
{
    // The values of serialized fields are set it the editor
    [SerializeField] Collider2D enemyCollider;
    [SerializeField] int damage = 1;

    // Every object can see the game manager, which tracks important variables
    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            for (int i = 0; i < damage; i++)
            {
                Debug.Log("hurting player");
                gameManager.HurtPlayer();
            }
        }
    }
}
