using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PassiveItemScriptableObject;

[CreateAssetMenu (fileName = "PassiveItemScriptableObject", menuName = "ScriptableObjects/Passive Item", order = 1)]
public class PassiveItemScriptableObject : ScriptableObject
{
    [SerializeField]
    int level;
    public int Level { get => level; private set => level = value; }

    [SerializeField]
    string itemClass;
    public string ItemClass { get => itemClass; private set => itemClass = value; }

    [SerializeField]
    GameObject nextLevelPrefab;
    public GameObject NextLevelPrefab { get => nextLevelPrefab; private set => nextLevelPrefab = value; }

    [SerializeField]
    new string name;
    public string Name { get => name; private set => name = value; }

    [SerializeField]
    string description;
    public string Description { get => description; private set => description = value; }

    public enum ModifiableStatType {
        MaxHealth,
        Recovery,
        MoveSpeed,
        Magnetism,
        Damage,
        Speed,
        CooldownDuration,
        Pierce,
        ActiveDuration,
        Size
    }

    [SerializeField]
    public ModifiableStatType statToModify;
    public ModifiableStatType StatToModify { get => statToModify; private set => statToModify = value; }

    [SerializeField]
    [Tooltip("in 1 +/- multiplier/100")]
    float multiplier;
    public float Multiplier { get => multiplier; private set => multiplier = value; }
}
