using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class UnitSelectionManager : MonoBehaviour
{
    [Header("References")]
    public GameManager gameManager;
    public Tilemap tilemap;                  // Ground tilemap
    public Tilemap highlightTilemap;         // Overlay tilemap
    public Tile highlightValidTile;
    public Tile highlightInvalidTile;
    public Image unitPreviewImage;

    [Header("Units")]
    public List<GameObject> unitPrefabs;     

    private GameObject selectedUnitPrefab;
    private int previousSelectedIndex = -1;
    private bool isDeploying = false;
    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        unitPrefabs = gameManager.testUnitPrefabs;
    }

    // Update is called once per frame
    void Update()
    {
        HandleUnitSelection();
        UpdatePreviewPosition();
        if (!isDeploying) return;
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Clicked on UI, not placing unit.");
                return;

            }
            TryPlaceUnits();
        }
    }

    private void HandleUnitSelection()
    {
        // Keep track of previous to 'deselect' if needed
        if (Input.GetKeyDown(KeyCode.Alpha1)) TrySelectPrefabAtIndex(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TrySelectPrefabAtIndex(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TrySelectPrefabAtIndex(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) TrySelectPrefabAtIndex(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) TrySelectPrefabAtIndex(4);

        if (Input.GetMouseButtonDown(1))
            CancelDeployment();
    }
    private void TrySelectPrefabAtIndex(int i)
    {
        if (i < 0 || i >= unitPrefabs.Count) return;

        if (i == previousSelectedIndex)
        {
            CancelDeployment();
            previousSelectedIndex = -1;
            return;
        }

        previousSelectedIndex = i;
        SelectUnit(unitPrefabs[i]);
    }


    public void SelectUnit(GameObject selectedPrefab)
    {
        if (selectedPrefab == null) return;
        if (PauseManager.Instance != null && PauseManager.Instance.isPaused)
            return;
        selectedUnitPrefab = selectedPrefab;
        isDeploying = true;
        ShowPreview(selectedPrefab);
        ShowDeployableTiles();
    }

    private void CancelDeployment()
    {
        isDeploying = false;
        selectedUnitPrefab = null;
        ClearHighlights();
        HidePreview();
    }

    public void TryPlaceUnits()
    {
        if (PauseManager.Instance != null && PauseManager.Instance.isPaused)
            return;
        Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cell = tilemap.WorldToCell(world);
        cell.z = 0;

        TileInstance tile = gameManager.GetTileAtPosition(cell);

        if (tile == null || !tile.isDeployable || tile.isOccupied)
            return;

        Unit prefabUnit = selectedUnitPrefab.GetComponent<Unit>();
        if (prefabUnit.deploymentCost > gameManager.funds)
        {
            Debug.Log("Not enough funds.");
            return;
        }

        GameObject obj = Instantiate(selectedUnitPrefab);
        Unit newUnit = obj.GetComponent<Unit>();

        gameManager.PlaceUnit(newUnit, cell);

        CancelDeployment();
    }

    // Might turn this into show deployable tiles for THAT type of unit so red will also have other placeable tiles
    private void ShowDeployableTiles()
    {
        foreach (TileInstance t in gameManager.tiles)
        {
            if (t == null) continue;

            // Ensure tile exists inside the actual tilemap bounds
            if (!tilemap.HasTile(t.gridPos))
                continue;

            if (t.isDeployable && !t.isOccupied)
            {
                highlightTilemap.SetTile(t.gridPos, highlightValidTile);
            }
            else
            {
                highlightTilemap.SetTile(t.gridPos, highlightInvalidTile);
            }
        }
    }


    private void ClearHighlights()
    {
        highlightTilemap.ClearAllTiles();
    }

    public void ShowPreview(GameObject unitPrefab)
    {
        SpriteRenderer sr = unitPrefab.GetComponent<SpriteRenderer>();

        unitPreviewImage.sprite = sr.sprite;

        unitPreviewImage.gameObject.SetActive(true);
    }

    public void HidePreview()
    {
        unitPreviewImage.gameObject.SetActive(false);
    }

    public void UpdatePreviewPosition()
    {
        if (unitPreviewImage == null || !unitPreviewImage.gameObject.activeSelf)
            return;

        Vector3 mousePos = Input.mousePosition;
        unitPreviewImage.transform.position = mousePos;
    }
}
