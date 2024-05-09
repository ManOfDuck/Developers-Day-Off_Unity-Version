using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//https://docs.unity3d.com/ScriptReference/Physics2D.Raycast.html

public class Thwomp : MonoBehaviour
{

    Vector3 startPos;
    Rigidbody2D rigidbody;
    bool outOfPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPos = this.transform.position;
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //move down if raycast is called


        int layerMask = 1 << 7;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 50f, layerMask);

        if (hit.collider != null)
        {
            Debug.Log("we hit something " + hit.collider.tag);
            MoveDown();
        }
        else if(outOfPosition == true)
        {
            rigidbody.velocity = Vector2.up * 7;
            if(rigidbody.position.y >= startPos.y)
            {
                rigidbody.velocity = new Vector2(0, 0);
                outOfPosition = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        rigidbody.velocity = new Vector2(0, 0);
        MoveUp(startPos);
    }

    void MoveDown()
    {
        //go down until something is hit, then move up to original position
        rigidbody.velocity = Vector2.down * 7;
        
    }

    void MoveUp(Vector3 originalPosition)
    {
        //move to original position
        //rigidbody.position = Vector3.MoveTowards(rigidbody.position, originalPosition, 7f);
        outOfPosition = true;
    }
}
