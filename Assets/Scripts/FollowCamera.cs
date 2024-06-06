using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{

    private static FollowCamera _instance;
    public static FollowCamera Instance { get { return _instance; } }


    [SerializeField] public Camera controlledCamera;

    [Tooltip("Offset from the player we'll follow at (based on the camera's position and the player's position)")]
    [SerializeField] private Vector3 offset;
    [SerializeField] private BoxCollider2D cameraBoundingBox;


    private float leftBound;
    private float rightBound;
    private float lowerBound;
    private float upperBound;
    Vector2 cameraBounds;

    private Vector2 target;

    private bool bounded = true;
    public bool isChangingScenes;

    public float shift;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        leftBound = cameraBoundingBox.bounds.min.x;
        rightBound = cameraBoundingBox.bounds.max.x;
        lowerBound = cameraBoundingBox.bounds.min.y;
        upperBound = cameraBoundingBox.bounds.max.y;

        cameraBounds = new Vector2(controlledCamera.aspect * controlledCamera.orthographicSize, controlledCamera.orthographicSize);

        transform.position = new Vector3(PlayerSpawn.Player.transform.position.x + offset.x,
        PlayerSpawn.Player.transform.position.y + offset.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        //Once the player goes out-of-bounds, they cannot reactivate the bounded camera
        if(!isChangingScenes)
        {
            bounded = bounded && cameraBoundingBox.bounds.Contains((Vector2)PlayerSpawn.Player.transform.position);
        }

        if (bounded)
        {
            target = GetBoundedPosition();
        }
        else
        { 
            Vector2 playerPos = PlayerSpawn.Player.transform.position;
            target = new(playerPos.x + offset.x, playerPos.y + offset.y);
        }

        transform.position = new(target.x, target.y, offset.z);
    }

    private Vector2 GetBoundedPosition()
    {
        float boundedX = Mathf.Clamp(PlayerSpawn.Player.transform.position.x + offset.x, leftBound + cameraBounds.x, rightBound - cameraBounds.x);
        float boundedY = Mathf.Clamp(PlayerSpawn.Player.transform.position.y + offset.y, lowerBound + cameraBounds.y, upperBound - cameraBounds.y);
        return new Vector3(boundedX, boundedY, transform.position.z);
    }
}
