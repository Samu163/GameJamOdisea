using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class RudderInteractable : InputInteractable
{
    #region Variables

    public UnityEvent<float> onRudderTurned;


    [Header("Setup")]
    [SerializeField] private Transform rudderMesh;

    [Header("Settings")]
    [SerializeField] private float maxTurnAngle = 180f;
    [SerializeField] private float linearTurnSpeed = 70f; // Speed for Keyboard
    [SerializeField] private float radialSensitivity = -1.5f; // Sensitivity for Joystick spin

    private PlayerInput pInput;
    private Interactor _interactor;

    // State
    private Vector2 _currentInputVector;
    private float _currentRudderAngle = 0f;

    // Radial State
    private float _lastStickAngle = 0f;
    private bool _isTrackingRotation = false;

    // Mode Switching
    private bool _isRadialMode = false; // Default to radial

    #endregion

    protected override void Awake()
    {
        base.Awake();
        pInput = GetComponent<PlayerInput>();
        if (rudderMesh == null) rudderMesh = transform;
    }

    public override void Interact(Interactor interactor)
    {
        base.Interact(interactor);
        _interactor = interactor;
        Deactivate();

        pInput.enabled = true;
        interactor.onInteractionLockMovement?.Invoke();

        _isTrackingRotation = false;
    }


    #region Interacting with Rudder

    private void Update()
    {
        if (pInput.enabled)
        {
            if (_isRadialMode)
            {
                HandleRadialRotation();
            }
            else
            {
                HandleLinearRotation();
            }
        }
    }

    // ----------------- JOYSTICK SPIN -----------------
    private void HandleRadialRotation()
    {
        if (_currentInputVector.magnitude < 0.2f)
        {
            _isTrackingRotation = false;
            return;
        }

        float currentStickAngle = Mathf.Atan2(_currentInputVector.y, _currentInputVector.x) * Mathf.Rad2Deg;

        if (!_isTrackingRotation)
        {
            _lastStickAngle = currentStickAngle;
            _isTrackingRotation = true;
            return;
        }

        float deltaAngle = currentStickAngle - _lastStickAngle;

        // Wrap-around fixes
        if (deltaAngle > 180f) deltaAngle -= 360f;
        if (deltaAngle < -180f) deltaAngle += 360f;

        ApplyRotation(deltaAngle * radialSensitivity);

        _lastStickAngle = currentStickAngle;
    }

    // ----------------- KEYBOARD HOLD -----------------
    private void HandleLinearRotation()
    {
        // If holding Right (X > 0), add angle. If Left (X < 0), subtract.
        if (Mathf.Abs(_currentInputVector.x) > 0.1f)
        {
            float delta = _currentInputVector.x * linearTurnSpeed * Time.deltaTime;
            ApplyRotation(delta);
        }
    }

    // Shared method to apply the value to the mesh
    private void ApplyRotation(float changeInAngle)
    {
        _currentRudderAngle += changeInAngle;
        _currentRudderAngle = Mathf.Clamp(_currentRudderAngle, -maxTurnAngle, maxTurnAngle);

        onRudderTurned?.Invoke(changeInAngle);

        rudderMesh.Rotate(Vector3.up, changeInAngle );    

        //rudderMesh.localRotation = Quaternion.Euler(rudderMesh.localEulerAngles.x, _currentRudderAngle, rudderMesh.localEulerAngles.z);
    }

    // ----------------- INPUT HANDLING -----------------
    public void OnRudderMovement(InputAction.CallbackContext context)
    {
        _currentInputVector = context.ReadValue<Vector2>();

        // Check which device sent this input
        if (context.control.device is Keyboard)
        {
            _isRadialMode = false;
        }
        else
        {
            _isRadialMode = true;
        }

    }



    public void OnInteractPressedWhileInteracting(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _interactor.onInteractionUnlockMovement?.Invoke();
            Activate();
        }
    }

    #endregion

    public override void Activate()
    {
        pInput.enabled = false;
        base.Activate();
    }
}