using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class PathData : ScriptableObject
{
    [SerializeField] private List<TileBase> _connectedUp;
    public List<TileBase> ConnectedUp => _connectedUp;

    [SerializeField] private List<TileBase> _connectedDown;
    public List<TileBase> ConnectedDown => _connectedDown;

    [SerializeField] private List<TileBase> _connectedLeft;
    public List<TileBase> ConnectedLeft => _connectedLeft;

    [SerializeField] private List<TileBase> _connectedRight;
    public List<TileBase> ConnectedRight => _connectedRight;

    [SerializeField] private List<TileBase> _notConnected;
    public List<TileBase> NotConnected => _notConnected;

    [SerializeField] private List<TileBase> _levelHoldingTiles;
    public List<TileBase> LevelHoldingTiles => _levelHoldingTiles;

    [SerializeField] private List<LevelTile> _levels;
    public List<LevelTile> Levels => _levels;
}
