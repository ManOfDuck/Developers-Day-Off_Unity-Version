using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class DecorationTile : RuleTile
{
    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject instantiatedGameObject)
    {
        bool succeeeded = base.StartUp(position, tilemap, instantiatedGameObject);

        if (instantiatedGameObject)
        {
            instantiatedGameObject.transform.rotation = tilemap.GetTransformMatrix(position).rotation;
        }

        return succeeeded;
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref UnityEngine.Tilemaps.TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);

        tileData.flags = TileFlags.None;
    }
}
