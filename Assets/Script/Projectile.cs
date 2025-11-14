using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    private Enemy target;
    public BuildableObject turrets;

    public void SetTarget(Enemy enemy)
    {
        target = enemy;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Move toward target
        Vector3 dir = target.transform.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        transform.LookAt(target.transform);
    }

    void HitTarget()
    {
        target.TakeDamage(turrets.damage);
        Destroy(gameObject);
    }
}
