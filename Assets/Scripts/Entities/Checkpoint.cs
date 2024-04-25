using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private PlayerSpawn newSpawn;
    SpriteRenderer spriteRenderer;
    [SerializeField] Sprite redFlag;
    [SerializeField] Sprite greenFlag;


    private void Start()
    {
        //change to red flag
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = redFlag;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerSpawn.SetSpawnPoint(newSpawn);
            spriteRenderer.sprite = greenFlag;
            //change to green flag
        }
    }
}
