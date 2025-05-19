using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WardController : WeaponContorller 
{
    protected override void Start(){
        base.Start();
    }
    protected override void Attack() {
        base.Attack();
        GameObject spawnedWard = Instantiate(stats.Prefab, transform.position, Quaternion.identity);
        //spawnedWard.transform.parent = transform;
        spawnedWard.transform.localScale = new Vector3(stats.Size, stats.Size, stats.Size);
        spawnedWard.GetComponent<WardBehaviour>().destroyAfter = stats.ActiveDuration;
    }
}
