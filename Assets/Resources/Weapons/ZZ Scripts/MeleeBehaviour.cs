using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBehaviour : MonoBehaviour {
    public WeaponController weaponController;

    [HideInInspector]
    public Transform player;

    protected virtual void Start() {
        weaponController = GetComponentInParent<WeaponController>();
        if (weaponController == null) {
            Debug.LogError("ProjectileBehaviour must be attached to a GameObject with a WeaponController component.");
        }

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Enemy")) {
            other.gameObject.GetComponent<EnemyController>().TakeDamage(weaponController.currentDamage);
        }
    }

}
