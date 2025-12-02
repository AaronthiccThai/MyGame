using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public Tilemap tilemap;
    public TileInstance[,] tiles;
    public List<GameObject> testUnitPrefabs; // Testing purposes

    private int selectedUnitIndex = 0;
    public int funds; // Player funds for unit deployment
    void Start()
    {
        BuildGridFromScene();
    }
    private void Update()
    {
        HandleUnitSelection();
        if (Input.GetMouseButtonDown(0))
        {
            //checkTilesInfo();
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            TestPlaceUnit();

        }
    }

    /* 
        Function to Debug, check for tiles info on mouse click
    */
    private void checkTilesInfo()
    {

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPos = tilemap.WorldToCell(mouseWorldPos);
        BoundsInt bounds = tilemap.cellBounds;
        int x = gridPos.x - bounds.xMin;
        int y = gridPos.y - bounds.yMin;
        if (x >= 0 && x < tiles.GetLength(0) && y >= 0 && y < tiles.GetLength(1))
        {
            TileInstance clickedTile = tiles[x, y];
            if (clickedTile != null)
            {
                Debug.Log($"Clicked on tile at grid ({x},{y}) worldPos:{gridPos} - Walkable: {clickedTile.isWalkable} - TileType: {clickedTile.tileData.tileType}");
            }
            else
            {
                Debug.Log($"No tile data at grid ({x},{y}) worldPos:{gridPos}");
            }
        }
        else
        {
            Debug.Log("Clicked outside of tilemap bounds.");
        }
    }

    /*
        Function that builds the grid representation from the Tilemap in the scene
    */
    private void BuildGridFromScene()
    {
        BoundsInt bounds = tilemap.cellBounds;
        int width = bounds.size.x;
        int height = bounds.size.y;

        tiles = new TileInstance[width, height];
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            Tile tile = tilemap.GetTile<Tile>(pos);
            if (tile == null) continue;

            //Debug.Log($"Tile at {pos}: {tile.name}");
            int x = pos.x - bounds.xMin;
            int y = pos.y - bounds.yMin;

            TileInstance instance = new TileInstance
            {
                gridPos = pos,
                tileData = tile as CustomTile,
                isWalkable = tile is CustomTile ct ? ct.defaultWalkable : true,
                isOccupied = false,
                unit = null
            };

            tiles[x, y] = instance;
            //Debug.Log($"Stored tile at ({x},{y}) worldPos:{pos}");

        }
    }
    /*
        Function that returns the TileInstance at a given grid position
    */
    public TileInstance GetTileAtPosition(Vector3Int pos)
    {
        BoundsInt bounds = tilemap.cellBounds;

        int x = pos.x - bounds.xMin;
        int y = pos.y - bounds.yMin;

        if (x < 0 || y < 0 ||
            x >= tiles.GetLength(0) ||
            y >= tiles.GetLength(1))
            return null;

        return tiles[x, y];
    }


    /*
       Function that handles unit selection from UI and placement on the grid
    */
    private void HandleUnitSelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedUnitIndex = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) selectedUnitIndex = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) selectedUnitIndex = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4)) selectedUnitIndex = 3;
        if (Input.GetKeyDown(KeyCode.Alpha5)) selectedUnitIndex = 4;

        selectedUnitIndex = Mathf.Clamp(selectedUnitIndex, 0, testUnitPrefabs.Count - 1);

        Debug.Log("Selected unit index = " + selectedUnitIndex);
    }

    /*
        Function that handles placing a unit on the grid
    */
    private void PlaceUnit(Unit unit, Vector3Int gridPos)
    {
        if (unit == null)
        {
            Debug.LogError("Why is place unit null???");
            return;
        }
        BoundsInt bounds = tilemap.cellBounds;
        int x = gridPos.x - bounds.xMin;
        int y = gridPos.y - bounds.yMin;
        if (x  < 0 || x >= tiles.GetLength(0) || y < 0 || y >= tiles.GetLength(1))
        {
            Debug.LogError("Tried to place unit outside of grid");
            return;
        }
        TileInstance tile = tiles[x, y];
        if (tile == null || !tile.isWalkable)
        {
            Debug.LogError("Tile is not walkable, cannot place unit");
            return;
        }
        if (tile.isOccupied)
        {
            Debug.LogError("Tile is already occupied, cannot place unit");
            return;
        }
        tile.unit = unit;
        tile.isOccupied = true;
            
        unit.currentTilePos = gridPos;
        Vector3 worldPos = tilemap.GetCellCenterWorld(gridPos);

        worldPos.z = 0f;  

        unit.transform.position = worldPos;

        Debug.Log("Placed unit at " + gridPos + " worldPos:" + worldPos);
    }

    private void TestPlaceUnit()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPos = tilemap.WorldToCell(mouseWorldPos);

        BoundsInt bounds = tilemap.cellBounds;
        int x = gridPos.x - bounds.xMin;
        int y = gridPos.y - bounds.yMin;

        // Outside grid bounds
        if (x < 0 || x >= tiles.GetLength(0) || y < 0 || y >= tiles.GetLength(1))
            return;

        TileInstance tile = tiles[x, y];
        if (tile == null || !tile.isWalkable || tile.isOccupied)
            return;

        // Only now instantiate
        GameObject obj = Instantiate(testUnitPrefabs[selectedUnitIndex]);
        Unit unit = obj.GetComponent<Unit>();

        PlaceUnit(unit, gridPos);
    }



}
