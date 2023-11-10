using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    public HUDController controller;
    [SerializeField] private AudioSource squawk;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            controller.LoseLife();
            squawk.Play();
        }
    }
}
