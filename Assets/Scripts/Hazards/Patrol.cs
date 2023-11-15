using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    [SerializeField] private List<Vector2> patrolPoints;
    [SerializeField] private float speed;
    [SerializeField] private float waitTime;
    [SerializeField] private Rigidbody2D body;

    Vector2 initPos;

    // Start is called before the first frame update
    void Awake()
    {
        patrolPoints.Add(new Vector2(0, 0));
        initPos = body.position;
        StartCoroutine(Move());
    }

    protected virtual IEnumerator Move()
    {
        float step = 0f;
        Vector2 target = Vector2.zero;
        while (true)
        {
            foreach (Vector2 point in patrolPoints)
            {
                target = point + initPos;
                while (body.position != target)
                {
                    step = speed * Time.deltaTime;
                    body.position = Vector2.MoveTowards(body.position, target, step);
                    yield return null;
                }
                yield return new WaitForSeconds(waitTime);
            }
        }
    }
}
