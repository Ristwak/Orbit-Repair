using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class HandGesturePlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform cameraTransform;
    public MoveDirection selectedDirection;

    private Vector3 moveDirection = Vector3.zero;
    private bool isMoving = false;
    private CharacterController controller;

    public enum MoveDirection { Forward, Backward, Left, Right }

    void Awake()
    {
        controller = GetComponent<CharacterController>();
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
            // ✅ Uses CharacterController.Move which respects collisions
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        }
    }

    public void MovePlayer()
    {
        Debug.Log("Moving: " + selectedDirection);

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        switch (selectedDirection)
        {
            case MoveDirection.Forward: moveDirection = cameraForward; break;
            case MoveDirection.Backward: moveDirection = -cameraForward; break;
            case MoveDirection.Left: moveDirection = -cameraRight; break;
            case MoveDirection.Right: moveDirection = cameraRight; break;
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
