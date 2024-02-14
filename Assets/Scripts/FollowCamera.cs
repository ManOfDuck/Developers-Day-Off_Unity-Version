using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{

    [SerializeField] public Camera controlledCamera;

    [Tooltip("Offset from the player we'll follow at (based on the camera's position and the player's position)")]
    [SerializeField] private Vector3 offset;

    [SerializeField] private float leftBound;
    [SerializeField] private float rightBound;
    [SerializeField] private float upperBound;
    [SerializeField] private float lowerBound;

    public float shift;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerSpawn.Player == null)
        {
            Debug.LogError("You didn't assign something to follow!");
        }
        transform.position = new Vector3(PlayerSpawn.Player.transform.position.x + offset.x,
        PlayerSpawn.Player.transform.position.y + offset.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        float boundedX = Mathf.Clamp(PlayerSpawn.Player.transform.position.x + offset.x, leftBound, rightBound);
        float boundedY = Mathf.Clamp(PlayerSpawn.Player.transform.position.y + offset.y, lowerBound, upperBound);
        transform.position = new Vector3(boundedX + shift, boundedY, transform.position.z);
    }

}
