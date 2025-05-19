using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : WeaponContorller
{

    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedMissile = Instantiate(stats.Prefab, transform.position, Quaternion.identity);
        spawnedMissile.GetComponent<ProjectileBehaviour>().SetVector(GetEnemyDirection(), stats.Speed);
    }
}
