using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : SimulatedScript
{
    [SerializeField] private PlayerSpawn newSpawn;
    SpriteRenderer spriteRenderer;
    [SerializeField] Sprite redFlag;
    [SerializeField] Sprite greenFlag;
    [SerializeField] private bool visible = true;

    protected override string DefaultVisualComponentName => "Checkpoint";

    protected override void Start()
    {
        base.Start();
        //change to red flag
        if (visible)
        {
            spriteRenderer = this.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = redFlag;
            spriteRenderer.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerSpawn.SetSpawnPoint(newSpawn);
            if (visible) spriteRenderer.sprite = greenFlag;
            //change to green flag
        }
    }

    public override SimulatedComponent Copy(ComponentHolder destination)
    {
        // no
        return null;
    }
}
