using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProjectileBehaviour : MonoBehaviour
{
    protected WeaponController weaponController;

    public Vector3 direction;
    public float projectilePierce = 0;
    public float destroyAfter = 5f;
    public float knockbackForce = 2f;

    protected virtual void Start() {
        weaponController = GetComponentInParent<WeaponController>();
        if (weaponController == null) {
            weaponController = FindAnyObjectByType<WeaponController>();
        }

        projectilePierce = weaponController.currentPierce;

        Destroy(gameObject, destroyAfter);
        transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);

    }

    protected virtual void Update() {
        transform.position += direction.normalized * weaponController.currentSpeed * Time.deltaTime;
    }

    public void SetDirection(Vector3 direction){
        this.direction = direction;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player")) {
            if (projectilePierce > 0) projectilePierce--;
            else Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Enemy")) {
            other.gameObject.GetComponent<EnemyController>().TakeDamage(weaponController.currentDamage);
            other.gameObject.GetComponent<EnemyController>().Knockback(knockbackForce);
        }
    }

}
