using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventoryManager: MonoBehaviour {
    public bool levelUpWeapon = false;
    public bool levelUpItem = false;

    public List<WeaponController> weaponSlots;
    public int[] weaponLevels = new int[6];
    public List<PassiveItem> itemSlots;
    public int[] itemLevels = new int[6];

    public List<GameObject> weaponFrames;
    public List<GameObject> itemFrames;

    public const int MaxSlots = 6;

    PlayerCore player;


    [System.Serializable]
    public class WeaponUpgrade {
        public GameObject initialWeapon;
        public WeaponScriptableObject weaponData;
    }

    [System.Serializable]
    public class ItemUpgrade {
        public GameObject initialItem;
        public PassiveItemScriptableObject itemData;
    }

    [System.Serializable]
    public class UpgradeUI {
        public TMP_Text upgradeNameDisplay;
        public TMP_Text upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button button;
    }

    public List<WeaponUpgrade> weaponUpgradeOptions = new List<WeaponUpgrade>();
    public List<ItemUpgrade> itemUpgradeOptions = new List<ItemUpgrade>();
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();

    public Image chestImage;


    void Awake() {
        weaponSlots = new List<WeaponController>(new WeaponController[MaxSlots]);
        itemSlots = new List<PassiveItem>(new PassiveItem[MaxSlots]);
        player = FindObjectOfType<PlayerCore>();
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
        if (slotIndex >= 0 && slotIndex < MaxSlots) {
            WeaponController wc = weapon.GetComponent<WeaponController>();
            weaponSlots[slotIndex] = wc;
            weaponLevels[slotIndex] = wc.stats.Level;

            // Create UI image for the weapon icon
            GameObject weaponImg = new GameObject();
            weaponImg.AddComponent<Image>();
            weaponImg.GetComponent<Image>().sprite = weapon.GetComponent<SpriteRenderer>().sprite;
            weaponImg.transform.Rotate(0, 0, 180);
            weaponImg.transform.SetParent(weaponFrames[slotIndex].transform, false);
        }

        if (GameManager.instance != null && GameManager.instance.isUpgrading) GameManager.instance.EndLevelUp();
    }

    public void AddItem(int slotIndex, GameObject item) {
        if (slotIndex >= 0 && slotIndex < MaxSlots) {
            PassiveItem pi = item.GetComponent<PassiveItem>();
            itemSlots[slotIndex] = pi;
            itemLevels[slotIndex] = pi.stats.Level;

            // Create UI image for the item icon
            GameObject itemImg = new GameObject();
            itemImg.AddComponent<Image>();
            itemImg.GetComponent<Image>().sprite = item.GetComponent<SpriteRenderer>().sprite;
            itemImg.transform.Rotate(0, 0, 180);
            itemImg.transform.SetParent(itemFrames[slotIndex].transform, false);

            item.SetActive(false); // Deactivate added item's game object
        }

        if (GameManager.instance != null && GameManager.instance.isUpgrading) GameManager.instance.EndLevelUp();
    }

    public void LevelupWeapon(int slotIndex) {
        if (weaponSlots[slotIndex] == null) return;

        WeaponController wc = weaponSlots[slotIndex];
        if (wc.stats.NextLevelPrefab == null) return; // Already max level

        // Create and place the upgraded version
        GameObject upgradedWeapon = Instantiate(wc.stats.NextLevelPrefab, transform.position, Quaternion.identity);
        upgradedWeapon.transform.SetParent(transform);
        AddWeapon(slotIndex, upgradedWeapon); // Add new, replacing old in logic
        Destroy(wc.gameObject); // Remove old weapon instance
        weaponLevels[slotIndex] = upgradedWeapon.GetComponent<WeaponController>().stats.Level;

        if (GameManager.instance != null && GameManager.instance.isUpgrading) GameManager.instance.EndLevelUp();
    }

    public void LevelupItem(int slotIndex) {
        if (itemSlots[slotIndex] == null) return;

        PassiveItem pi = itemSlots[slotIndex];
        if (pi.stats.NextLevelPrefab == null) return; // Already max level

        // Create and place the upgraded version
        GameObject upgradedItem = Instantiate(pi.stats.NextLevelPrefab, transform.position, Quaternion.identity);
        AddItem(slotIndex, upgradedItem); // Add new, replacing old in logic
        pi.gameObject.SetActive(false); // Deactivate old item instance
        itemLevels[slotIndex] = upgradedItem.GetComponent<PassiveItem>().stats.Level;

        if (GameManager.instance != null && GameManager.instance.isUpgrading) GameManager.instance.EndLevelUp();
    }

    public void ApplyModifier(float modifier, int index) {
        for (int i = 0; i < MaxSlots; i++) {
            if (weaponSlots[i] != null) {
                switch (index) {
                    case 0: weaponSlots[i].currentDamage += modifier; break;
                    case 1: weaponSlots[i].currentSpeed += modifier; break;
                    case 2: weaponSlots[i].currentCooldownDuration += modifier; break;
                    case 3: weaponSlots[i].currentPierce += modifier; break;
                    case 5: weaponSlots[i].currentActiveDuration += modifier; break;
                    case 6: weaponSlots[i].currentSize += modifier; break;
                }
            }
        }
    }

    void ApplyUpgradeOptions() {
        // Use copies for current selection batch
        List<WeaponUpgrade> availableWeaponUpgrades = new List<WeaponUpgrade>(weaponUpgradeOptions);
        List<ItemUpgrade> availableItemUpgrades = new List<ItemUpgrade>(itemUpgradeOptions);

        foreach (var upgradeOption in upgradeUIOptions) {
            int upgradeType;
            // Decide: weapon or item offer for this UI slot
            if (availableWeaponUpgrades.Count == 0) upgradeType = 2;
            else if (availableItemUpgrades.Count == 0) upgradeType = 1;
            else upgradeType = Random.Range(1, 3);

            if (upgradeType == 1) { // type is weapon
                if (availableWeaponUpgrades.Count == 0) { DisableUpgradeOption(upgradeOption); continue; }
                WeaponUpgrade chosenWeaponUpgrade = availableWeaponUpgrades[Random.Range(0, availableWeaponUpgrades.Count)];
                availableWeaponUpgrades.Remove(chosenWeaponUpgrade); // Prevent duplicate offers in this batch

                if (chosenWeaponUpgrade != null) {
                    EnableUpgradeOption(upgradeOption);

                    bool newWeapon = false;
                    for (int i = 0; i < weaponSlots.Count; i++) {
                        if (weaponSlots[i] != null && weaponSlots[i].stats.WeaponClass == chosenWeaponUpgrade.weaponData.WeaponClass) {
                            newWeapon = false;
                            WeaponController equippedWeapon = weaponSlots[i]; // Get the currently equipped weapon

                            // The equipped weapon itself is at its max level
                            if (equippedWeapon.stats.NextLevelPrefab == null) DisableUpgradeOption(upgradeOption);
                            // The equipped weapon has an upgrade available
                            else {
                                upgradeOption.button.onClick.AddListener(() => LevelupWeapon(i));

                                // Get details from the NextLevelPrefab of the EQUIPPED weapon
                                WeaponController nextLevelController = equippedWeapon.stats.NextLevelPrefab.GetComponent<WeaponController>();
                                if (nextLevelController != null && nextLevelController.stats != null) {
                                    upgradeOption.upgradeNameDisplay.text = nextLevelController.stats.Name;
                                    upgradeOption.upgradeDescriptionDisplay.text = nextLevelController.stats.Description;
                                }
                                else DisableUpgradeOption(upgradeOption); // Prefab data missing
                            }
                            break;
                        }
                        else newWeapon = true;
                    }
                    if (newWeapon) {
                        // Check if this base weapon type has no next level
                        if (chosenWeaponUpgrade.weaponData.NextLevelPrefab == null) DisableUpgradeOption(upgradeOption);
                        // It's a new weapon and it has a potential upgrade path from its base form.
                        else {
                            upgradeOption.button.onClick.AddListener(() => player.AddWeaponController(chosenWeaponUpgrade.initialWeapon));
                            upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.weaponData.Name;
                            upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.weaponData.Description;
                        }
                    }
                    upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.initialWeapon.GetComponent<SpriteRenderer>().sprite;
                }
                else DisableUpgradeOption(upgradeOption);
            }
            else if (upgradeType == 2) { // type is item
                if (availableItemUpgrades.Count == 0) { DisableUpgradeOption(upgradeOption); continue; }
                ItemUpgrade chosenItemUpgrade = availableItemUpgrades[Random.Range(0, availableItemUpgrades.Count)];
                availableItemUpgrades.Remove(chosenItemUpgrade); // Prevent duplicate offers

                if (chosenItemUpgrade != null) {
                    EnableUpgradeOption(upgradeOption);

                    bool newItem = false;
                    for (int i = 0; i < itemSlots.Count; i++) {
                        if (itemSlots[i] != null && itemSlots[i].stats != null && itemSlots[i].stats.ItemClass == chosenItemUpgrade.itemData.ItemClass) {
                            newItem = false;
                            PassiveItem equippedItem = itemSlots[i]; // Get the currently equipped item

                            // The equipped item itself is at its max level
                            if (equippedItem.stats.NextLevelPrefab == null) DisableUpgradeOption(upgradeOption);
                            // The equipped item has an upgrade available
                            else {
                                upgradeOption.button.onClick.AddListener(() => LevelupItem(i));
                                PassiveItem nextLevelController = equippedItem.stats.NextLevelPrefab.GetComponent<PassiveItem>();

                                // Get details from the NextLevelPrefab of the EQUIPPED item
                                if (nextLevelController != null && nextLevelController.stats != null) {
                                    upgradeOption.upgradeNameDisplay.text = nextLevelController.stats.Name;
                                    upgradeOption.upgradeDescriptionDisplay.text = nextLevelController.stats.Description;
                                }
                                else DisableUpgradeOption(upgradeOption);
                            }
                            break;
                        }
                        else newItem = true;
                    }

                    if (newItem) {
                        // Check if this base item type has no next level
                        if (chosenItemUpgrade.itemData.NextLevelPrefab == null) DisableUpgradeOption(upgradeOption);
                        // It's a new item and it has a potential upgrade path from its base form.
                        else {
                            upgradeOption.button.onClick.AddListener(() => player.AddItem(chosenItemUpgrade.initialItem));
                            upgradeOption.upgradeNameDisplay.text = chosenItemUpgrade.itemData.Name;
                            upgradeOption.upgradeDescriptionDisplay.text = chosenItemUpgrade.itemData.Description;
                        }
                    }
                    upgradeOption.upgradeIcon.sprite = chosenItemUpgrade.initialItem.GetComponent<SpriteRenderer>().sprite;
                }
                else DisableUpgradeOption(upgradeOption);
            }
        }
    }

    void RemoveUpgradeOptions() {
        foreach (var upgradeOption in upgradeUIOptions) {
            upgradeOption.button.onClick.RemoveAllListeners();
            DisableUpgradeOption(upgradeOption);
        }
    }

    public void RemoveAndApplyUpgrades() {
        RemoveUpgradeOptions();
        ApplyUpgradeOptions();
    }

    void DisableUpgradeOption(UpgradeUI ui) { ui.button.gameObject.SetActive(false); }

    void EnableUpgradeOption(UpgradeUI ui) { ui.button.gameObject.SetActive(true); }

    public void ChestSelector() {
        int availableWeapons = weaponSlots.Count;
        int availableItems = itemSlots.Count;
        int selectionIndex = Random.Range(0, (availableWeapons + availableItems));
        if (selectionIndex < availableWeapons) LevelupWeapon(selectionIndex);
        else LevelupItem(selectionIndex - availableWeapons);

        List<Sprite> availableOptions = new List<Sprite>();
        foreach(var weapon in weaponSlots) availableOptions.Add(weapon != null ? weapon.GetComponent<SpriteRenderer>().sprite : null);
        foreach(var item in itemSlots) availableOptions.Add(item != null ? item.GetComponent<SpriteRenderer>().sprite : null);
        StartCoroutine(Spin(availableOptions, selectionIndex));
    }

    public IEnumerator Spin(List<Sprite> availableOptions, int selectionIndex) {
        Debug.LogError("starting spin coroutine");
        if (availableOptions == null || availableOptions.Count == 0) {
            Debug.LogError("Available Options list is empty or null!");
            yield break;
        }
        if (selectionIndex < 0 || selectionIndex >= availableOptions.Count) {
            Debug.LogError("Selection Index is out of bounds!");
            yield break;
        }

        float fastSpinDur = 0.5f;
        float slowDownDur = 1f;
        float totalDur = fastSpinDur + slowDownDur;

        float timePassed = 0f;
        int currentOptIndex = 0;
        int finalIndex = selectionIndex;

        // --- Rapid Spin Phase ---
        while (timePassed < fastSpinDur) {
            chestImage.sprite = availableOptions[currentOptIndex];
            currentOptIndex = (currentOptIndex + 1) % availableOptions.Count;
            timePassed += Time.unscaledDeltaTime;
            yield return new WaitForSeconds(0.05f);
        }

        // --- Slow Down Phase ---
        int cyclesToTarget = availableOptions.Count * 2;
        int effectiveTargetIndex = finalIndex + cyclesToTarget;

        float spinProgress = 0f;
        while (spinProgress < 1f) {
            spinProgress = (timePassed - fastSpinDur) / slowDownDur;

            // Use an eased curve for slowing down
            float easedProgress = 1f - (1f - spinProgress) * (1f - spinProgress);

            // Calculate the current index based on eased progress
            // Adding a large number and then modulo to handle negative results and ensure enough spins
            float currentRelativeIndex = easedProgress * effectiveTargetIndex;
            currentOptIndex = (int)Mathf.Round(currentRelativeIndex) % availableOptions.Count;

            chestImage.sprite = availableOptions[currentOptIndex];

            timePassed += Time.unscaledDeltaTime;
            yield return null;
        }

        chestImage.sprite = availableOptions[finalIndex];
    }
}