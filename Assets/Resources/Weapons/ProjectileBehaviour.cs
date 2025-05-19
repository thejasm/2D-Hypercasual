using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProjectileBehaviour : MonoBehaviour
{
    public WeaponScriptableObject stats;

    public Vector3 direction;
    public float destroyAfter = 5f;
    public float speed = 1f;

    protected virtual void Start(){
        Destroy(gameObject, destroyAfter);
        transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);

    }

    protected virtual void Update() {
        transform.position += direction.normalized * speed * Time.deltaTime;
    }

    public void SetVector(Vector3 direction, float speed){
        this.direction = direction;
        this.speed = speed;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player")) {
            if (stats.Pierce > 0) stats.Pierce--;
            else Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Enemy")) {
            other.gameObject.GetComponent<EnemyController>().TakeDamage(stats.Damage);
        }
    }
}
