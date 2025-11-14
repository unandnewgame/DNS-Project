using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Tower Settings")]
    public float range = 5f;
    public float fireRate = 1f;
    public GameObject projectilePrefab;
    public Transform firePoint; // where bullets are spawned
    public Transform head;

    private float nextFireTime = 0f;
    private Enemy currentTarget;

    void Update()
    {
        FindTarget();

        if (currentTarget != null)
        {
            AimAtTarget();
            Shoot();
        }
    }

    void FindTarget()
    {
        // If current target is dead or out of range, clear it
        if (currentTarget != null)
        {
            float dist = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (dist > range || currentTarget.health <= 0)
            {
                currentTarget = null;
            }
        }

        // If no current target, search for one
        if (currentTarget == null)
        {
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            float shortestDistance = Mathf.Infinity;
            Enemy nearestEnemy = null;

            foreach (Enemy enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < shortestDistance && distance <= range)
                {
                    shortestDistance = distance;
                    nearestEnemy = enemy;
                }
            }

            if (nearestEnemy != null)
            {
                currentTarget = nearestEnemy;
            }
        }
    }

    void AimAtTarget()
    {
        if (head == null) return;

        Vector3 direction = currentTarget.transform.position - head.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = lookRotation.eulerAngles;

        // Adjust rotation depending on your model’s facing direction
        head.rotation = Quaternion.Euler(0f, rotation.y + 180f, 0f);
    }

    void Shoot()
    {
        if (Time.time >= nextFireTime && projectilePrefab != null && firePoint != null)
        {
            nextFireTime = Time.time + 1f / fireRate;
            GameObject projGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Projectile projectile = projGO.GetComponent<Projectile>();

            if (projectile != null)
            {
                projectile.SetTarget(currentTarget);
            }
        }
    }

    // Draw range in scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
