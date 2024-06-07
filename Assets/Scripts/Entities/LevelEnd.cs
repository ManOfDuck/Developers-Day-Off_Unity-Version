using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    [SerializeField] Collider2D trigger;
    [SerializeField] CircleShrink circleShrinkScript;
    public bool isSecondLevel;
    public bool doFakeFall;
    [SerializeField] AudioSource secondLevelFallingSound;

    private void Start()
    {
        circleShrinkScript.changeAmount = 4000;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            if(isSecondLevel || doFakeFall)
            {
                //play audio clip before doing clear level
                FollowCamera.Instance.isChangingScenes = true;
                secondLevelFallingSound.Play();
                if (isSecondLevel) StartCoroutine(EndSecondLevel());
            }

            else
            {
                Debug.Log("level cleared");
                //GameManager.Instance.ClearLevel();
                circleShrinkScript.ClearLevel();
            }
        }
    }

    IEnumerator EndSecondLevel()
    {
        yield return new WaitForSeconds(6f);
        circleShrinkScript.ClearLevel();
    }
}
