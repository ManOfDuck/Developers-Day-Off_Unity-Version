using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public GameObject playerPrefab;
    public bool defaultSpawner;

    private static PlayerSpawn _activeSpawner;
    public static PlayerSpawn ActiveSpawner { get { return _activeSpawner; } }

    private static PlayerController _player;
    public static PlayerController Player { get { return _player; } }

    // Start is called before the first frame update
    void Awake()
    {
        if (!playerPrefab.GetComponent<PlayerController>())
        {
            Debug.LogError("JESSE: That is NOT a PlayerController");
        }

        if (defaultSpawner)
        {
            SetActive();
            SpawnPlayer();
        }
    }

    public void SetActive()
    {
        _activeSpawner = this;
    }

    public PlayerController SpawnPlayer()
    {
        GameObject spawnedObject = GameObject.Instantiate(playerPrefab, this.transform.position, this.transform.rotation);
        _player = spawnedObject.GetComponent<PlayerController>();
        return Player;
    }
}
