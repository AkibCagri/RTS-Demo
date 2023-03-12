using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ControllerUI : MonoBehaviour
{
    ModelUI model;

    private void Awake()
    {
        ReferansHolder.instance.controllerUI = this;
        model = GetComponent<ModelUI>();
    }

    // This function sends the request to instantiate a silhouette of the barracks to the factory manager.
    public void BarracksButton()
    {
        if (model.currentSilhouette != null)
            model.currentSilhouette.GetComponent<SilhouetteBehaviour>().BreakMe();

        SetCurrentSilhouette(ReferansHolder.instance.factoryManager.InstantiateSilhouette(BuildingBehaviour.BuildingType.Barracks));
    }

    // This function sends the request to instantiate a soldier to the factory manager with the soldier properties.
    public void SoldierButton(int index)
    {
        Transform currentBuilding = ReferansHolder.instance.selectionManager.GetCurrentSelection().transform;
        Vector2 pos = currentBuilding.transform.position;
        BarracksBehaviour barracksBehaviour = currentBuilding.GetComponent<BarracksBehaviour>();
        Vector2 doorPoint = barracksBehaviour.GetDoorPoint();
        Vector2 spawnPoint = barracksBehaviour.GetSpawnPoint();
        ReferansHolder.instance.factoryManager.InstantiateSoldier(pos, doorPoint, spawnPoint, index);
    }

    // This function sends the request to instantiate a silhouette of the power plant to the factory manager.
    public void PowerPlantButton()
    {
        if (model.currentSilhouette != null)
            model.currentSilhouette.GetComponent<SilhouetteBehaviour>().BreakMe();

        SetCurrentSilhouette(ReferansHolder.instance.factoryManager.InstantiateSilhouette(BuildingBehaviour.BuildingType.PowerPlant));
    }

    // This function sends the request to instantiate a silhouette of the tower to the factory manager.
    public void TowerButton()
    {
        if (model.currentSilhouette != null)
            model.currentSilhouette.GetComponent<SilhouetteBehaviour>().BreakMe();

        SetCurrentSilhouette(ReferansHolder.instance.factoryManager.InstantiateSilhouette(BuildingBehaviour.BuildingType.Tower));
    }

    // This function set current silhouette of model.
    public void SetCurrentSilhouette(GameObject currentSilhouette)
    {
        model.currentSilhouette = currentSilhouette;
    }

    // This funtion set information panel.
    public void SetInformationPanel(BuildingBehaviour currentBuilding, BuildingBehaviour.BuildingType type)
    {
        model.buildingPanel.SetActive(true);
        string infoName;
        Sprite infoImage;
        GetInfoProperties(type, out infoName, out infoImage);
        model.infoName.text = infoName;
        model.infoImage.sprite = infoImage;

        if (type == BuildingBehaviour.BuildingType.Barracks)
        {
            model.infoProductionPanel.SetActive(true);  
            model.infoBarracsPanel.SetActive(true);
        }
        else
        {
            model.infoProductionPanel.SetActive(false);
            model.infoBarracsPanel.SetActive(false);
        }
    }

    public void TurnOffInformationPanel()
    {
        model.buildingPanel.SetActive(false);
    }

    void GetInfoProperties(BuildingBehaviour.BuildingType type, out string infoName, out Sprite infoImage)
    {
        switch (type)
        {
            case BuildingBehaviour.BuildingType.Barracks:
                infoName = model.barracksInfoText;
                infoImage = model.barracksInfoImage;
                break;
            case BuildingBehaviour.BuildingType.PowerPlant:
                infoName = model.powerPlantInfoText;
                infoImage = model.powerPlantInfoImage;
                break;
            case BuildingBehaviour.BuildingType.Tower:
                infoName = model.towerInfoText;
                infoImage = model.towerInfoImage;
                break;
            default:
                infoName = "";
                infoImage = null;
                break;
        }
    }
}
