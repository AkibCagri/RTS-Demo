using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingBehaviour : MonoBehaviour, IDestroyable, ITeam
{
    [SerializeField] protected int teamIndex;
    [SerializeField] protected List<Color> teamColors = new List<Color>();
    [SerializeField] List<Transform> gridPoints = new List<Transform>();
    [SerializeField] SpriteRenderer visual;
    [SerializeField] GameObject selectionIndicator;

    public enum BuildingType
    {
        Barracks,
        PowerPlant,
        Tower
    }
    protected BuildingType myType;

    protected virtual void Start()
    {
        PlaceMe();

        // This line set team color
        visual.color = teamColors[teamIndex];
    }

    // This code sets the grid cells covered by the building to be full and invokes the obstacle event because the grid states change.
    void PlaceMe()
    {
        foreach (Transform t in gridPoints)
        {
            ReferansHolder.instance.pathManager.grid.SetValue(t.position, true);
            ReferansHolder.instance.pathManager.InvokeObstacleEvent();
        }
    }

    // When clicked on the building, building is selected.
    private void OnMouseDown()
    {
        // When mouse on over the UI, building cannot be selected.
        if (EventSystem.current.IsPointerOverGameObject() == false)
            SelectMe();
    }

    // This function select building and turn on selection indicator.
    void SelectMe()
    {
        // if team index is not '0', building can not selected.
        if (teamIndex != 0)
            return;

        ReferansHolder.instance.selectionManager.SetBuildingSelection(gameObject, myType);
        selectionIndicator.SetActive(true);
    }

    // This function turn off selection indicator.
    public void DeselectMe()
    {
        selectionIndicator.SetActive(false);
    }

    // This function destroy game object and 
    public void DestroyMe()
    {
        Destroy(gameObject);
        foreach (Transform t in gridPoints)
        {
            ReferansHolder.instance.pathManager.grid.SetValue(t.position, false);
        }
        ReferansHolder.instance.pathManager.InvokeObstacleEvent();
    }

    // This function return team index.
    public int GetTeamIndex()
    {
        return teamIndex;
    }
}
