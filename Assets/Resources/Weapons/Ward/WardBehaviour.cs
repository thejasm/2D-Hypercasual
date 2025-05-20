using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class WardBehaviour : MeleeBehaviour
{
    Animator animator;
    private float timer = 0;
    private float damageTimer = 0;
    public Transform player;

    List<GameObject> markedEnemies = new List<GameObject>();

    protected override void Start(){
        animator = GetComponent<Animator>();
        if(player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update() {
        transform.position = player.position + stats.Offset;
        transform.rotation = transform.rotation * Quaternion.Euler(0, 0, stats.Speed * 0.1f);

        TickDamage();

        timer += Time.deltaTime;
        if (timer <= destroyAfter) return;
        //else
        animator.SetTrigger("End");
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Despawn") && 
           animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) {
            Destroy(gameObject);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Enemy") && !markedEnemies.Contains(other.gameObject)) {
                markedEnemies.Add(other.gameObject);
        }
    }

    protected void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.CompareTag("Enemy") && markedEnemies.Contains(other.gameObject)) {
            markedEnemies.Remove(other.gameObject);
        }
    }

    private void TickDamage() {
        damageTimer += Time.deltaTime;
        if (damageTimer < 0.25) return;
        //else
        damageTimer = 0;
        foreach (GameObject enemy in markedEnemies) {
            if (enemy == null) continue;
            enemy.GetComponent<EnemyController>().TakeDamage(stats.Damage);
        }
    }

}
