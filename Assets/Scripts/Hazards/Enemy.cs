using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Collider2D enemyCollider;
    [SerializeField] int damage = 1;

    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            for (int i = 0; i < damage; i++)
            {
                gameManager.HurtPlayer();
            }
        }
    }
}
