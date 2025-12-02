using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType
{
    Land,
    Water,
}
[CreateAssetMenu(fileName = "New Custom Tile", menuName = "Tiles/Custom Tile")]
public class CustomTile : Tile
{
    public TileType tileType;
    public bool defaultWalkable = true;
}

