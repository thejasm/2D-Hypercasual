using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponScriptableObject", menuName = "ScriptableObjects/Weapon", order = 1)]
public class WeaponScriptableObject: ScriptableObject {

    [SerializeField]
    GameObject prefab;
    public GameObject Prefab { get => prefab; private set => prefab = value; }

    [SerializeField]
    string weaponClass;
    public string WeaponClass { get => weaponClass; private set => weaponClass = value; }

    [SerializeField]
    int level;
    public int Level { get => level; private set => level = value; }

    [SerializeField]
    GameObject nextLevelPrefab;
    public GameObject NextLevelPrefab { get => nextLevelPrefab; private set => nextLevelPrefab = value; }

    [SerializeField]
    new string name;
    public string Name { get => name; private set => name = value; }

    [SerializeField]
    string description;
    public string Description { get => description; private set => description = value; }

    [Header("Base Stats")] //------------------------------------------------------------------------------------- [Header("Base Stats")]
    [SerializeField]
    GameObject spawnableObject;
    public GameObject SpawnableObject { get => spawnableObject; private set => spawnableObject = value; }

    [SerializeField]
    float damage = 1;
    public float Damage { get => damage; private set => damage = value; }

    [SerializeField]
    float speed = 1;
    public float Speed { get => speed; private set => speed = value; }

    [SerializeField]
    float cooldownDuration = 1f;
    public float CooldownDuration { get => cooldownDuration; private set => cooldownDuration = value; }

    [SerializeField]
    float pierce = 1;
    public float Pierce { get => pierce; private set => pierce = value; }

    [Header("Melee Stats")] //----------------------------------------------------------------------------------- [Header("Melee Stats")]
    [SerializeField]
    Vector3 offset = Vector3.zero;
    public Vector3 Offset { get => offset; private set => offset = value; }

    [Header("Lingering Stats")] //------------------------------------------------------------------------------- [Header("Lingering Stats")]
    [SerializeField]
    float activeDuration = 1f;
    public float ActiveDuration { get => activeDuration; private set => activeDuration = value; }

    [SerializeField]
    float size = 1f;
    public float Size { get => size; private set => size = value; }

}
