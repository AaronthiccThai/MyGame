using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public Tilemap tilemap;
    public TileInstance[,] tiles;
    public List<GameObject> testUnitPrefabs;
    public UnitManager unitManager;

    private int selectedUnitIndex = 0;
    public int funds;

    private void Awake()
    {
        unitManager = FindFirstObjectByType<UnitManager>();
        tilemap = FindFirstObjectByType<Tilemap>();
    }

    void Start()
    {
        BuildGridFromScene();
        unitManager.BuildUnitsFromScene();
    }

    private void Update()
    {
        HandleUnitSelection();
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            checkTilesInfo();
            TestPlaceUnit();
        }
    }

    /* Debug: Info on clicked tile */
    private void checkTilesInfo()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPos = tilemap.WorldToCell(mouseWorldPos);
        gridPos.z = 0;

        BoundsInt bounds = tilemap.cellBounds;
        int x = gridPos.x - bounds.xMin;
        int y = gridPos.y - bounds.yMin;

        if (x >= 0 && x < tiles.GetLength(0) && y >= 0 && y < tiles.GetLength(1))
        {
            TileInstance clickedTile = tiles[x, y];
            if (clickedTile != null)
            {
                Debug.Log($"Clicked on tile at grid ({x},{y}) worldPos:{gridPos} deployable: {clickedTile.isDeployable}");
            }
            else Debug.Log($"No tile data at grid ({x},{y}) worldPos:{gridPos}");
        }
        else Debug.Log("Clicked outside of tilemap bounds.");
    }

    /* Build grid from tilemap */
    private void BuildGridFromScene()
    {
        BoundsInt bounds = tilemap.cellBounds;
        int width = bounds.size.x;
        int height = bounds.size.y;

        tiles = new TileInstance[width, height];

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            Vector3Int fixedPos = pos;
            fixedPos.z = 0;   

            Tile tile = tilemap.GetTile<Tile>(fixedPos);
            if (tile == null) continue;

            int x = fixedPos.x - bounds.xMin;
            int y = fixedPos.y - bounds.yMin;

            TileInstance instance = new TileInstance
            {
                gridPos = fixedPos,   
                tileData = tile as CustomTile,
                isWalkable = tile is CustomTile ct ? ct.defaultWalkable : true,
                isOccupied = false,
                isDeployable = tile is CustomTile ct2 ? ct2.defaultDeployable : false,
                unit = null
            };

            tiles[x, y] = instance;
        }
    }

    /* Return TileInstance at tilemap position */
    public TileInstance GetTileAtPosition(Vector3Int pos)
    {
        pos.z = 0;  

        BoundsInt bounds = tilemap.cellBounds;
        int x = pos.x - bounds.xMin;
        int y = pos.y - bounds.yMin;

        if (x < 0 || y < 0 ||
            x >= tiles.GetLength(0) ||
            y >= tiles.GetLength(1))
            return null;

        return tiles[x, y];
    }

    /* Handle selecting unit prefab */
    private void HandleUnitSelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2)) selectedUnitIndex = 0;
        if (Input.GetKeyDown(KeyCode.Alpha3)) selectedUnitIndex = 1;
        if (Input.GetKeyDown(KeyCode.Alpha4)) selectedUnitIndex = 2;
        if (Input.GetKeyDown(KeyCode.Alpha5)) selectedUnitIndex = 3;
        if (Input.GetKeyDown(KeyCode.Alpha6)) selectedUnitIndex = 4;

        selectedUnitIndex = Mathf.Clamp(selectedUnitIndex, 0, testUnitPrefabs.Count - 1);
    }

    /* Place a unit on a tile */
    private void PlaceUnit(Unit unit, Vector3Int gridPos)
    {
        if (unit == null) return;
        if (unit.deploymentCost > funds)
        {
            Debug.Log("Not enough funds to deploy unit.");
            return;
        }
        gridPos.z = 0;  

        BoundsInt bounds = tilemap.cellBounds;
        int x = gridPos.x - bounds.xMin;
        int y = gridPos.y - bounds.yMin;

        if (x < 0 || x >= tiles.GetLength(0) || y < 0 || y >= tiles.GetLength(1)) return;

        TileInstance tile = tiles[x, y];
        if (tile == null || !tile.isWalkable || tile.isOccupied || !tile.isDeployable) return;

        tile.unit = unit;
        tile.isOccupied = true;
        unit.currentTilePos = gridPos;

        Vector3 worldPos = tilemap.GetCellCenterWorld(gridPos);
        worldPos.z = 0f;   

        unit.transform.position = worldPos;
        unitManager.AddUnitToList(unit);
        funds -= unit.deploymentCost;
    }

    /* Test placing unit with left click */
    private void TestPlaceUnit()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPos = tilemap.WorldToCell(mousePos);
        gridPos.z = 0;

        BoundsInt bounds = tilemap.cellBounds;
        int x = gridPos.x - bounds.xMin;
        int y = gridPos.y - bounds.yMin;

        // Out of bounds
        if (x < 0 || x >= tiles.GetLength(0) || y < 0 || y >= tiles.GetLength(1))
            return;

        TileInstance tile = tiles[x, y];

        // Tile not placeable
        if (tile == null || !tile.isWalkable || tile.isOccupied)
            return;

        // Get the unit prefab cost WITHOUT instantiating
        Unit prefabUnit = testUnitPrefabs[selectedUnitIndex].GetComponent<Unit>();

        if (prefabUnit.deploymentCost > funds)
        {
            Debug.Log("Not enough funds.");
            return;
        }

        // All checks passed, now instantiate
        GameObject obj = Instantiate(testUnitPrefabs[selectedUnitIndex]);
        Unit unit = obj.GetComponent<Unit>();

        PlaceUnit(unit, gridPos);
    }

}
