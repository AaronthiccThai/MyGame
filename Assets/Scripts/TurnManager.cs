using UnityEngine;
using System.Collections;

public enum TurnState { PlayerSelect, PlayerAction, EnemyAction }
public enum PlayerAction { March, Attack, Wait }

public class TurnManager : MonoBehaviour
{
    public UnitManager unitManager;

    private TurnState currentState;
    private PlayerAction selectedAction;

    void Awake()
    {
        unitManager = FindFirstObjectByType<UnitManager>();
    }

    void Start()
    {
        StartCoroutine(TurnLoop());
    }

    // ------------------------------------------------------------
    // MAIN TURN LOOP (Runs forever)
    // ------------------------------------------------------------
    private IEnumerator TurnLoop()
    {
        while (true)
        {
            // --------------------------
            // PLAYER SELECT PHASE
            // --------------------------
            currentState = TurnState.PlayerSelect;
            Debug.Log("=== PLAYER TURN: Select Action ===");

            // Wait for the UI button click to change the state
            yield return new WaitUntil(() => currentState == TurnState.PlayerAction);

            // --------------------------
            // PLAYER ACTION PHASE
            // --------------------------
            yield return StartCoroutine(ExecutePlayerActionRoutine());

            // Short delay after player turn
            yield return new WaitForSeconds(0.75f);

            // --------------------------
            // ENEMY ACTION PHASE
            // --------------------------
            currentState = TurnState.EnemyAction;
            yield return StartCoroutine(ExecuteEnemyActionRoutine());

            // Short delay after enemy turn
            yield return new WaitForSeconds(0.75f);
        }
    }

    // ------------------------------------------------------------
    // BUTTON CALLBACK
    // ------------------------------------------------------------
    public void OnPlayerActionButton(int actionIndex)
    {
        selectedAction = (PlayerAction)actionIndex;
        Debug.Log("Player selected: " + selectedAction);

        currentState = TurnState.PlayerAction;
    }

    // ------------------------------------------------------------
    // PLAYER ACTION ROUTINE
    // ------------------------------------------------------------
    private IEnumerator ExecutePlayerActionRoutine()
    {
        Debug.Log("=== PLAYER ACTION: " + selectedAction + " ===");

        switch (selectedAction)
        {
            case PlayerAction.March:
                unitManager.MoveAllPlayerUnits();
                break;

            case PlayerAction.Attack:
                unitManager.AttackWithAllPlayerUnits();
                break;

            case PlayerAction.Wait:
                // Do nothing
                break;
        }

        // If your unit actions have animations you can yield them here
        yield return null;
    }

    // ------------------------------------------------------------
    // ENEMY ACTION ROUTINE
    // ------------------------------------------------------------
    private IEnumerator ExecuteEnemyActionRoutine()
    {
        Debug.Log("=== ENEMY ACTION ===");

        unitManager.EnemyTurnActions();

        // If animations exist, yield for them here
        yield return null;
    }
}
