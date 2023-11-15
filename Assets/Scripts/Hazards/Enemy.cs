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
