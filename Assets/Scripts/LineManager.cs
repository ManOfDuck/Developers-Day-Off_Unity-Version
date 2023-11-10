using UnityEngine;
using UnityEngine.InputSystem;

public class LineManager : MonoBehaviour
{

    GameManager gameManager;
    float initialMana;

    [SerializeField]
    private Material lineMaterial;

    Vector2 oldMousePos;
    Vector2 mousePos;
    Root currentLine;

    Collider2D[] overlapArray;

    [SerializeField]
    [Range(.5f, 2f)]
    private float percision = .5f;

    [SerializeField] GameObject LinePrefab;

    [SerializeField] float mouseRadius;

    private bool creating;

    private bool rootable;
    private bool unrootable;

    private LineRenderer lr;

    private void Awake()
    {
        creating = false;
        lr = null;
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        oldMousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        initialMana = gameManager.mana;
        overlapArray = new Collider2D[10];
        currentLine = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(mousePos, mouseRadius);
    }


    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.T))
        {
            gameManager.mana = initialMana;
        }

        /*
        //move this below the if statements
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log($"retracting vines: {transform.childCount}");
            creating = false;
            //while (lineCount > 0)
            //{
                Debug.Log(transform.childCount);
                Destroy(transform.GetChild(0).gameObject);
                Debug.Log(transform.childCount);
            //}
        }
        */


        float input = Mouse.current.leftButton.ReadValue();
        mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());


        if (input == 1)
        {
            rootable = false;
            unrootable = false;
            overlapArray = Physics2D.OverlapCircleAll(mousePos, mouseRadius); //add an array of collider2ds

            for (int i = 0; i < overlapArray.Length; i++)
            {
                if (overlapArray[i].tag == "Rootable")
                {
                    rootable = true;
                }
                if (overlapArray[i].tag == "UnRootable")
                {
                    unrootable = true;
                }
            }
        }


        // Initial click
        if (!creating && input == 1 && rootable && !unrootable && gameManager.mana > 0)
        {
            creating = true;
            oldMousePos = mousePos;

            //Physics2D.CapsuleCast(mousePos, (2, 4), ?, 0, (0, 0));

            // Sets up line object
            //GameObject line = new GameObject("Line");
            //INSTANTIATE A PREFAB HERE
            GameObject line = Instantiate<GameObject>(LinePrefab, Vector2.zero, Quaternion.identity);
           // currentLine = line;
            //line.transform.position = mousePos;
            lr = line.GetComponent<LineRenderer>(); //use getcomponent for prefab
            //lr.transform.parent = transform;
            lr.material = lineMaterial;
            lr.startColor = Color.white;
            lr.endColor = Color.white;
            lr.startWidth = .5f;
            lr.endWidth = .5f;
            lr.positionCount = 1;
            lr.sortingOrder = 5;

            lr.SetPosition(0, mousePos);

            //line.transform.tag = "Ground";
            line.layer = LayerMask.NameToLayer("ground");
            line.tag = "Rootable";
        }

        //else if (creating)
        else if (creating)
        {
            Debug.Log(gameManager.GetMana());
            // Holding down mouse
            if (input == 1 && gameManager.mana > 0 && !unrootable)
            {
                Vector2 length = mousePos - (Vector2)lr.GetPosition(lr.positionCount - 2);
                if (length.magnitude >= percision)
                    lr.positionCount++;
                lr.SetPosition(lr.positionCount - 1, mousePos);


                gameManager.mana -= (mousePos - oldMousePos).magnitude;
                oldMousePos = mousePos;
            }
            // Releasing mouse
            else
            {
                lr.SetPosition(lr.positionCount - 1, mousePos);

                // Setting up line collider
                PolygonCollider2D col = lr.gameObject.AddComponent<PolygonCollider2D>();
                col.pathCount = lr.positionCount;
                for (int i = 0; i < lr.positionCount - 1; i++)
                {
                    col.SetPath(i, GetColliderPoints(lr.GetPosition(i), lr.GetPosition(i + 1)));
                }

                creating = false;
                lr = null;
            }
        }
    }
    private Vector2[] GetColliderPoints(Vector2 p1, Vector2 p2)
    {
        Vector2[] points = new Vector2[4];

        // Finding colliders for first point
        Vector2 line = p2 - p1;
        float angle = Vector2.SignedAngle(transform.right, line) + 90;
        float halfWidth = lr.startWidth / 2;
        Vector2 offset = new Vector2(halfWidth * Mathf.Cos(angle * Mathf.Deg2Rad), halfWidth * Mathf.Sin(angle * Mathf.Deg2Rad));
        points[0] = p1 + offset;
        points[1] = p2 + offset;
        points[2] = p2 - offset;
        points[3] = p1 - offset;
        return points;
    }

}