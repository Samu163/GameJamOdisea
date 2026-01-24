using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputScript : MonoBehaviour
{

    private PlayerMovement playerMovement;
    private PlayerAlargar playerAlargar;
    private Interactor playerInteractor;
    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerAlargar = GetComponent<PlayerAlargar>();
        playerInteractor = GetComponentInChildren<Interactor>();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
       
        if (context.performed)
        {
           if (playerAlargar.isAlargarHeld || !GameManager.instance.hasGameStarted)
           {
               playerMovement.inputDir = Vector3.zero;
               return;
           }

           Vector2 movement = context.ReadValue<Vector2>();
           playerMovement.inputDir = new Vector3(movement.x, 0, movement.y).normalized;
        }
        else if (context.canceled)
        {
            playerMovement.inputDir = Vector3.zero;
        }
    }
    public void OnAlargar(InputAction.CallbackContext context)
    {
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
        if (!context.performed) return;
        
        playerInteractor.TryToInteract();
        
    }

    public void OnNextLine(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            UIController.instance.NextLineDialogue();
        }
        
    }
}