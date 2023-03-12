using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarracksBehaviour : BuildingBehaviour
{
    [SerializeField] Transform doorPoint;
    [SerializeField] Transform spawnPoint;
    protected override void Start()
    {
        base.Start();
        myType = BuildingType.Barracks;
    }

    // This function returns the position of the door point.
    public Vector2 GetDoorPoint()
    {
        return doorPoint.position;
    }

    // This function returns the position of the spawn point.
    public Vector2 GetSpawnPoint()
    {
        return spawnPoint.position;
    }
}
