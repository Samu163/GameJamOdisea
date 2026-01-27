using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerInputScript : MonoBehaviour
{

    public UnityEvent<InputAction.CallbackContext> onMovementInput;
    public UnityEvent<InputAction.CallbackContext> onInteractInput;

    private PlayerMovement playerMovement;
    private PlayerAlargar playerAlargar;
    private PlayerAudioSystem playerAudioSystem;
    private Interactor playerInteractor;

    private Camera mainCamera;

    [SerializeField] private float keyboardExtraRotationDegrees = 45f;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerAlargar = GetComponent<PlayerAlargar>();
        playerAudioSystem = GetComponent<PlayerAudioSystem>();
        playerInteractor = GetComponentInChildren<Interactor>();

        mainCamera = Camera.main;

        // Enable or disable this script if a interaction locks movement
        playerInteractor.onInteractionLockMovement.AddListener(LockMovement);
        playerInteractor.onInteractionUnlockMovement.AddListener(UnlockMovement);
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        onMovementInput?.Invoke(context);
        if (!enabled) return; // If movement is not enable, we read and emit the input but we dont move


        if (context.performed)
        {
           if (playerAlargar.isAlargarHeld || !GameManager.instance.hasGameStarted)
           {
               playerMovement.inputDir = Vector3.zero;
               return;
           }

            Vector2 movement = context.ReadValue<Vector2>();

            if (GameManager.instance.playerSelector.IsDeviceGamepad(context.control.device))
            {
                playerMovement.inputDir = GetCameraRelativeDirection(movement);
            }
            else
            {
                playerMovement.inputDir = GetKeyboardRotatedDirection(movement);

            }


        }
        else if (context.canceled)
        {
            playerMovement.inputDir = Vector3.zero;
        }
    }
    public void OnAlargar(InputAction.CallbackContext context)
    {
        if (!enabled) return;

        if (GameManager.instance.hasGameStarted == false)
            return;

        if (context.started)
        {
            playerAlargar.isAlargarHeld = true;
            playerMovement.inputDir = Vector3.zero;
        }
        else if (context.canceled)
        {
            playerAlargar.CancelAlargar();
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        onInteractInput?.Invoke(context);
        if (!enabled) return; // If movement is not enable, we read and emit the input but we dont interact

        if (!context.performed) return;
        
        playerInteractor.TryToInteract();
        
    }

    public void OnNextLine(InputAction.CallbackContext context)
    {
        if (context.started && UIController.instance.isDialogueActive)
        {
            UIController.instance.NextLineDialogue();
        }
        
    }

    public void OnSkipDialogue(InputAction.CallbackContext context)
    {
        if (context.started && UIController.instance.isDialogueActive)
        {
            UIController.instance.SkipDialogue();
        }
    }

    public void OnLadrar(InputAction.CallbackContext context)
    {
        if (!enabled) return;

        if (GameManager.instance.hasGameStarted == false)
            return;

        if (context.started)
        {
            playerAudioSystem.PlayLadrido();
        }
    }

    public void OnSkipLevel(InputAction.CallbackContext context)
    {

        if (!enabled) return;

        if (GameManager.instance.hasGameStarted == false)
            return;

        if (context.started)
        {
            LevelManager.instance.SkipLevel();
        }
    }

    private void LockMovement()
    {
        enabled = false;
    }

    private void UnlockMovement()
    {
        enabled = true;
    }

    private Vector3 GetCameraRelativeDirection(Vector2 input)
    {
        if (mainCamera == null)
        {
            // Fallback al comportamiento anterior si no hay cámara
            return new Vector3(input.x, 0f, input.y).normalized;
        }

        Vector3 camForward = mainCamera.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = mainCamera.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 worldDir = camRight * input.x + camForward * input.y;
        if (worldDir.sqrMagnitude > 1f) worldDir.Normalize();
        return worldDir;
    }

    private Vector3 GetKeyboardRotatedDirection(Vector2 input)
    {
        Vector3 input3 = new Vector3(input.x, 0f, input.y);

        float yaw = keyboardExtraRotationDegrees;
        if (mainCamera != null)
        {
            yaw += mainCamera.transform.eulerAngles.y;
        }

        Quaternion rot = Quaternion.Euler(0f, yaw, 0f);
        Vector3 rotated = rot * input3;

        if (rotated.sqrMagnitude > 1f) rotated.Normalize();
        return rotated;
    }
}