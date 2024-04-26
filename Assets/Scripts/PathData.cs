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
    public List<TileBase> ConnectedDown => _connectedUp;

    [SerializeField] private List<TileBase> _connectedLeft;
    public List<TileBase> ConnectedLeft => _connectedLeft;

    [SerializeField] private List<TileBase> _connectedRight;
    public List<TileBase> ConnectedRight => _connectedRight;
}
