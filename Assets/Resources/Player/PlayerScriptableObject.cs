using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "PlayerScriptableObject", menuName = "ScriptableObjects/Player", order = 1)]
public class PlayerScriptableObject : ScriptableObject
{

    [SerializeField]
    GameObject startingWeapon;
    public GameObject StartingWeapon { get { return startingWeapon; } set { startingWeapon = value; } }

    [SerializeField]
    float maxHealth;
    public float MaxHealth { get { return maxHealth; } set { maxHealth = value; } }

    [SerializeField]
    float recovery;
    public float Recovery { get { return recovery; } set { recovery = value; } }

    [SerializeField]
    float moveSpeed;
    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }

    [SerializeField]
    float might;
    public float Might { get { return might; } set { might = value; } }

    [SerializeField]
    float magnetism;
    public float Magnetism { get { return magnetism; } set { magnetism = value; } }


}
