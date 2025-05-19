using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    Transform target;
    public EnemyScriptableObject stats;
    private Animator animator;
    public float currentHealth;
    public float currentSpeed;

    void Awake(){
        target = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();

        currentHealth = stats.MaxHealth;
        currentSpeed = stats.Speed;
    }

    void Update(){
        Walk();
    }

    void Walk() {
        transform.position = Vector2.MoveTowards(transform.position, target.position, stats.Speed * Time.deltaTime);
        if (target.position.x < transform.position.x) this.gameObject.GetComponent<SpriteRenderer>().flipX = true;
        else this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
    }

    public void TakeDamage(float dmg) {
        currentHealth -= dmg;
        animator.SetTrigger("Hurt");
        if(currentHealth <= 0) {
            animator.SetTrigger("Die");
        }
    }

    public void Destroy() {
            Destroy(gameObject);
    }
}
