using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    [SerializeField] private string upgradeName;
    [SerializeField] private Collider2D trigger;

    private bool collected = false;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collected)
        {
            collected = true;
            Collect();
        }
    }

    private void Collect()
    {
        gameManager.AddUpgrade(upgradeName);
        Destroy(this.gameObject);
    }
}
