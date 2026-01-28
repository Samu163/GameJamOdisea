using System.Collections;
using UnityEngine;

public class Ballesta : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float delayBeforeShoot = 0.5f;
    [SerializeField] private float shootInterval = 2.5f;

    [Header("Referencias")]
    public BallestaProjectile projectile;
    [SerializeField] private Transform shootingPosition;
    public LayerMask playerLayer;

    // 1. Añadimos la referencia al Animator
    [SerializeField] private Animator animator;

    private bool canShoot = true;

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

        // 2. Activamos la animación de disparo
        projectile.Shoot();
        if (animator != null) animator.SetTrigger("Shoot");
        AudioManager.instance.PlayBallesta();

        StartCoroutine(EnableShootingAfterDelay(shootInterval));
    }

    private IEnumerator EnableShootingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        canShoot = true;
        projectile.ResetProjectile();
    }
}