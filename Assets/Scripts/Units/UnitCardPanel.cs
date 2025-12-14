using UnityEngine;

public class UnitCardPanel : MonoBehaviour
{
    public GameObject cardPrefab;   // UI card template prefab (assign once in inspector)
    public Transform cardParent;    // content container (assign once)

    void Start()
    {
        var selection = FindFirstObjectByType<UnitSelectionManager>();
        if (selection == null || selection.gameManager == null)
        {
            Debug.LogError("Missing UnitSelectionManager or GameManager");
            return;
        }

        foreach (GameObject prefab in selection.gameManager.testUnitPrefabs)
        {
            GameObject card = Instantiate(cardPrefab, cardParent);
            UnitCard unitCard = card.GetComponent<UnitCard>();
            unitCard.Initialise(prefab);
        }
    }
}
