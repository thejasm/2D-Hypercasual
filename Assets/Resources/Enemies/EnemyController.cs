using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
    Transform target;
    public EnemyScriptableObject stats;
    private Animator animator;
    private DropRateManager dropRateManager;
    public float currentHealth;
    public float currentSpeed;
    public float currentDamage;


    void Awake(){
        target = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        dropRateManager = GetComponent<DropRateManager>();

        currentHealth = stats.MaxHealth;
        currentSpeed = stats.Speed;
        currentDamage = stats.Damage;

    }

    private void Start() {
        StartCoroutine(DespawnDistanceDetector());
    }

    void Update(){
        Walk();
    }

    void Walk() {
        transform.position = Vector2.MoveTowards(transform.position, target.position, currentSpeed * Time.deltaTime);
        if (target.position.x < transform.position.x) this.gameObject.GetComponent<SpriteRenderer>().flipX = true;
        else this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
    }

    public void TakeDamage(float dmg) {
        currentHealth -= dmg;
        animator.SetTrigger("Hurt");
        if (currentHealth <= 0) Die();
    }

    public void Die() {
        animator.SetTrigger("Die");
        currentSpeed = 0;
        dropRateManager.DropLoot();
        Destroy(gameObject, 0.5f);
    }

    public void Destroy() {
            Destroy(gameObject);
    }

    IEnumerator DespawnDistanceDetector() {
        while (true) {
            if ((transform.position - target.position).sqrMagnitude > 500f) {
                if(stats.Important) {
                    Vector2 randomPoint = Random.insideUnitCircle;
                    Vector3 spawnOffset = (Vector3)randomPoint.normalized * FindObjectOfType<EnemySpawner>().spawnRadius;
                    Vector3 spawnPoint = target.transform.position + spawnOffset;
                    transform.position = spawnPoint;

                } else Destroy(gameObject);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void OnCollisionStay2D(Collision2D col) {
        if(col.gameObject.tag == "Player") {
            col.gameObject.GetComponent<PlayerCore>().TakeDamage(currentDamage);
        }
    }

    private void OnDestroy() {
        if(!gameObject.scene.isLoaded) return;

        EnemySpawner es = FindObjectOfType<EnemySpawner>();
        es.enemiesAlive--;
    }
}
