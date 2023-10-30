using Prefab;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [Header("Smoothness Parameters")]
    [SerializeField] private float smoothSpeed = 0.125f;

    [Header("Movement Parameters")]
    [SerializeField] private float moveSpeed = 100f;
    [SerializeField] private float fastMoveSpeed = 150f;

    [Header("Movement To Prefab Parameters")]
    [SerializeField] private float distanceFromPrefab = 5f;
    [SerializeField] private float heightAbovePrefab = 2f;

    [Header("Rotation Parameters")]
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float smoothRotation = 1f;

    [Header("Zoom Parameters")]
    [SerializeField] private float minZoom = 20f;
    [SerializeField] private float maxZoom = 60f;
    [SerializeField] private float zoomSpeed = 2000f;
    [SerializeField] private float fastZoomSpeed = 3000f;

    private Vector3 targetPrefabPosition;
    private Vector3 targetPosition;
    private Camera cameraComponent;

    private float targetFieldOfView;
    private Vector2 smoothMouseInput;
    private Vector2 currentRotation;
    private Vector2 rotationDelta;

    private bool isNormalMoving = false;
    private bool isMovingToPrefab = false;

    public bool IsUIClicked { get; set; } = false;

    private void Awake()
    {
        cameraComponent = GetComponent<Camera>();
        targetPosition = transform.position;
        targetFieldOfView = cameraComponent.fieldOfView;
    }

    private void Update()
    {
        // Check if the mouse is over a UI element
        bool isCursorOverUI = IsCursorOverUIElement();

        // Detect mouse click on UI
        if (Input.GetMouseButtonDown(0)) IsUIClicked = isCursorOverUI;

        // Handle moving the camera towards a prefab
        HandleMovementToPrefab();

        // If not interacting with UI, handle camera movement and input
        if (!IsUIClicked)
        {
            if (!isMovingToPrefab)
            {
                HandleMovementInput();
                HandleVerticalMovementInput();

                // Check if the mouse is not over UI for rotation and zoom
                if (!isCursorOverUI)
                {
                    HandleRotateInput();
                    HandleZoomInput();
                }
            }
            SmoothZoom(); // Smoothly adjust the camera's field of view
        }

        SmoothMovement(); // Smoothly adjust the camera's position
    }

    private void HandleMovementInput()
    {
        // Get input from the WASD keys
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate the new position for the camera
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput);

        if (moveDirection == Vector3.zero)
        {
            isNormalMoving = false;
            return;
        }

        float speed = Input.GetKey(KeyCode.LeftShift) ? fastMoveSpeed : moveSpeed;

        // Apply camera rotation to the movement direction
        moveDirection = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0) * moveDirection;

        // Set the target position for smooth movement
        targetPosition = transform.position + moveDirection * speed * Time.deltaTime;

        isNormalMoving = true;
    }

    private void HandleVerticalMovementInput()
    {
        float verticalInput;

        if (Input.GetKey(KeyCode.E)) verticalInput = 1f;
        else if (Input.GetKey(KeyCode.Q)) verticalInput = -1f;
        else return;

        float speed = Input.GetKey(KeyCode.LeftShift) ? fastMoveSpeed : moveSpeed;

        // Apply camera rotation to the movement direction
        var moveDirection = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0) * Vector3.up * verticalInput;

        Vector3 verticalMovement = moveDirection * speed * Time.deltaTime;

        if (isNormalMoving)
        {
            targetPosition += verticalMovement;
        }
        else
        {
            targetPosition = transform.position + verticalMovement;
        }
    }

    private void HandleMovementToPrefab()
    {
        if (Input.GetKey(KeyCode.F))
        {
            GameObject prefab = PrefabDragManager.SelectedObject;
            if (prefab != null)
            {
                // Calculate the target position based on the prefab's position
                targetPosition = prefab.transform.position - prefab.transform.forward * distanceFromPrefab + Vector3.up * heightAbovePrefab;

                targetPrefabPosition = prefab.transform.position;

                isMovingToPrefab = true;
            }
        }
    }

    private void SmoothMovement()
    {
        // If it's close enough to the target position, stop moving
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            if (isMovingToPrefab)
            {
                // Update currentRotation to match the new look-at direction
                currentRotation = new Vector2(transform.eulerAngles.y, -transform.eulerAngles.x);

                isMovingToPrefab = false;
            }
            return;
        }

        // Use Lerp to smoothly move the camera towards the target position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

        // Update the camera's position
        transform.position = smoothedPosition;

        if (isMovingToPrefab)
        {
            transform.LookAt(targetPrefabPosition);
        }
    }

    private void HandleRotateInput()
    {
        if (Input.GetMouseButton(1))
        {
            Cursor.lockState = CursorLockMode.Locked;

            Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            mouseDelta *= mouseSensitivity * smoothRotation * rotationSpeed;
            smoothMouseInput.x = Mathf.Lerp(smoothMouseInput.x, mouseDelta.x, 1f / smoothRotation);
            smoothMouseInput.y = Mathf.Lerp(smoothMouseInput.y, mouseDelta.y, 1f / smoothRotation);

            rotationDelta = smoothMouseInput * Time.deltaTime;

            currentRotation += rotationDelta;

            currentRotation.y = Mathf.Clamp(currentRotation.y, -90, 90);

            transform.localRotation = Quaternion.Euler(-currentRotation.y, currentRotation.x, 0);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void HandleZoomInput()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (!Mathf.Approximately(scrollInput, 0.0f))
        {
            float speed = Input.GetKey(KeyCode.LeftShift) ? fastZoomSpeed : zoomSpeed;
            targetFieldOfView = cameraComponent.fieldOfView - scrollInput * speed * Time.deltaTime;
            targetFieldOfView = Mathf.Clamp(targetFieldOfView, minZoom, maxZoom);
        }
    }

    private void SmoothZoom()
    {
        if (!Mathf.Approximately(Mathf.Abs(cameraComponent.fieldOfView - targetFieldOfView), 0.0f))
        {
            cameraComponent.fieldOfView = Mathf.Lerp(cameraComponent.fieldOfView, targetFieldOfView, smoothSpeed * 2);
        }
    }

    private bool IsCursorOverUIElement()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}
