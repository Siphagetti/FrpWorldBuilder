using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [Header("Smoothness Parameters")]
    [SerializeField] private float _smoothSpeed = 0.125f;

    [Header("Movement Parameters")]
    [SerializeField] private float _moveSpeed = 100f;
    [SerializeField] private float _fastMoveSpeed = 150f;

    [Header("Movement To Prefab Parameters")]
    [SerializeField] private float _distanceFromPrefab = 5f;
    [SerializeField] private float _heightAbovePrefab = 2f;

    [Header("Rotation Parameters")]
    [SerializeField] private float _rotationSpeed = 100f;
    [SerializeField] private float _mouseSensitivity = 1f;
    [SerializeField] private float _smoothRotation = 1f;

    [Header("Zoom Parameters")]
    [SerializeField] private float _minZoom = 20f;
    [SerializeField] private float _maxZoom = 60f;
    [SerializeField] private float _zoomSpeed = 2000f;
    [SerializeField] private float _fastZoomSpeed = 3000f;

    private Vector3 _targetPrefabPosition;
    private Vector3 _targetPosition;
    private Camera _cameraComponent;

    private float _targetFieldOfView;
    private Vector2 _smoothMouseInput;
    private Vector2 _currentRotation;
    private Vector2 _rotationDelta;

    private bool _isNormalMoving = false;
    private bool _isMovingToPrefab = false;

    private bool _isUIClicked = false;

    private void Awake()
    {
        _cameraComponent = GetComponent<Camera>();
        _targetPosition = transform.position;
        _targetFieldOfView = _cameraComponent.fieldOfView;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) _isUIClicked = IsCursorOverUIElement();

        if (_isUIClicked) return;

        HandleMovementToPrefab();

        if (!_isMovingToPrefab)
        {
            HandleMovementInput();
            HandleVerticalMovementInput();
            HandleRotateInput();
            HandleZoomInput();
        }

        SmoothMovement();
        SmoothZoom();
    }

    private void HandleMovementInput()
    {
        // Get input from the WASD keys
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate the new position for the camera
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput);

        if (moveDirection == Vector3.zero) { _isNormalMoving = false; return; }
        
        float speed = Input.GetKey(KeyCode.LeftShift) ? _fastMoveSpeed : _moveSpeed;

        // Apply camera rotation to the movement direction
        moveDirection = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0) * moveDirection;

        // Set the target position for smooth movement
        _targetPosition = transform.position + moveDirection * speed * Time.deltaTime;

        _isNormalMoving = true;
    }

    private void HandleVerticalMovementInput()
    {
        float verticalInput;

        if (Input.GetKey(KeyCode.E)) verticalInput = 1f;
        else if (Input.GetKey(KeyCode.Q)) verticalInput = -1f;
        else return;

        float speed = Input.GetKey(KeyCode.LeftShift) ? _fastMoveSpeed : _moveSpeed;

        // Apply camera rotation to the movement direction
        var moveDirection = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0) * Vector3.up * verticalInput;

        Vector3 verticalMovement = moveDirection * speed * Time.deltaTime;

        if (_isNormalMoving) _targetPosition += verticalMovement;
        else _targetPosition = transform.position + verticalMovement;
    }

    private void HandleMovementToPrefab()
    {
        if (!Input.GetKey(KeyCode.F)) return;

        GameObject prefab = PrefabDragManager.SelectedObject;
        if (prefab == null) return;

        // Calculate the target position based on the prefab's position
        _targetPosition = prefab.transform.position - prefab.transform.forward * _distanceFromPrefab + Vector3.up * _heightAbovePrefab;

        _targetPrefabPosition = prefab.transform.position;

        _isMovingToPrefab = true;
    }

    private void SmoothMovement()
    {
        // If its close enough to target postion, stop it.
        if (Vector3.Distance(transform.position, _targetPosition) < 0.1f)
        {
            if (_isMovingToPrefab)
            {
                // Update _currentRotation to match the new look-at direction
                _currentRotation = new Vector2(transform.eulerAngles.y, -transform.eulerAngles.x);

                _isMovingToPrefab = false;
            }
            return;
        }

        // Use Lerp to smoothly move the camera towards the target position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, _targetPosition, _smoothSpeed);

        // Update the camera's position
        transform.position = smoothedPosition;

        if (_isMovingToPrefab) transform.LookAt(_targetPrefabPosition);
    }

    private void HandleRotateInput()
    {
        if (!Input.GetMouseButton(1)) { Cursor.lockState = CursorLockMode.None; return; }

        Cursor.lockState = CursorLockMode.Locked;

        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        mouseDelta *= _mouseSensitivity * _smoothRotation * _rotationSpeed;
        _smoothMouseInput.x = Mathf.Lerp(_smoothMouseInput.x, mouseDelta.x, 1f / _smoothRotation);
        _smoothMouseInput.y = Mathf.Lerp(_smoothMouseInput.y, mouseDelta.y, 1f / _smoothRotation);

        _rotationDelta = _smoothMouseInput * Time.deltaTime;

        _currentRotation += _rotationDelta;

        _currentRotation.y = Mathf.Clamp(_currentRotation.y, -90, 90);

        transform.localRotation = Quaternion.Euler(-_currentRotation.y, _currentRotation.x, 0);
    }

    private void HandleZoomInput()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Approximately(scrollInput, 0.0f)) return;

        float speed = Input.GetKey(KeyCode.LeftShift) ? _fastZoomSpeed : _zoomSpeed;
        _targetFieldOfView = _cameraComponent.fieldOfView - scrollInput * speed * Time.deltaTime;
        _targetFieldOfView = Mathf.Clamp(_targetFieldOfView, _minZoom, _maxZoom);
    }

    private void SmoothZoom()
    {
        if (Mathf.Approximately(Mathf.Abs(_cameraComponent.fieldOfView - _targetFieldOfView), 0.0f))
            return;

        _cameraComponent.fieldOfView = Mathf.Lerp(_cameraComponent.fieldOfView, _targetFieldOfView, _smoothSpeed * 2);
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
