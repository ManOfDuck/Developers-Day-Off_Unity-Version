using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{

    [SerializeField] public Camera followCamera;

    [Tooltip("Offset from the player we'll follow at (based on the camera's position and the player's position)")]
    [SerializeField] private Vector3 offset;

    [Tooltip("Character we are following")]
    [SerializeField] private GameObject following;

    [SerializeField] private float leftBound;
    [SerializeField] private float rightBound;
    [SerializeField] private float upperBound;
    [SerializeField] private float lowerBound;

    public float shift;


    private void Awake()
    {
        if (following == null)
        {
            Debug.LogError("You didn't assign something to follow!");
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        if (following == null)
        {
            Debug.LogError("You didn't assign something to follow!");
        }
        transform.position = new Vector3(following.transform.position.x + offset.x,
        following.transform.position.y + offset.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        float boundedX = Mathf.Clamp(following.transform.position.x + offset.x, leftBound, rightBound);
        float boundedY = Mathf.Clamp(following.transform.position.y + offset.y, lowerBound, upperBound);
        transform.position = new Vector3(boundedX + shift, boundedY, transform.position.z);
        Debug.Log(shift);
    }

}
