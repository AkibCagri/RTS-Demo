using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] float panSpeed = 20f;
    [SerializeField] float borderThickness = 10f;
    [SerializeField] Transform leftBorder, rightBorder;
    float topMapBorder, bottomMapBorder, leftMapBorder, rightMapBorder;

    private void Start()
    {
        PathManager pathManager = ReferansHolder.instance.pathManager;
        float borderOffset = pathManager.GetCellSize() * 5;

        // This code sets camera movement borders.
        bottomMapBorder = pathManager.transform.position.y + borderOffset;
        leftMapBorder = pathManager.transform.position.x + borderOffset;
        topMapBorder = pathManager.transform.position.y + pathManager.GetGridHeight() * pathManager.GetCellSize() - borderOffset;
        rightMapBorder = pathManager.transform.position.y + pathManager.GetGridWith() * pathManager.GetCellSize() - borderOffset;
    }

    void Update()
    {
        Movement();
    }

    // This code moves the camera
    void Movement()
    {
        Vector2 movement = Vector2.zero;

        // This code set movement vector.
        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - borderThickness)
        {
            movement += Vector2.up;
        }
        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= borderThickness)
        {
            movement += Vector2.down;
        }
        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= rightBorder.position.x - borderThickness && Input.mousePosition.x < rightBorder.position.x)
        {
            movement += Vector2.right;
        }
        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= leftBorder.position.x + borderThickness && Input.mousePosition.x > leftBorder.position.x)
        {
            movement += Vector2.left;
        }

        movement = movement.normalized;
        movement *= panSpeed * Time.deltaTime;

        transform.Translate(movement);

        // This code clamp the camera position in borders.
        float ClampX = Mathf.Clamp(transform.position.x, leftMapBorder, rightMapBorder);
        float ClampY = Mathf.Clamp(transform.position.y, bottomMapBorder, topMapBorder);
        transform.position = new Vector3(ClampX, ClampY, -10);
    }
}
