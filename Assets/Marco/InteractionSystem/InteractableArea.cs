using System.ComponentModel;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

[ExecuteAlways]
public class InteractableArea : MonoBehaviour
{
    private IInteractableObject interactableObject;
    private BoxCollider zoneCollider;

    // Default to "Everything"
    public LayerMask detectionLayer = ~0;

    [Description("Interactor that should be ignored when detecting presence in the area, normally this interactable's own interactors")]
    public List<UnityEngine.Collider> InteractorCollidersToExclude = new List<UnityEngine.Collider>();

    // List with every interactor that can currently interact with the interactable (inside interaction area)
    public Collider[] hits;

    private void Awake()
    {
        interactableObject = GetComponentInParent<IInteractableObject>();
        zoneCollider = GetComponent<BoxCollider>();
    }

    private void FixedUpdate()
    {
        if (zoneCollider == null) return;

        Vector3 worldCenter = transform.TransformPoint(zoneCollider.center);
        Vector3 worldHalfExtents = Vector3.Scale(zoneCollider.size, transform.lossyScale) * 0.5f;
        hits = Physics.OverlapBox(
            worldCenter,
            worldHalfExtents,
            transform.rotation,
            detectionLayer,
            QueryTriggerInteraction.Collide
        );
        Collider last_hit = null;

        bool isCurrentlyActive = false;
        foreach (Collider hit in hits)
        {
            last_hit = hit;
            if (hit.gameObject == this.gameObject) continue; // Ignore self

            if (hit.CompareTag("Interactor") && InteractorCollidersToExclude.Contains(hit) == false)
            {
                isCurrentlyActive = true;
                break;
            }
        }

        // If we detect the interactable is active/inactive when it shouldnt be AND we want the area to autmatically activate interaction when player within range

        if (isCurrentlyActive != interactableObject.IsActive() && interactableObject.AreaAutomaticActivation())
        {
            if (isCurrentlyActive)
            {

                interactableObject.Activate();
                if (interactableObject is InputInteractable)
                {
                    InputInteractable inputInteractable = (InputInteractable)interactableObject;
                    if (last_hit != null)
                    {
                        inputInteractable.OnPlayerEnterRange(last_hit.gameObject.GetComponentInParent<PlayerInput>());
                    }
                }
            }
            else
            {
                interactableObject.Deactivate();
            }
        }
    }

    // --- VISUAL DEBUGGER ---
    // This draws the box that Physics is checking.
    // If the Red/Green box doesn't match your object, adjust the Collider size.
    private void OnDrawGizmos()
    {

        if (interactableObject == null) return;

        if (zoneCollider == null) zoneCollider = GetComponent<BoxCollider>();
        if (zoneCollider == null) return;

        Gizmos.color = interactableObject.IsActive() ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);
        // Use the same matrix the physics engine uses
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(
            transform.TransformPoint(zoneCollider.center),
            transform.rotation,
            transform.lossyScale
        );
        Gizmos.matrix = rotationMatrix;
        // Draw the cube using Local Size (Gizmos matrix handles the scale/pos)
        Gizmos.DrawCube(Vector3.zero, zoneCollider.size);
        Gizmos.DrawWireCube(Vector3.zero, zoneCollider.size);
    }
}