using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponContorller : MonoBehaviour
{
    public WeaponScriptableObject stats;
    private float cooldownTimer;

    public GameObject nearestEnemy;

    protected virtual void Start(){
        cooldownTimer = stats.CooldownDuration;
    }

    // Update is called once per frame
    protected virtual void Update(){
        cooldownTimer -= Time.deltaTime;
        if(cooldownTimer <= 0)
        {
            Attack();
        }
    }

    protected virtual void Attack() {
        cooldownTimer = stats.CooldownDuration;

        nearestEnemy = FindNearestEnemy();
    }

    protected GameObject FindNearestEnemy() {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        GameObject closest = null;
        float minDist = Mathf.Infinity; 
        Vector3 currentPos = transform.position;

        foreach (GameObject e in enemies) {
            float dist = Vector3.Distance(currentPos, e.transform.position);

            if (dist < minDist) {
                closest = e;
                minDist = dist;
            }
        }

        return closest;
    }

    protected Vector3 GetEnemyDirection() {
        if (nearestEnemy == null) {
            Vector2 randomPoint = Random.insideUnitCircle;
            return new Vector3(randomPoint.x * 10, randomPoint.y * 10, 0f);
        }
        return nearestEnemy.transform.position - this.transform.position;
    }
}
