using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponScriptableObject", menuName = "ScriptableObjects/Weapon", order = 1)]
public class WeaponScriptableObject: ScriptableObject {

    [SerializeField]
    GameObject prefab;
    public GameObject Prefab { get => prefab; set => prefab = value; }

    [Header("Base Stats")] //------------------------------------------------------------------------------------- [Header("Base Stats")]
    [SerializeField]
    float damage = 1;
    public float Damage { get => damage; set => damage = value; }

    [SerializeField]
    float speed = 1;
    public float Speed { get => speed; set => speed = value; }

    [SerializeField]
    float cooldownDuration = 1f;
    public float CooldownDuration { get => cooldownDuration; set => cooldownDuration = value; }

    [SerializeField]
    float pierce = 1;
    public float Pierce { get => pierce; set => pierce = value; }

    [Header("Melee Stats")] //----------------------------------------------------------------------------------- [Header("Melee Stats")]
    [SerializeField]
    Vector3 offset = Vector3.zero;
    public Vector3 Offset { get => offset; set => offset = value; }

    [Header("Lingering Stats")] //------------------------------------------------------------------------------- [Header("Lingering Stats")]
    [SerializeField]
    float activeDuration = 1f;
    public float ActiveDuration { get => activeDuration; set => activeDuration = value; }

    [SerializeField]
    float size = 1f;
    public float Size { get => size; set => size = value; }

}
