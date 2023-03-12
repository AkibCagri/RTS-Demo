using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPlantBehaviour : BuildingBehaviour
{
    protected override void Start()
    {
        base.Start();
        myType = BuildingType.PowerPlant;
    }
}
