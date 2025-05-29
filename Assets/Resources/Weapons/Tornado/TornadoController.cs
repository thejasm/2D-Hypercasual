using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoController : WeaponController
{

    protected override void Awake(){
        base.Awake();
    }

    protected override void Update() {
        base.Update();
    }

    protected override void Attack(){
        base.Attack();

        GameObject spawnedMissile = Instantiate(stats.SpawnableObject, transform.position, Quaternion.identity);
        spawnedMissile.GetComponent<ProjectileBehaviour>().SetDirection(GetEnemyDirection());
    }
}
