using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UnitCard : MonoBehaviour
{
    UnitSelectionManager unitSelectionManager;
    public Image unitImage;             
    private GameObject unitPrefab;      

    void Awake()
    {
        unitSelectionManager = FindFirstObjectByType<UnitSelectionManager>();
    }

    public void Initialise(GameObject prefab)
    {
        unitPrefab = prefab;

        SpriteRenderer sr = prefab.GetComponentInChildren<SpriteRenderer>();
        if (sr != null && unitImage != null)
            unitImage.sprite = sr.sprite;
        else
            Debug.LogWarning($"Prefab {prefab.name} has no SpriteRenderer or UnitCard has no Image.");

        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClick);
    }

    // invoked when the UI button is clicked
    public void OnClick()
    {
        if (unitPrefab == null) return;
        unitSelectionManager.SelectUnit(unitPrefab); 
    }
}
