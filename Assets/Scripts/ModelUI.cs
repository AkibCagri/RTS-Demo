using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModelUI : MonoBehaviour
{
    // This script hold UI properties.
    [HideInInspector] public GameObject currentSilhouette;
    public GameObject buildingPanel;
    public TextMeshProUGUI infoName;
    public Image infoImage;
    public GameObject infoProductionPanel;
    public GameObject infoBarracsPanel;

    public string barracksInfoText = "BARRACKS";
    public string powerPlantInfoText = "POWER PLANT";
    public string towerInfoText = "TOWER";

    public Sprite barracksInfoImage;
    public Sprite powerPlantInfoImage;
    public Sprite towerInfoImage;
}
