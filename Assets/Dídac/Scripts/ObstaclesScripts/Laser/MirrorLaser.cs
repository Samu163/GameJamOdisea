using UnityEngine;

public class MirrorLaser : MonoBehaviour
{

    public bool isReceivingLaser = false;
    public LayerMask mirrorMask;
    private MirrorLaser currentHittingMirror;
    private LaserReceiver currentHittingReceiver;
    private Vector3 laserDir;
    private Vector3 originLaser;
    private Vector3 hitPos;
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
            Physics.Raycast(originLaser, laserDir, out RaycastHit hitInfo, Mathf.Infinity);
            if (hitInfo.collider != null)
            {
                Vector3 distanceToHit = hitInfo.point - originLaser;
                Debug.DrawRay(originLaser, distanceToHit, Color.red);

                hitPos = hitInfo.point;

                Vector3 incomingDir = distanceToHit.normalized;
                Vector3 surfaceNormal = hitInfo.normal;
                reflectionDir = Vector3.Reflect(incomingDir, surfaceNormal);
            }
            else
            {
                Debug.DrawRay(originLaser, laserDir * 20f, Color.red);
            }

            if (hitInfo.collider != null && (hitInfo.collider.CompareTag("Mirror")))
            {
                currentHittingMirror = hitInfo.collider.GetComponent<MirrorLaser>();
                if (currentHittingMirror != null)
                {
                    currentHittingMirror.ActivateMirror(reflectionDir, hitPos);
                }
            }
            else if (hitInfo.collider != null && (hitInfo.collider.CompareTag("LaserReceiver")))
            {
                currentHittingReceiver = hitInfo.collider.GetComponent<LaserReceiver>();
                if (currentHittingReceiver != null)
                {
                    currentHittingReceiver.ActivateReceiver();
                }
            }
            else
            {
                currentHittingMirror?.DeactivateMirror();
                currentHittingReceiver?.DeactivateReceiver();
                currentHittingMirror = null;
                currentHittingReceiver = null;
            }
        }
    }

    public void ActivateMirror(Vector3 receivedDir, Vector3 hitPosReceived)
    {
        isReceivingLaser = true;
        laserDir = receivedDir;
        originLaser = hitPosReceived;
    }

    public void DeactivateMirror()
    {
        isReceivingLaser = false;
    }
}
