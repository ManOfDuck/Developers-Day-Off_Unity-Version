using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D body;

    private bool turned = false;

    private void Update()
    {
        if (turned)
        {
            body.rotation += speed * Time.deltaTime;
        }
        else
        {
            body.rotation -= speed * Time.deltaTime;
        }

        if (Mathf.Abs(body.rotation) > 90)
        {
            turned = !turned;
        }
    }
}
