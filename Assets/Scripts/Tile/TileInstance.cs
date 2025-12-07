using UnityEngine;

public class TileInstance
{
    public Vector3Int gridPos;
    public CustomTile tileData;

    public bool isWalkable;
    public bool isOccupied;
    public bool isDeployable;
    public Unit unit;

}
