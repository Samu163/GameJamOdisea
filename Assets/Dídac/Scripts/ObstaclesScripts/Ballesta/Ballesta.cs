using System.Collections;
using UnityEngine;

public class Ballesta : MonoBehaviour
{
    [SerializeField] private float delayBeforeShoot = 0.5f;
    [SerializeField] private float shootInterval = 2.5f;
    private bool canShoot = true;
    public BallestaProjectile projectile;
    [SerializeField] private Transform shootingPosition;
    public LayerMask playerLayer;

    // Update is called once per frame
    void Update()
    {
        if (canShoot)
        {
            Physics.Raycast(shootingPosition.position, transform.forward, out RaycastHit hitInfo, Mathf.Infinity, playerLayer);
            Debug.DrawRay(shootingPosition.position, transform.forward * 20f, Color.red);
            if (hitInfo.collider != null && (hitInfo.collider.CompareTag("Player")))
            {
                StartCoroutine(Shoot(delayBeforeShoot));
                canShoot = false;
            }
        }
        
    }

    private IEnumerator Shoot(float delay)
    {
        yield return new WaitForSeconds(delay);
        projectile.Shoot();
        StartCoroutine(EnableShootingAfterDelay(shootInterval));
    }

    private IEnumerator EnableShootingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canShoot = true;
        projectile.ResetProjectile();
    }
}
