using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    [SerializeField] private string upgradeName;
    [SerializeField] private Collider2D trigger;
    [SerializeField] private AudioSource audiosource;

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
            Debug.Log("playing audio");
            audiosource.Play();
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
