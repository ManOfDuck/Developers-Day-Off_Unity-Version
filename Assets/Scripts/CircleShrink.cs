using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleShrink : MonoBehaviour
{
    RectTransform rectTransform;

    [SerializeField] float changeAmount = 10;
    [SerializeField] float size = 0;
    private bool closing = false;
    private bool opening = true;
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
            size += changeAmount;
            rectTransform.sizeDelta = new Vector2(size, size);
            if(size >= 2200)
            {
                opening = false;
            }

        }

        if(closing)
        {
            size -= changeAmount;
            rectTransform.sizeDelta = new Vector2(size, size);

            if(size <= 0)
            {
                //change scenes
                FollowCamera.Instance.isChangingScenes = false;
                gameManager.LoadScene(nextScene, true);
            }
        }
    }

    public void closeCircle(string sceneName)
    {
        FollowCamera.Instance.isChangingScenes = true;
        closing = true;
        nextScene = sceneName;
    }
}
