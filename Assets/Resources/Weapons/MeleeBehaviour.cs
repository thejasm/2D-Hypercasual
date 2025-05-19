using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBehaviour : MonoBehaviour {

    public float destroyAfter = 5f;
    public WeaponScriptableObject stats;
    protected virtual void Start() {
        Destroy(gameObject, destroyAfter);

    }

    protected virtual void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Enemy")) {
            other.gameObject.GetComponent<EnemyController>().TakeDamage(stats.Damage);
        }
    }

}
