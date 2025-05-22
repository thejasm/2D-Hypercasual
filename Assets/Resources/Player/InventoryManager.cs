using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager: MonoBehaviour {
    public List<WeaponController> weaponSlots;
    public int[] weaponLevels = new int[6];
    public List<PassiveItem> itemSlots;
    public int[] itemLevels = new int[6];

    public List<GameObject> weaponFrames;
    public List<GameObject> itemFrames;

    public const int MaxSlots = 6;

    void Awake() {
        weaponSlots = new List<WeaponController>(new WeaponController[MaxSlots]);
        itemSlots = new List<PassiveItem>(new PassiveItem[MaxSlots]);
    }

    public void AddWeapon(int slotIndex, GameObject weapon) {
        Debug.Log("Adding weapon to slot " + slotIndex);
        if (slotIndex >= 0 && slotIndex < MaxSlots) {
            weaponSlots[slotIndex] = weapon.GetComponent<WeaponController>();
        }
    }

    public void AddItem(int slotIndex, GameObject item) {
        Debug.Log("Adding item to slot " + slotIndex);
        if (slotIndex >= 0 && slotIndex < MaxSlots) {
            itemSlots[slotIndex] = item.GetComponent<PassiveItem>();
        }

        item.transform.parent = itemFrames[slotIndex].transform;
    }

    public void LevelupWeapon(int slotIndex) {
    }

    public void LevelupItem(int slotIndex) {
    }

    public void ApplyModifier(WeaponScriptableObject newStats, int index) {
        for (int i = 0; i < MaxSlots; i++) {
            if (weaponSlots[i] != null) {
                switch (index) {
                    case 0:
                        weaponSlots[i].currentDamage += newStats.Damage;
                        break;
                    case 1:
                        weaponSlots[i].currentSpeed += newStats.Speed;
                        break;
                    case 2:
                        weaponSlots[i].currentCooldownDuration += newStats.CooldownDuration;
                        break;
                    case 3:
                        weaponSlots[i].currentPierce += newStats.Pierce;
                        break;
                    case 5:
                        weaponSlots[i].currentActiveDuration += newStats.ActiveDuration;
                        break;
                    case 6:
                        weaponSlots[i].currentSize += newStats.Size;
                        break;
                }
            }
        }
    }
}