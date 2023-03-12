using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBehaviour : MonoBehaviour
{
    [SerializeField] List<Transform> gridPoints = new List<Transform>();
    void Start()
    {
        PlaceMe();
    }

    // This code sets the grid cells covered by the obstacle to be full and invokes the obstacle event because the grid states change.
    void PlaceMe()
    {
        foreach (Transform t in gridPoints)
        {
            ReferansHolder.instance.pathManager.grid.SetValue(t.position, true);
            ReferansHolder.instance.pathManager.InvokeObstacleEvent();
        }
    }
}
