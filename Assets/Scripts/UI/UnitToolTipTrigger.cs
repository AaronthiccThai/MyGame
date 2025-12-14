using UnityEngine;

public class UnitToolTipTrigger : MonoBehaviour
{
    private Unit unit;

    private void Awake()
    {
        unit = GetComponent<Unit>();
    }

    private void OnMouseEnter()
    {
        Debug.Log("UNIT TOOL TIP TRIGGER " + unit);
        if (unit == null) return;

        string stats =
            $"<b><size=20><color=#FFD700>{unit.unitName}</color></size></b>\n\n" +
            $"<color=#FF6A6A><b>Health:</b></color> {unit.currentHealth}/{unit.maxHealth}\n" +
            $"<color=#FF8C00><b>Damage:</b></color> {unit.attackDamage}\n" +
            $"<color=#87CEEB><b>Range:</b></color> {unit.attackRange}\n" +
            $"<color=#00FA9A><b>Speed:</b></color> {unit.moveSpeed}\n" +
            $"<color=#ADD8E6><b>Armour:</b></color> {unit.armour}";


        UnitToolTip.Instance.Show(stats);
        Debug.Log("UNIT STATS + " + stats);
    }

    private void OnMouseExit()
    {
        UnitToolTip.Instance.Hide();
    }
}
