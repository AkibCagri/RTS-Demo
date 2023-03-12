using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    GameObject currentSelection;
    Vector2 selectionStartPos;
    bool isSelecting;
    List<SoldierController> selectedSoldiers = new List<SoldierController>();
    [SerializeField] Transform selectionArea;
    [SerializeField] int maxSoldierSelectionCount = 25;
    [SerializeField] int maxCountInRow = 5;
    private void Awake()
    {
        ReferansHolder.instance.selectionManager = this;
    }

    private void Update()
    {
        HandleSelection();
        HandleSoldiersMovement();
        SelectTargetEnemy();    
    }

    // This function select target enemy.
    void SelectTargetEnemy()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, 0.1f);
            List<Transform> enemies = new List<Transform>();

            foreach (Collider2D hit in hits)
            {
                ITeam currentITeam = hit.GetComponent<ITeam>();
                if (currentITeam != null)
                {
                    if (currentITeam.GetTeamIndex() != 0)
                    {
                        enemies.Add(hit.transform);
                    }
                }
            }

            Transform target;
            if (enemies.Count > 0)
                target = enemies[0];
            else
                target = null;

            for (int i = 0; i < selectedSoldiers.Count; i++)
            {
                selectedSoldiers[i].target = target;
            }
        }
    }


    // This function handles the movement of soldiers and makes them stop in order.
    void HandleSoldiersMovement()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            for (int i = 0; i < selectedSoldiers.Count; i++)
            {
                int rowMultiplier = 1;
                int rowMod = i % maxCountInRow;
                int rowIndex = Mathf.CeilToInt((float)rowMod / 2);
                int columnMod = Mathf.FloorToInt((float)i / maxCountInRow);
                int columnIndex = Mathf.CeilToInt((float)columnMod / 2);
                int columnMultiplier = 1;
                if (i % 2 == 0)
                {
                    rowMultiplier = -1;
                }

                if (columnMod % 2 == 1)
                {
                    columnMultiplier = -1;
                }

                float cellSize = ReferansHolder.instance.pathManager.GetCellSize();
                Vector2 destinationPoint = worldPos + new Vector2(rowIndex * rowMultiplier * cellSize, columnIndex * columnMultiplier * cellSize);

                selectedSoldiers[i].SetDestination(destinationPoint);

            }
        }
    }

    // This function selection handle soldier selection
    void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                Vector2 mousePos = Input.mousePosition;
                selectionStartPos = Camera.main.ScreenToWorldPoint(mousePos);
                selectionArea.gameObject.SetActive(true);
                isSelecting = true;
            }
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            selectionArea.transform.position = selectionStartPos;
            selectionArea.transform.localScale = worldPos - selectionStartPos;
        }

        if (Input.GetMouseButtonUp(0) && isSelecting)
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 selectionEndPos = Camera.main.ScreenToWorldPoint(mousePos);
            selectionArea.gameObject.SetActive(false);
            Collider2D[] selectedColliders = Physics2D.OverlapAreaAll(selectionStartPos, selectionEndPos);

            foreach (SoldierController item in selectedSoldiers)
            {
                item.DeselectMe();
            }
            selectedSoldiers.Clear();

            foreach (Collider2D collider in selectedColliders)
            {
                SoldierController controller = collider.GetComponent<SoldierController>();
                if (selectedSoldiers.Count >= maxSoldierSelectionCount)
                    break;
                if (controller != null)
                {
                    if (controller.GetTeamIndex() != 0)
                        continue;

                    selectedSoldiers.Add(controller);
                    controller.SelectMe();
                }
            }

            if (selectedSoldiers.Count > 0)
            {
                DeselectBuilding();
            }

            isSelecting = false;
        }
    }

    // This fuction remove removed soldier form selected soldiers
    public void RemoveSoldierFromSelectedList(SoldierController removedSoldier)
    {
        selectedSoldiers.Remove(removedSoldier);
    }

    // This function set building selection and set information panel according to building.
    public void SetBuildingSelection(GameObject selection, BuildingBehaviour.BuildingType type)
    {
        DeselectBuilding();

        currentSelection = selection;
        BuildingBehaviour currentBuilding = currentSelection.GetComponent<BuildingBehaviour>();
        ReferansHolder.instance.controllerUI.SetInformationPanel(currentBuilding, type);
    }

    // This function deselect building and turn off information panel.
    void DeselectBuilding()
    {
        if (currentSelection != null)
            currentSelection.GetComponent<BuildingBehaviour>().DeselectMe();

        ReferansHolder.instance.controllerUI.TurnOffInformationPanel();
    }

    // This function return current selection;
    public GameObject GetCurrentSelection()
    {
        return currentSelection;
    }

}
