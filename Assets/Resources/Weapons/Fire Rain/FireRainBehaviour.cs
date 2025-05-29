using UnityEngine;

/// <summary>
/// Controls the behavior of a "Fire Rain" projectile.
/// The projectile selects a target area near the player, spawns at an offset from that target,
/// moves towards the target, and triggers a landing animation after a fixed duration.
/// </summary>
public class FireRainProjectile: MonoBehaviour {
    public GameObject damageArea;
    public float damage;

    public Transform playerTransform;
    public float minRadiusFromPlayer = 0f;
    public float maxRadiusFromPlayer = 5f;
    public float fallSpeed = 10f;
    public Vector2 spawnOffsetFromTarget = new Vector2(10f, 10f);
    public float timeToLand = 1f;
    public float SpreadTime = 2f;

    private Animator animator;
    private Vector2 targetWorldPosition;
    private bool hasLanded = false;
    private Vector3 initialSpawnPosition;

    void Start() {
        animator = GetComponent<Animator>();
        if (playerTransform == null) {
            playerTransform = FindObjectOfType<PlayerCore>().transform;
        }
        InitializeProjectileState();
    }

    void InitializeProjectileState() {
        ChooseTargetWorldPosition();
        SetInitialSpawnPositionFromTarget();
    }

    void ChooseTargetWorldPosition() {
        if (playerTransform == null) return;

        Vector2 playerPos = playerTransform.position;

        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float randomDistance = Random.Range(0, maxRadiusFromPlayer);

        Vector2 offset = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * randomDistance;
        targetWorldPosition = playerPos + offset;
    }

    void SetInitialSpawnPositionFromTarget() {
        initialSpawnPosition = new Vector3(
            targetWorldPosition.x + spawnOffsetFromTarget.x,
            targetWorldPosition.y + spawnOffsetFromTarget.y,
            0f
        );
        transform.position = initialSpawnPosition;
    }

    void Update() {
        if (hasLanded) return;

        Vector2 currentPosition2D = transform.position;
        Vector2 directionToTarget = (targetWorldPosition - currentPosition2D).normalized;

        if (Vector2.Distance(currentPosition2D, targetWorldPosition) > 0.01f) {
            currentPosition2D += directionToTarget * fallSpeed * Time.deltaTime;
            transform.position = new Vector3(currentPosition2D.x, currentPosition2D.y, 0f);
        }
        else if (!hasLanded) PerformLanding();
    }

    void PerformLanding() {
        hasLanded = true;

        if (animator != null && !string.IsNullOrEmpty("Land")) {
            animator.SetTrigger("Land");
        }

        Destroy(gameObject, SpreadTime);
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Enemy")) {
            other.gameObject.GetComponent<EnemyController>().TakeDamage(damage);
        }
    }
}
