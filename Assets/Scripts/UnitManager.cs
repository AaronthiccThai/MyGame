using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
// TODO - State manager
public class UnitManager : MonoBehaviour
{
    [Header("Unit Storage")]
    private List<Unit> enemyUnits = new List<Unit>();
    private List<Unit> playerUnits = new List<Unit>();

    [Header("References")]
    public Tilemap tilemap;
    public GameManager gameManager;

    public void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        tilemap = FindFirstObjectByType<Tilemap>();
    }
    private void Start()
    {
        
    }
    public void BuildUnitsFromScene()
    {
        Unit[] units = FindObjectsByType<Unit>(FindObjectsSortMode.None);
        foreach (Unit u in units)
        {
            RegisterUnit(u);
            //Debug.Log(units.Length);

        }
    }


    // UNIT is not moving because the registering UNIT from scene isn't working or snapping;not sure
    // Only when placing unit manually does it work
    // IT adds the unit
    public void RegisterUnit(Unit u)
    {
        Vector3Int gridPos = tilemap.WorldToCell(u.transform.position);
        u.currentTilePos = gridPos;
        TileInstance t = gameManager.GetTileAtPosition(gridPos);
        // Snapping Unit to center of tile
        Vector3 snappedPos = tilemap.GetCellCenterWorld(gridPos);
        snappedPos.z = 0; // ensure 2D
        u.transform.position = snappedPos;

        if (t != null && t.isWalkable)
        {
            t.isOccupied = true;
            t.unit = u;
            //Debug.Log("Unit name: " + t.unit.name + "| Is occupied: " + t.isOccupied);
        }

        if (u.team == Unit.Team.Friendly)
        {
            playerUnits.Add(u);
            //Debug.Log("PLAYERUNITS COUNT: " + playerUnits.Count);
        }
        else if (u.team == Unit.Team.Enemy)
        {
            enemyUnits.Add(u);
            //Debug.Log("ENEMYUNITS COUNT: " + enemyUnits.Count);
        }

    }
    public void AddUnitToList(Unit u)
    {
        if (u.team == Unit.Team.Friendly)
        {
            playerUnits.Add(u);
        }
        else if (u.team == Unit.Team.Enemy)
        {
            enemyUnits.Add(u);
        }
    }

    public void RemoveUnitFromList(Unit u)
    {
        if (playerUnits.Contains(u))
            playerUnits.Remove(u);

        if (enemyUnits.Contains(u))
            enemyUnits.Remove(u);

        // Also clear tile occupancy
        TileInstance tile = gameManager.GetTileAtPosition(u.currentTilePos);
        if (tile != null)
        {
            tile.unit = null;
            tile.isOccupied = false;
        }
    }
    public void MoveAllPlayerUnits()
    {
        foreach (Unit u in playerUnits)
        {
            //Debug.Log("Calling Move all player units");
            u.MoveTowardClosestEnemy(u);
        }
    }
    public void AttackWithAllPlayerUnits()
    {
        foreach (Unit u in playerUnits)
        {
            //Debug.Log("Calling Attack for all player units");
            u.AttackClosestEnemy(u);
        }
    }

    public void EnemyTurnActions()
    {
        foreach (Unit u in enemyUnits)
        {
            u.EnemyUnitTurn();
        }
    }


}
