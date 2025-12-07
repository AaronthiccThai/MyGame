using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Unit : MonoBehaviour
{
    [Header("Unit Stats")]
    public int maxHealth;
    public int currentHealth;
    public int attackDamage;
    public float attackRange;
    public float moveSpeed;
    public float armour;
    public int deploymentCost;

    [Header("References")]
    public Animator animator;
    public GameManager gameManager;
    public Tilemap tilemap;
    public UnitManager unitManager;
    [Header("Grid Position")]
    public Vector3Int currentTilePos;
    public static readonly Vector3Int[] oddr =
    {
        new(1, 0, 0), new(-1, 0, 0),
        new(0, 1, 0), new(0, -1, 0),
        new(1, 1, 0), new(1, -1, 0),
    };
    public enum Team
    {
        Friendly,
        Enemy
    }

    public Team team;
    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        tilemap = FindFirstObjectByType<Tilemap>();
        unitManager = FindFirstObjectByType<UnitManager>();
    }

    public void Start()
    {
        currentHealth = maxHealth;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            Unit closest = FindClosestEnemy(this);

            if (closest != null)
            {
                if (!inAttackRange(this, closest, attackRange))
                {
                    MoveTowardClosestEnemy(this);
                }


            }

        }
    }

    public virtual void TakeDamage(int amount)
    {
        // Change calc here
        float effectiveDamage = amount * (1 - armour / 100f);
        currentHealth -= Mathf.RoundToInt(effectiveDamage);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        Destroy(gameObject);
        unitManager.RemoveUnitFromList(this);
    }

    public Vector3Int OddrToCube(Vector3Int h)
    {
        int x = h.x - (h.y - (h.y & 1)) / 2;
        int z = h.y;
        int y = -x - z;

        return new Vector3Int(x, y, z);
    }
    public int HexDistance(Vector3Int a, Vector3Int b)
    {
        Vector3Int ac = OddrToCube(a);
        Vector3Int bc = OddrToCube(b);

        return Mathf.Max(
            Mathf.Abs(ac.x - bc.x),
            Mathf.Abs(ac.y - bc.y),
            Mathf.Abs(ac.z - bc.z)
        );
    }


    public Unit FindClosestEnemy(Unit self)
    {
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
        queue.Enqueue(self.currentTilePos);
        visited.Add(self.currentTilePos);

        while (queue.Count > 0)
        {
            Vector3Int currentPos = queue.Dequeue();
            TileInstance tile = gameManager.GetTileAtPosition(currentPos);
            // IT IS FINDING THE ENEMY
            if (tile.unit != null && tile.unit.team != self.team)
            {
                Debug.Log(tile.unit.name);
                return tile.unit;
            }
            foreach (Vector3Int neighbor in GetHexNeighbors(currentPos))
            {
                if (!visited.Contains(neighbor))
                {
                    TileInstance neighborTile = gameManager.GetTileAtPosition(neighbor);
                    if (neighborTile != null && neighborTile.isWalkable)
                    {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                    }
                }
            }


        }
        return null; 
    }
    // BUGFIX, ITS NOT FINDING THE PATH OR SMTH MAYBE
    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal)
    {
        Queue<Vector3Int> queue = new();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new();

        queue.Enqueue(start);
        cameFrom[start] = start;

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();

            if (current == goal)
                break;

            foreach (Vector3Int n in GetHexNeighbors(current))
            {
                if (cameFrom.ContainsKey(n)) continue;

                TileInstance t = gameManager.GetTileAtPosition(n);
                if (t != null && t.isWalkable)
                {
                    queue.Enqueue(n);
                    cameFrom[n] = current;
                }
            }
        }

        // Reconstruct path
        List<Vector3Int> path = new();
        Vector3Int p = goal;

        if (!cameFrom.ContainsKey(goal))
            return null; // no path

        while (p != start)
        {
            path.Add(p);
            p = cameFrom[p];
        }

        path.Reverse();
        return path;
    }
    // Units are fighting on top of each other, maybe do one tile before 
    public void MoveTowardClosestEnemy(Unit self)
    {
        Unit enemy = FindClosestEnemy(self);
        if (enemy == null) return;
        //Debug.Log("FOUND ENEMY" + enemy.name);

        Vector3Int targetTile = GetTileAdjacentTo(enemy);
        List<Vector3Int> path = FindPath(self.currentTilePos, targetTile);
      

        if (path == null || path.Count == 0) return;
        MoveUnitToTile(self, path[0]);
    }
    Vector3Int GetTileAdjacentTo(Unit enemy)
    {
        foreach (Vector3Int n in GetHexNeighbors(enemy.currentTilePos))
        {
            TileInstance t = gameManager.GetTileAtPosition(n);
            if (t != null && t.isWalkable && t.unit == null)
            {
                return n;
            }
        }

        return enemy.currentTilePos; 
    }

    public void MoveUnitToTile(Unit unit, Vector3Int newTilePos)
    {
        TileInstance newTile = gameManager.GetTileAtPosition(newTilePos);
        if (newTile == null || !newTile.isWalkable) return;
        if (newTile.unit != null) return;
        

        TileInstance oldTile = gameManager.GetTileAtPosition(unit.currentTilePos);
        if (oldTile != null) oldTile.unit = null;


        newTile.unit = unit;
        unit.currentTilePos = newTilePos;

        Vector3 worldPos = tilemap.CellToWorld(newTilePos);
        worldPos.z = 0; 
        unit.transform.position = worldPos;

        //if (unit.animator != null) unit.animator.SetTrigger("Move"); Placeholder for move animation
    }

    public IEnumerable<Vector3Int> GetHexNeighbors(Vector3Int pos)
    {
        bool isOdd = pos.y % 2 != 0;

        if (isOdd)
        {
            yield return pos + new Vector3Int(1, 0, 0);
            yield return pos + new Vector3Int(-1, 0, 0);
            yield return pos + new Vector3Int(0, 1, 0);
            yield return pos + new Vector3Int(0, -1, 0);
            yield return pos + new Vector3Int(1, 1, 0);
            yield return pos + new Vector3Int(1, -1, 0);
        }
        else
        {
            yield return pos + new Vector3Int(1, 0, 0);
            yield return pos + new Vector3Int(-1, 0, 0);
            yield return pos + new Vector3Int(0, 1, 0);
            yield return pos + new Vector3Int(0, -1, 0);
            yield return pos + new Vector3Int(-1, 1, 0);
            yield return pos + new Vector3Int(-1, -1, 0);
        }
    }

    public virtual bool inAttackRange(Unit a, Unit b, float range)
    {
        int dist = HexDistance(a.currentTilePos, b.currentTilePos);
        return dist <= range;
    }
    public void AttackClosestEnemy(Unit self)
    {
        Unit enemy = FindClosestEnemy(self);
        if (enemy == null) return;

        // Check range
        if (!inAttackRange(self, enemy, attackRange))
        {
            Debug.Log(self.name + " cannot reach " + enemy.name);
            return;
        }

        // Attack
        Debug.Log(self.name + " attacks " + enemy.name);

        enemy.TakeDamage(self.attackDamage);

        // Optional animation
        if (self.animator != null)
            self.animator.SetTrigger("Attack");

        if (enemy.currentHealth <= 0)
        {
            Debug.Log(enemy.name + " has been defeated!");
            Destroy(enemy);
        }
    }
    public void EnemyUnitTurn()
    {
        Unit closest = FindClosestEnemy(this);
        if (closest == null) return;

        // If in range, ATTACK
        if (inAttackRange(this, closest, attackRange))
        {
            AttackClosestEnemy(this);
        }
        else
        {
            // If NOT in range, MOVE
            MoveTowardClosestEnemy(this);
        }
    }


}

