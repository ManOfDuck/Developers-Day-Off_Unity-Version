using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleShrink : MonoBehaviour
{
    RectTransform rectTransform;

    [SerializeField] public float changeAmount = 10;
    [SerializeField] float size = 0;
    private bool closing = false;
    private bool opening = true;
    private bool clearLevel = false;
    private string nextScene;

    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = this.GetComponent<RectTransform>();
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if(opening)
        {
            size += changeAmount * Time.deltaTime;
            rectTransform.sizeDelta = new Vector2(size, size);
            if(size >= 2200)
            {
                opening = false;
            }

        }

        if(closing)
        {
            size -= changeAmount * Time.deltaTime;
            rectTransform.sizeDelta = new Vector2(size, size);

            if(size <= 0 && clearLevel == false)
            {
                //change scenes
                FollowCamera.Instance.isChangingScenes = false;
                gameManager.LoadScene(nextScene, true);
            }
            else if(size <= 0 && clearLevel == true)
            {
                GameManager.Instance.ClearLevel();
            }
        }
    }

    public void closeCircle(string sceneName)
    {
        FollowCamera.Instance.isChangingScenes = true;
        closing = true;
        nextScene = sceneName;
    }

    public void ClearLevel()
    {
        FollowCamera.Instance.isChangingScenes = true;
        closing = true;
        clearLevel = true;
    }
}
