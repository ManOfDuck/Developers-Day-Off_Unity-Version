using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.FilePathAttribute;

[CreateAssetMenu]
public class LevelTile : Tile
{
    [SerializeField] private string _levelName;
    public string LevelName => _levelName;

    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite unlockedSprite;
    [SerializeField] private Sprite clearedSprite;

    public PathManager.ConnectionDirections EntranceDirection { get; set; }
}
