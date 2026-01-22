using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputScript : MonoBehaviour
{
    
    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
       if (context.performed)
       {
           Vector2 movement = context.ReadValue<Vector2>();
           playerMovement.inputDir = new Vector3(movement.x, 0, movement.y).normalized;
       }
       else if (context.canceled)
       {
           playerMovement.inputDir = Vector3.zero;
        }
    }
}
