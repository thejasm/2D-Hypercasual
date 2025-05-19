using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "ScriptableObjects/Enemy", order = 1)]
public class EnemyScriptableObject: ScriptableObject {

    [SerializeField]
    GameObject prefab;
    public GameObject Prefab { get => prefab; set => prefab = value; }

    [Header("Enemy Stats")] //------------------------------------------------------------------------------------- [Header("Enemy Stats")]
    [SerializeField]
    float maxHealth;
    public float MaxHealth { get => maxHealth; set => maxHealth = value; }

    [SerializeField]
    float damage;
    public float Damage { get => damage; set => damage = value; }

    [SerializeField]
    float speed;
    public float Speed { get => speed; set => speed = value; }
}
