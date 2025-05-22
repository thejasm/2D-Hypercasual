using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : WeaponController
{

    protected override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedMissile = Instantiate(stats.Prefab, transform.position, Quaternion.identity);
        spawnedMissile.GetComponent<ProjectileBehaviour>().SetDirection(GetEnemyDirection());
    }
}
