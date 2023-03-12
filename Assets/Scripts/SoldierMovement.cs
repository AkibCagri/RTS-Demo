using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierMovement : MonoBehaviour
{
    [SerializeField] float speed;
    PathManager pathManager;
    List<Vector3> pathPoints;
    int currentPathIndex;
    Vector3 direction;
    [SerializeField] float stopDistance = 1;
    [SerializeField] Transform visual;
    [SerializeField] float lookLerpValue;
    Vector2 lastTargetPos;
    public Vector3 doorPoint, spawnPoint;
    bool isSpawnMovement = true;
    [SerializeField] bool dontSpawnMovement;
    SoldierController myController;

    private void Awake()
    {
        myController = GetComponent<SoldierController>();
    }

    void Start()
    {
        pathManager = ReferansHolder.instance.pathManager;
        pathManager.obstacleEvent.AddListener(SetNewDestination);

        // This prevent start movement.
        if (dontSpawnMovement)
        {
            FinishSpawnMovement();
            StopMoving();
        }
    }

    void Update()
    {
        GoDestination();
        SpawnMovement();
    }

    // This function return stop distance.
    public float GetStopDistance()
    {
        return stopDistance;
    }

    // This funciton handle spawn movement when soldier instantiate
    public void SpawnMovement()
    {
        if (isSpawnMovement == false)
            return;

        direction = (doorPoint - transform.position).normalized;
        LookAtDirection(direction);
        transform.Translate(direction * speed * Time.deltaTime, Space.World);


        if (Vector3.Distance(doorPoint, transform.position) < stopDistance)
        {
            myController.myState = SoldierController.State.Moving;
            SetDestination(spawnPoint, false);
            FinishSpawnMovement();
        }
    }

    void FinishSpawnMovement()
    {
        isSpawnMovement = false;
        GetComponent<Collider2D>().enabled = true;
    }



    // This funtion set target destination.
    public void SetDestination(Vector3 targetPos, bool dontClearCell)
    {
        currentPathIndex = 0;
        if (dontClearCell == false)
            pathManager.grid.SetValue(transform.position, false);
        pathManager.InvokeObstacleEvent();
        lastTargetPos = targetPos;
        StartCoroutine(GetWayPointEnum(lastTargetPos));
    }

    // This coroutine wait for other soldiers obstacle invokes.
    IEnumerator GetWayPointEnum(Vector3 targetPos)
    {
        yield return null;
        pathPoints = pathManager.GetWayPoints(transform.position, targetPos);
    }

    // When obstacle event of path manager invoke, this funcition is called.
    void SetNewDestination()
    {
        if (pathPoints == null)
            return;
        //StopMoving();
        pathPoints = null;
        SetDestination(lastTargetPos, true);
    }

    // This fonction stop soldier.
    public void StopMoving()
    {
        pathPoints = null;

        pathManager.grid.SetValue(transform.position, true);
        pathManager.InvokeObstacleEvent();
    }

    // This function provides movement.
    void GoDestination()
    {
        if (pathPoints == null)
        {
            return;
        }

        if (pathPoints.Count == 0 || myController.myState != SoldierController.State.Moving)
        {
            StopMoving();
            return;
        }

        Vector3 targetPos = pathPoints[currentPathIndex];
        if (Vector3.Distance(transform.position, targetPos) > stopDistance)
        {
            direction = (targetPos - transform.position).normalized;
            LookAtDirection(direction);
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
        else
        {
            currentPathIndex++;
            if (currentPathIndex >= pathPoints.Count)
            {
                StopMoving();
                myController.myState = SoldierController.State.Idle;

            }
        }
    }

    // This function provides looking in the direction.
    public void LookAtDirection(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        visual.rotation = Quaternion.Slerp(visual.rotation, rotation, lookLerpValue * Time.deltaTime);
    }

    // This function make empty to current grid.
    public void DestroyMe()
    {
        pathManager.grid.SetValue(transform.position, false);
    }
}
