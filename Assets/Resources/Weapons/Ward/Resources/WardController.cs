using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WardController : WeaponController 
{
    void Start(){
        base.Awake();
        Attack();
    }
    protected override void Attack() {
        base.Attack();
        GameObject spawnedWard = Instantiate(stats.SpawnableObject, transform.position, Quaternion.identity);
        spawnedWard.transform.parent = transform;
        spawnedWard.transform.localScale = new Vector3(stats.Size, stats.Size, stats.Size);
    }
}
