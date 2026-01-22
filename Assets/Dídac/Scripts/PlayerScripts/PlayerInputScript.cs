using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputScript : MonoBehaviour
{
    
    private PlayerMovement playerMovement;
    private PlayerAlargar playerAlargar;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerAlargar = GetComponent<PlayerAlargar>();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {   

        if (context.performed)
        {
           if (playerAlargar.isAlargarHeld)
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
        if (context.started)
        {
            playerAlargar.isAlargarHeld = true;
        }
        else if (context.canceled)
        {
            playerAlargar.CancelAlargar();
        }
    }
}
