using UnityEngine;

public class MirrorLaser : MonoBehaviour
{

    public bool isReceivingLaser = false;
    public LayerMask mirrorMask;
    private MirrorLaser currentHittingMirror;
    private Vector3 laserDir;
    private Vector3 reflectionDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isReceivingLaser)
        {
            Physics.Raycast(transform.position, laserDir, out RaycastHit hitInfo, Mathf.Infinity, mirrorMask);
            if (hitInfo.collider != null)
            {
                Vector3 distanceToHit = hitInfo.point - transform.position;
                Debug.DrawRay(transform.position, distanceToHit, Color.red);
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
                    currentHittingMirror.ActivateMirror(reflectionDir);
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

    public void ActivateMirror(Vector3 receivedDir)
    {
        isReceivingLaser = true;
        laserDir = receivedDir;
    }

    public void DeactivateMirror()
    {
        isReceivingLaser = false;
    }
}
