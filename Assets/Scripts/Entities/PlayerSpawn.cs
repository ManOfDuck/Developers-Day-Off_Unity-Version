using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerSpawn : MonoBehaviour
{
    public GameObject playerPrefab;
    public bool defaultSpawner;
    public bool secondarySpawner;
    [SerializeField] SpriteRenderer editorRenderer;

    private static PlayerSpawn _activeSpawner;
    public static PlayerSpawn ActiveSpawner { get { return _activeSpawner; } }

    private static PlayerController _player;
    public static PlayerController Player { get { return _player; } }

    public UnityEvent playerDead;



    // Start is called before the first frame update
    void Awake()
    {
        if (!playerPrefab.GetComponent<PlayerController>())
        {
            Debug.LogError("JESSE: That is NOT a PlayerController");
        }

        if (defaultSpawner && GameManager.Instance.DoPrimarySpawn)
        {
            SetSpawnPoint(this);
            SpawnPlayer();
        }
        else if (secondarySpawner && !GameManager.Instance.DoPrimarySpawn)
        {
            SetSpawnPoint(this);
            SpawnPlayer();
        }

        editorRenderer.enabled = false;
    }

    public static void Respawn()
    {
        // This isnt fantastic, but it works better than killing the player and spawning a new one
        Player.transform.position = ActiveSpawner.transform.position;
        GameManager.Instance.OnPlayerHurt.Invoke();
    }

    public static void SetSpawnPoint(PlayerSpawn newSpawner)
    {
        _activeSpawner = newSpawner;
    }

    public PlayerController SpawnPlayer()
    {
        GameObject spawnedObject = GameObject.Instantiate(playerPrefab, this.transform.position, this.transform.rotation);
        _player = spawnedObject.GetComponent<PlayerController>();
        return Player;
    }
}
