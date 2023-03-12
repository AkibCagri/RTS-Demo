using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryManager : MonoBehaviour
{
    [SerializeField] GameObject barracksSilhouette;
    [SerializeField] GameObject powerPlantSilhouette;
    [SerializeField] GameObject towerSilhouette;

    [SerializeField] GameObject barracksPrefab;
    [SerializeField] GameObject powerPlantsPrefab;
    [SerializeField] GameObject towerPrefab;

    [SerializeField] List<GameObject> soldierPrefab = new List<GameObject>();

    private void Awake()
    {
        ReferansHolder.instance.factoryManager = this;
    }

    // This function instantiate building silhouette according to type of building.
    public GameObject InstantiateSilhouette(BuildingBehaviour.BuildingType type)
    {
        GameObject newSilhouette = null;
        switch (type)
        {
            case BuildingBehaviour.BuildingType.Barracks:
                newSilhouette = Instantiate(barracksSilhouette);
                break;
            case BuildingBehaviour.BuildingType.PowerPlant:
                newSilhouette = Instantiate(powerPlantSilhouette);
                break;
            case BuildingBehaviour.BuildingType.Tower:
                newSilhouette = Instantiate(towerSilhouette);
                break;
        }
        return newSilhouette;
    }

    // This function instantiate building according to type of building.
    public GameObject InstantiateBuilding(Vector2 pos, BuildingBehaviour.BuildingType type)
    {
        GameObject newBuilding = null;
        switch (type)
        {
            case BuildingBehaviour.BuildingType.Barracks:
                newBuilding = Instantiate(barracksPrefab, pos, Quaternion.identity);
                break;
            case BuildingBehaviour.BuildingType.PowerPlant:
                newBuilding = Instantiate(powerPlantsPrefab, pos, Quaternion.identity);
                break;
            case BuildingBehaviour.BuildingType.Tower:
                newBuilding = Instantiate(towerPrefab, pos, Quaternion.identity);
                break;
        }
        return newBuilding;
    }

    // This function instantiate soldier according to index of soldier and set the soldier properties.
    public GameObject InstantiateSoldier(Vector2 pos, Vector2 doorPoint, Vector2 spawnPoint, int index)
    {
        GameObject newSoldier = Instantiate(soldierPrefab[index], pos, Quaternion.identity);
        newSoldier.GetComponent<SoldierController>().SetStart(doorPoint, spawnPoint);
        return newSoldier;
    }
}
