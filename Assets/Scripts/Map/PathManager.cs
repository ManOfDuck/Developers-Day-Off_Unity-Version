using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    public enum LevelStatus
    {
        Locked,
        Unlocked,
        Cleared
    }

    [Header("Tilemaps")]
    [SerializeField] private Tilemap lockedPaths;
    [SerializeField] private Tilemap animatingPaths;
    [SerializeField] private Tilemap unlockedPaths;
    [SerializeField] private Tilemap levels;
    [SerializeField] private Tilemap levelVisuals;

    [Header("Tiles")]
    [SerializeField] private PathData pathData;
    [SerializeField] private TileBase unlockedLevelTile;
    [SerializeField] private TileBase clearedLevelTile;

    [Header("Visual")]
    [SerializeField] private float lockedOpacity = 132f;
    [SerializeField] private float unlockedOpacity = 255f;
    [SerializeField] private float fadeSpeed = 100f;

    private Dictionary<TileBase, ConnectionDirections> _connectionsFromTile = new();
    public Dictionary<TileBase, ConnectionDirections> ConnectionsFromTile { get => _connectionsFromTile; private set => _connectionsFromTile = value; }

    public Dictionary<LevelTile, LevelStatus> LevelStatuses { get; private set; } = new();

    private static PathManager _instance;
    public static PathManager Instance { get { return _instance; } }

    private GameManager gameManager;

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
        gameManager = GameManager.Instance;

        AddDirectionFlags(pathData.ConnectedUp, ConnectionDirections.Up);
        AddDirectionFlags(pathData.ConnectedDown, ConnectionDirections.Down);
        AddDirectionFlags(pathData.ConnectedLeft, ConnectionDirections.Left);
        AddDirectionFlags(pathData.ConnectedRight, ConnectionDirections.Right);
        AddDirectionFlags(pathData.NotConnected, ConnectionDirections.None);
        AddDirectionFlags(pathData.Levels, ConnectionDirections.None);

        foreach(LevelTile level in pathData.Levels)
        {
            LevelStatuses.Add(level, LevelStatus.Locked);
        }

        Vector3Int playerStartingCell = Vector3Int.RoundToInt(this.transform.position);

        List<Vector3Int> unlockedLevels = UnlockTilesFromPoint(Vector3Int.RoundToInt(this.transform.position), lockedPaths, unlockedPaths);
        foreach (Vector3Int cell in unlockedLevels)
        {
            LevelTile level = levels.GetTile(cell) as LevelTile;

            if (level == null) continue;

            if (gameManager.CurrentLevel != null && gameManager.CurrentLevel == level.LevelName && gameManager.ClearedLevels.Contains(level.LevelName))
            {
                // We just cleared this level, lets do an animation
                LevelStatuses[level] = LevelStatus.Cleared;
                UnlockTilesFromPoint(cell, lockedPaths, animatingPaths);
                StartCoroutine(AnimateTileUnlock(cell));
            }
            else
            {
                // This level hasn't been cleared, just set it's visual
                LevelStatuses[level] = LevelStatus.Unlocked;
                levelVisuals.SetTile(cell, unlockedLevelTile);
            }

           
        }
    }

    private IEnumerator AnimateTileUnlock(Vector3Int unlockFrom)
    {
        float alpha = animatingPaths.color.a;
        while (alpha < unlockedOpacity)
        {
            alpha += fadeSpeed * Time.deltaTime;
            animatingPaths.color = new Color(animatingPaths.color.r, animatingPaths.color.g, animatingPaths.color.b, alpha/255);
            yield return null;
        }
        List<Vector3Int> newLevels = UnlockTilesFromPoint(unlockFrom, animatingPaths, unlockedPaths);
        foreach (Vector3Int cell in newLevels)
        {
            LevelTile level = levels.GetTile(cell) as LevelTile;

            Debug.Log(level);

            if (level == null) continue;

            if (LevelStatuses[level] == LevelStatus.Locked)
            {
                LevelStatuses[level] = LevelStatus.Unlocked;
                levelVisuals.SetTile(cell, unlockedLevelTile);
            }
        }
    }

    void AddDirectionFlags(IEnumerable<TileBase> tiles, ConnectionDirections direction)
    {
        foreach (TileBase tile in tiles)
        {
            if (!ConnectionsFromTile.ContainsKey(tile))
            {
                ConnectionsFromTile.Add(tile, direction);
            }
            else
            {
                ConnectionsFromTile[tile] |= direction;
            }
            Debug.Log(tile + ", " + ConnectionsFromTile[tile]);
        }
    }


    public List<Vector2> GetPathFromPoint(Vector2 worldPosition, Vector2 direction)
    {
        return GetPathFromPoint(worldPosition, direction, unlockedPaths.WorldToCell(worldPosition));
    }

    public List<Vector2> GetPathFromPoint(Vector2 worldPosition, Vector2 direction, Vector3Int startingGridPosition)
    {
        List<Vector2> path = new();
        Vector3Int gridPosition = unlockedPaths.WorldToCell(worldPosition);

        // Get the direction to travel in and its reverse
        ConnectionDirections connectionDirection = DirectionFromVector(direction);
        ConnectionDirections backwardsDirection = DirectionFromVector(-direction);

        // Check if we can move in that direction
        if ((ConnectionsFromTile[unlockedPaths.GetTile(gridPosition)] & connectionDirection) == 0)
            return null;

        // If we can move, start the path here
        path.Add(GetCellCenter(gridPosition));

        // Move in that direction until we hit a tile that isn't straight
        do
        {
            gridPosition += Vector3Int.RoundToInt((Vector3)direction);
            if (!unlockedPaths.HasTile(gridPosition))
            {
                Debug.Log("begone!");
                path.Add(GetCellCenter(gridPosition - Vector3Int.RoundToInt((Vector3)direction)));
                return path; // Exit if out of bounds (Last tile had connection to empty space)
            }
        }
        while ((ConnectionsFromTile[unlockedPaths.GetTile(gridPosition)] & ~backwardsDirection) == connectionDirection && levels.GetTile(gridPosition) == null);

        // Add the tile we arrived at to the path
        path.Add(GetCellCenter(gridPosition));

        // Check if we need to stop
        if (gridPosition == startingGridPosition) return path; // Stop loops
        if (levels.GetTile(gridPosition) != null) return path; // Stop at levels

        // Get the relevant connections (we dont care if its connected backwards)
        ConnectionDirections otherConnections = ConnectionsFromTile[unlockedPaths.GetTile(gridPosition)] & ~backwardsDirection;

        // If the only other connection is perpinduclar to us (L-shaped path), keep pathfinding in that direction and add the result to our path
        Vector2 perpendicular = Vector2.Perpendicular(direction);
        if (otherConnections == DirectionFromVector(perpendicular))
        {
            path.AddRange(GetPathFromPoint(unlockedPaths.CellToWorld(gridPosition), perpendicular, startingGridPosition));

            return path;
        }

        if (otherConnections == DirectionFromVector(-perpendicular))
        {
            path.AddRange(GetPathFromPoint(unlockedPaths.CellToWorld(gridPosition), -perpendicular, startingGridPosition));

            return path;
        }

        // Otherwise, we're either at a dead-end or a junction, and the path should stop
        return path;
    }


    public List<Vector3Int> UnlockTilesFromPoint(Vector3Int gridPosition, Tilemap originMap, Tilemap destinationMap)
    {
        return UnlockTilesFromPoint(gridPosition, originMap, destinationMap, Vector2.zero, new());
    }

    public List<Vector3Int> UnlockTilesFromPoint(Vector3Int gridPosition, Tilemap originMap, Tilemap destinationMap, Vector2 searchingFrom, HashSet<Vector3Int> alreadyFound)
    {
        TileBase tile = originMap.GetTile(gridPosition);
        List<Vector3Int> foundLevels = new();

        //Debug.Log("searching at " + gridPosition);

        // If this cell's tile is null or has already been found, return early
        if (!originMap.HasTile(gridPosition) || alreadyFound.Contains(gridPosition))
        {
            return foundLevels; 
        }

        if (levels.GetTile(gridPosition) is LevelTile level)
        {
            // If the level is uncleared, stop this path (dont unlock the tile beneath it)
            if (LevelStatuses[level] != LevelStatus.Cleared)
            {
                // Add this connection to the level tile
                ConnectionsFromTile[level] |= DirectionFromVector(-searchingFrom);


                TileBase levelHolder = null;
                // Search all level holding tiles
                foreach (TileBase holdingTile in pathData.LevelHoldingTiles)
                {
                    // Find the one with the same connections as the level
                    if (ConnectionsFromTile[holdingTile] == ConnectionsFromTile[level])
                    {
                        levelHolder = holdingTile;
                        break;
                    }
                }
                // Add that tile beneath the level
                destinationMap.SetTile(gridPosition, levelHolder);

                // Return this level
                foundLevels.Add(gridPosition);

                // This level hasn't been cleared, stop unlocking the path
                Debug.Log(foundLevels.Count);
                return foundLevels;
            }
            else
            {
                // This level has been cleared, update its sprite
                levelVisuals.SetTile(gridPosition, clearedLevelTile);
            }
        }



        // Unlock this tile
        destinationMap.SetTile(gridPosition, tile);

        //Set this tile as found
        alreadyFound.Add(gridPosition);
        //Debug.Log("found tile: " + tile + " with connections " + ConnectionsFromTile[tile]);

        // Get all valid directions from this cell's tile
        List<Vector2> vectors = VectorsFromDirection(ConnectionsFromTile[tile]);

        // Recursively search in each direction
        foreach (Vector2 v in vectors)
        {
            //Debug.Log("searching " + DirectionFromVector(v) + " from " + gridPosition);
            Vector3Int newGridPosition = gridPosition + Vector3Int.CeilToInt(v);
            foundLevels.AddRange(UnlockTilesFromPoint(newGridPosition, originMap, destinationMap, v, alreadyFound));
        }
        return foundLevels;
    }


    public TileBase GetTileAtPoint(Vector2 worldPoint)
    {
        Vector3Int gridPosiiton = unlockedPaths.WorldToCell((Vector3)worldPoint);

        return unlockedPaths.GetTile(gridPosiiton);
    }

    public LevelTile GetLevelTileAtPoint(Vector2 worldPoint)
    {
        Vector3Int gridPosiiton = levels.WorldToCell((Vector3)worldPoint);
        return levels.GetTile(gridPosiiton) as LevelTile;
    }

    public bool IsPositionStoppingPoint(Vector2 position)
    {
        Vector3Int cellPosition = unlockedPaths.WorldToCell(position);
        TileBase tile = unlockedPaths.GetTile(cellPosition);
        ConnectionDirections connections = ConnectionsFromTile[tile];
        int numConnections = 0;
        if ((connections & ConnectionDirections.Up) != 0) numConnections++;
        if ((connections & ConnectionDirections.Down) != 0) numConnections++;
        if ((connections & ConnectionDirections.Left) != 0) numConnections++;
        if ((connections & ConnectionDirections.Right) != 0) numConnections++;

        return numConnections > 2 || levels.GetTile(cellPosition) != null;
    }

    private Vector2 GetCellCenter(Vector3Int cellPosition)
    {
        Vector2 position = unlockedPaths.CellToWorld(cellPosition);
        Vector2 adjustedPosition = position + (Vector2)unlockedPaths.cellSize / 2;

        return adjustedPosition;
    }

    private ConnectionDirections DirectionFromVector(Vector2 vector)
    {
        if (vector == Vector2.up)
        {
            return ConnectionDirections.Up;
        }
        if (vector == Vector2.down)
        {
            return ConnectionDirections.Down;
        }
        if (vector == Vector2.left)
        {
            return ConnectionDirections.Left;
        }
        if (vector == Vector2.right)
        {
            return ConnectionDirections.Right;
        }

        return ConnectionDirections.None;
    }

    private List<Vector2> VectorsFromDirection(ConnectionDirections direction)
    {
        List<Vector2> vectors = new();
        if ((direction & ConnectionDirections.Up) != 0)
        {
            vectors.Add(Vector2.up);
        }
        if ((direction & ConnectionDirections.Down) != 0)
        {
            vectors.Add(Vector2.down);
        }
        if ((direction & ConnectionDirections.Left) != 0)
        {
            vectors.Add(Vector2.left);
        }
        if ((direction & ConnectionDirections.Right) != 0)
        {
            vectors.Add(Vector2.right);
        }
        return vectors;
    }

    // Update is called once per frame
    void Update()
    {

    }
}

