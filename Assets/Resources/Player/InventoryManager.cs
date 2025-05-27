using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventoryManager: MonoBehaviour {
    public bool levelUpWeapon = false;
    public  bool levelUpItem = false;

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

    private void Update() {
        if (levelUpItem) {
            LevelupItem(0);
            levelUpItem = false;
        }
        if (levelUpWeapon) {
            LevelupWeapon(0);
            levelUpWeapon = false;
        }
    }

    public void AddWeapon(int slotIndex, GameObject weapon) {
        Debug.Log("Adding weapon to slot " + slotIndex);
        if (slotIndex >= 0 && slotIndex < MaxSlots) {
            WeaponController wc = weapon.GetComponent<WeaponController>();
            weaponSlots[slotIndex] = wc;
            weaponLevels[slotIndex] = wc.stats.Level;

            GameObject weaponImg = new GameObject();
            weaponImg.AddComponent<Image>();
            weaponImg.GetComponent<Image>().sprite = weapon.GetComponent<SpriteRenderer>().sprite;
            weaponImg.transform.SetParent(weaponFrames[slotIndex].transform, false);
        }


    }

    public void AddItem(int slotIndex, GameObject item) {
        Debug.Log("Adding item to slot " + slotIndex);
        if (slotIndex >= 0 && slotIndex < MaxSlots) {
            PassiveItem pi = item.GetComponent<PassiveItem>();
            itemSlots[slotIndex] = pi;
            itemLevels[slotIndex] = pi.stats.Level;

            item.transform.SetParent(itemFrames[slotIndex].transform);
            item.GetComponent<RectTransform>().localPosition = Vector3.zero;
        }
    }

    public void LevelupWeapon(int slotIndex) {
        if (weaponSlots[slotIndex] == null) {
            Debug.LogWarning("No weapon in slot " + slotIndex + " to level up.");
            return;
        }

        WeaponController wc = weaponSlots[slotIndex];

        if(wc.stats.NextLevelPrefab == null) {
            Debug.LogWarning("Max upgrade reached for " + wc.name);
            return;
        }

        GameObject upgradedWeapon = Instantiate(wc.stats.NextLevelPrefab, transform.position, Quaternion.identity);
        upgradedWeapon.transform.SetParent(transform);
        AddWeapon(slotIndex, upgradedWeapon);
        Destroy(wc.gameObject);
        weaponLevels[slotIndex] = upgradedWeapon.GetComponent<WeaponController>().stats.Level;
    }

    public void LevelupItem(int slotIndex) {
        if (itemSlots[slotIndex] == null) {
            Debug.LogWarning("No item in slot " + slotIndex + " to level up.");
            return;
        }

        PassiveItem pi = itemSlots[slotIndex];

        if (pi.stats.NextLevelPrefab == null) {
            Debug.LogWarning("Max upgrade reached for " + pi.name);
            return;
        }


        GameObject upgradedItem = Instantiate(pi.stats.NextLevelPrefab, transform.position, Quaternion.identity);
        AddItem(slotIndex, upgradedItem);
        Destroy(pi.gameObject);
        itemLevels[slotIndex] = upgradedItem.GetComponent<PassiveItem>().stats.Level;
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