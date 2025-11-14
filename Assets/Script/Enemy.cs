using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 2f;
    public int damage = 10;
    public int health = 50;
    public float attackRate = 1.5f;
    public Animator animator;

    private Transform targetGate;
    private Gate gate;
    private bool isAtGate = false;
    private float nextAttackTime = 0f;

    // 🧩 Event for when this enemy dies
    public event Action<Enemy> OnEnemyDeath;

    void Start()
    {
        GameObject gateObj = GameObject.FindGameObjectWithTag("Gate");
        if (gateObj != null)
        {
            targetGate = gateObj.transform;
            gate = gateObj.GetComponent<Gate>();
        }
        else
        {
            Debug.LogError("Gate not found! Make sure the Gate is tagged as 'Gate'.");
        }
    }

    void Update()
    {
        if (gate == null) return;

        if (!isAtGate)
        {
            MoveTowardsGate();
            animator.SetBool("isRunning", true);
            animator.SetBool("isAttacking", false);
            gate.isUnderAttack = false;
        }
        else
        {
            AttackGate();
            animator.SetBool("isAttacking", true);
            animator.SetBool("isRunning", false);
            gate.isUnderAttack = true;
        }
    }

    void MoveTowardsGate()
    {
        if (targetGate == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetGate.position,
            speed * Time.deltaTime
        );
        
        Vector3 direction = (targetGate.position - transform.position).normalized;
        direction.y = 0f; // keep upright

        if (Vector3.Distance(transform.position, targetGate.position) < 0.5f)
        {
            isAtGate = true;
        }
        else
        {
            isAtGate = false;
        }
    }

    void AttackGate()
    {
        if (Time.time >= nextAttackTime)
        {
            
            nextAttackTime = Time.time + attackRate;
            if (gate != null)
            {
                gate.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        OnEnemyDeath?.Invoke(this);
        Destroy(gameObject);
    }
}
