using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PassiveItemScriptableObject;

[CreateAssetMenu (fileName = "PassiveItemScriptableObject", menuName = "ScriptableObjects/Passive Item", order = 1)]
public class PassiveItemScriptableObject : ScriptableObject
{
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
    public ModifiableStatType StatToModify { get => statToModify; set => statToModify = value; }

    [SerializeField]
    [Tooltip("in 1 + multiplier/100")]
    float multiplier;
    public float Multiplier { get => multiplier; set => multiplier = value; }
}
