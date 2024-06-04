using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransitionTrigger : MonoBehaviour
{
    [SerializeField] private string level;
    [SerializeField] private bool secondarySpawn = false;
    [SerializeField] private Collider2D trigger;


    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            //gameManager.LoadScene(level, !secondarySpawn);
            StartCoroutine(SceneChange(2));
        }
    }

    IEnumerator SceneChange(float secondsToWait)
    {
        //set boolean playerIsInCutscene
        //PlayerSpawn.Player.   

        yield return new WaitForSeconds(secondsToWait);
        gameManager.LoadScene(level, !secondarySpawn);
    }
}
