using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class PathManager : MonoBehaviour
{
    [System.Flags]
    public enum ConnectionDirections
    {
        None = 0,       // 0000
        Up = 1 << 0,    // 0001
        Down = 1 << 1,  // 0010
        Left = 1 << 2,  // 0100
        Right = 1 << 3  // 1000
    }

    [SerializeField] private Tilemap pathmap;
    [SerializeField] private Tilemap levelmap;

    [SerializeField] private PathData pathData;

    private Dictionary<TileBase, ConnectionDirections> _connectionsFromTile = new();
    public Dictionary<TileBase, ConnectionDirections> ConnectionsFromTile { get => _connectionsFromTile; private set => _connectionsFromTile = value; }

    private static PathManager _instance;
    public static PathManager Instance { get { return _instance; } }

    private void Awake()
    {

        if (Instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        AddDirectionFlags(pathData.ConnectedUp, ConnectionDirections.Up);
        AddDirectionFlags(pathData.ConnectedDown, ConnectionDirections.Down);
        AddDirectionFlags(pathData.ConnectedLeft, ConnectionDirections.Left);
        AddDirectionFlags(pathData.ConnectedRight, ConnectionDirections.Right);
    }

    void AddDirectionFlags(IEnumerable<TileBase> tiles, ConnectionDirections direction)
    {
        foreach (TileBase tile in tiles)
        {
            Debug.Log(tile + ", " + direction);

            if (!ConnectionsFromTile.ContainsKey(tile))
            {
                ConnectionsFromTile.Add(tile, direction);
            }
            else
            {
                ConnectionsFromTile[tile] |= direction;
            }
        }
    }



    public TileBase GetTileAtPoint(Vector2 worldPoint)
    {
        Vector3Int gridPosiiton = pathmap.WorldToCell((Vector3)worldPoint);

        return pathmap.GetTile(gridPosiiton);
    }

    public LevelTile GetLevelTileAtPoint(Vector2 worldPoint)
    {
        Vector3Int gridPosiiton = levelmap.WorldToCell((Vector3)worldPoint);
        return levelmap.GetTile(gridPosiiton) as LevelTile;
    }

    public List<Vector2> GetPathFromPoint(Vector2 worldPosition, Vector2 direction)
    {
        return GetPathFromPoint(worldPosition, direction, pathmap.WorldToCell(worldPosition));
    }

    public List<Vector2> GetPathFromPoint(Vector2 worldPosition, Vector2 direction, Vector3Int startingGridPosition)
    {
        List<Vector2> path = new();
        Vector3Int gridPosition = pathmap.WorldToCell(worldPosition);

        ConnectionDirections connectionDirection = DirectionFromVector(direction);
        ConnectionDirections backwardsDirection = DirectionFromVector(-direction);

        if ((ConnectionsFromTile[pathmap.GetTile(gridPosition)] & connectionDirection) == 0)
            return null; // Cant move in that direction

        path.Add(GetCellCenter(gridPosition));

        Debug.Log(backwardsDirection);

        do
        {
            gridPosition += Vector3Int.RoundToInt((Vector3)direction);
            if (!pathmap.HasTile(gridPosition))
            {
                Debug.Log("begone!");
                return path; // Exit if out of bounds
            }
        }
        while ((ConnectionsFromTile[pathmap.GetTile(gridPosition)] & ~backwardsDirection) == connectionDirection);
        path.Add(GetCellCenter(gridPosition));

        if (gridPosition == startingGridPosition) return path; // Stop loops 

        Vector2 perpendicular = Vector2.Perpendicular(direction);
        ConnectionDirections otherConnections = ConnectionsFromTile[pathmap.GetTile(gridPosition)] & ~backwardsDirection;

        if (otherConnections == DirectionFromVector(perpendicular))
        {
            path.AddRange(GetPathFromPoint(pathmap.CellToWorld(gridPosition), perpendicular, startingGridPosition));

            return path;
        }

        if (otherConnections == DirectionFromVector(-perpendicular))
        {
            path.AddRange(GetPathFromPoint(pathmap.CellToWorld(gridPosition), -perpendicular, startingGridPosition));

            return path;
        }

        return path;
    }

    public bool IsPositionStoppingPoint(Vector2 position)
    {
        Vector3Int cellPosition = pathmap.WorldToCell(position);
        TileBase tile = pathmap.GetTile(cellPosition);
        ConnectionDirections connections = ConnectionsFromTile[tile];
        int numConnections = 0;
        if ((connections & ConnectionDirections.Up) != 0) numConnections++;
        if ((connections & ConnectionDirections.Down) != 0) numConnections++;
        if ((connections & ConnectionDirections.Left) != 0) numConnections++;
        if ((connections & ConnectionDirections.Right) != 0) numConnections++;

        return numConnections > 2 || levelmap.GetTile(cellPosition) != null;
    }

    private Vector2 GetCellCenter(Vector3Int cellPosition)
    {
        Vector2 position = pathmap.CellToWorld(cellPosition);
        Vector2 adjustedPosition = position + (Vector2)pathmap.cellSize / 2;

        return adjustedPosition;
    }

    private ConnectionDirections DirectionFromVector(Vector2 direction)
    {
        if (direction == Vector2.up)
        {
            return ConnectionDirections.Up;
        }
        if (direction == Vector2.down)
        {
            return ConnectionDirections.Down;
        }
        if (direction == Vector2.left)
        {
            return ConnectionDirections.Left;
        }
        if (direction == Vector2.right)
        {
            return ConnectionDirections.Right;
        }

        return ConnectionDirections.None;
    }

    // Update is called once per frame
    void Update()
    {

    }
}

