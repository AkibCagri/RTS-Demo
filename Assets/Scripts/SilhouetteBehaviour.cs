using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilhouetteBehaviour : MonoBehaviour
{
    [SerializeField] Transform cursorPoint;
    Vector2 cursorOffset;

    [SerializeField] Color placeableColor, notPleaceableColor;
    bool cornerPlaceable = true;
    bool colliderPlaceable = true;
    bool isPleaceable = true;

    [SerializeField] SpriteRenderer visualRenderer;
    Grid grid;
    [SerializeField] BuildingBehaviour.BuildingType myType;

    List<GameObject> collidingObjects = new List<GameObject>();
    [SerializeField] List<Transform> cornerPoints = new List<Transform>();
    Vector3 oldPosition;

    void Start()
    {
        grid = ReferansHolder.instance.pathManager.grid;
        cursorOffset = cursorPoint.position - transform.position;

        SetPos();
    }

    void Update()
    {
        SetPos();

        // If transfom position and old position are not same, this code check placeable or not.
        if (transform.position != oldPosition)
        {
            CheckCorners();
            CheckPlaceable();
        }

        // if mouse is not over the UI, this code place or break the silhouette.
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                PlaceMe();
            }
            if (Input.GetMouseButtonDown(1))
            {
                BreakMe();
            }
        }

        oldPosition = transform.position;
    }

    // This function check corners of building grid whether full or empty especially out of grid.
    void CheckCorners()
    {
        cornerPlaceable = true;
        foreach (Transform t in cornerPoints)
        {
            if (grid.CheckValue(t.position))
            {
                cornerPlaceable = false;
                break;
            }
        }
    }

    // This function set position.
    void SetPos()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 gridPos = grid.WorldToGridPosition(worldPos);
        Vector2 gridToWorldPos = grid.GridToWorldPosition((int)gridPos.x, (int)gridPos.y, true);

        transform.position = gridToWorldPos - cursorOffset;
    }

    // if 'isPleaceable' is true, this function instantiate building at the silhouette position.
    void PlaceMe()
    {
        if (!isPleaceable)
        {
            return;
        }

        ReferansHolder.instance.factoryManager.InstantiateBuilding(transform.position, myType);
        Destroy(gameObject);
        ReferansHolder.instance.controllerUI.SetCurrentSilhouette(null);
    }

    // This function destroy silhouette.
    public void BreakMe()
    {
        Destroy(gameObject);
        ReferansHolder.instance.controllerUI.SetCurrentSilhouette(null);
    }

    // This function check placeabe or not.
    void CheckPlaceable()
    {
        if (cornerPlaceable && colliderPlaceable)
            isPleaceable = true;
        else
        isPleaceable = false;

        if (isPleaceable)
            visualRenderer.color = placeableColor;
        else
            visualRenderer.color = notPleaceableColor;
    }

    // If silhouette enter colliding with other collider, 'colliderPlaceable' is false.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Building") || collision.CompareTag("Soldier"))
        {
            if (collidingObjects.Contains(collision.gameObject) == false)
            {
                collidingObjects.Add(collision.gameObject);
            }

            colliderPlaceable = false;
            CheckPlaceable();
        }
    }

    // If silhouette exit colliding with other collider and if colliding object count is zero, 'colliderPlaceable' is true.
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Building") || collision.CompareTag("Soldier"))
        {
            if (collidingObjects.Contains(collision.gameObject))
            {
                collidingObjects.Remove(collision.gameObject);
            }

            if (collidingObjects.Count == 0)
            {
                colliderPlaceable = true;
                CheckPlaceable();
            }
        }
    }
}
