using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleShrink : MonoBehaviour
{
    RectTransform rectTransform;

    [SerializeField] float shrinkValue = 10;
    float size = 2200;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = this.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        size -= shrinkValue;
        rectTransform.sizeDelta = new Vector2(size, size);
    }
}
