using UnityEngine;

public class LaserEmitter : MonoBehaviour
{

    public LayerMask mirrorMask;
    private MirrorLaser currentHittingMirror;
    private Vector3 reflectionDir;
    private Vector3 hitPos;

    // Update is called once per frame
    void Update()
    {
        Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, Mathf.Infinity, mirrorMask);
        if (hitInfo.collider != null)
        {
            Vector3 distanceToHit = hitInfo.point - transform.position;
            Debug.DrawRay(transform.position, distanceToHit, Color.red);

            hitPos = hitInfo.point;

            Vector3 incomingDir = distanceToHit.normalized;
            Vector3 surfaceNormal = hitInfo.normal;
            reflectionDir = Vector3.Reflect(incomingDir, surfaceNormal);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * 20f, Color.red);
        }


        if (hitInfo.collider != null && (hitInfo.collider.CompareTag("Mirror")))
        {
            currentHittingMirror = hitInfo.collider.GetComponent<MirrorLaser>();
            if (currentHittingMirror != null)
            {
                currentHittingMirror.ActivateMirror(reflectionDir, hitPos);
            }
        }
        else
        {
            if (currentHittingMirror != null)
            {
                currentHittingMirror.DeactivateMirror();
                currentHittingMirror = null;
            }
        }
    }
}
