using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinItem : MonoBehaviour
{
    public HUDController controller;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            controller.Win();

        }
    }
}
