
using UnityEngine;

public class HandGesturePlayerMove: MonoBehaviour
{
    public float moveSpeed = 5f;  // Speed of the player
    public Transform cameraTransform;  // Reference to the camera's transform

    public MoveDirection selectedDirection;  // Select direction in Inspector

    private Vector3 moveDirection = Vector3.zero;
    private bool isMoving = false;

    public enum MoveDirection
    {
        Forward,
        Backward,
        Left,
        Right
    }

    void Update()
    {
        if (isMoving)
        {
            MoveInCameraDirection();
        }
    }

    private void MoveInCameraDirection()
    {
        if (moveDirection != Vector3.zero)
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    // This method will be called from the Inspector to move the player
    public void MovePlayer()
    {
        Debug.Log("Moving: " + selectedDirection.ToString());

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Ensure movement is on the horizontal plane
        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        switch (selectedDirection)
        {
            case MoveDirection.Forward:
                moveDirection = cameraForward;
                break;
            case MoveDirection.Backward:
                moveDirection = -cameraForward;
                break;
            case MoveDirection.Left:
                moveDirection = -cameraRight;
                break;
            case MoveDirection.Right:
                moveDirection = cameraRight;
                break;
        }

        isMoving = true;
    }

    public void StopPlayer()
    {
        Debug.Log("Stopping Player");
        isMoving = false;
        moveDirection = Vector3.zero;
    }
}
