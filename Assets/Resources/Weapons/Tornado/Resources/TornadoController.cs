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
        var missile = spawnedMissile.GetComponent<TornadoBehaviour>();
        float angle = Random.Range(0f, 360f);
        Vector3 randomDir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
        missile.SetDirection(randomDir);
        missile.speed *= stats.Speed;

    }
}
