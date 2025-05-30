using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCore : MonoBehaviour
{
    public PlayerScriptableObject stats;
    private Healthbar healthbar;

    #region Stats
    [Header("Current Stats")] 
    float currentHealth;
    public float CurrentHealth {
        get { return currentHealth; }
        set {
            currentHealth = value;
            if (GameManager.instance != null) {
                GameManager.instance.currentHealth.text = currentHealth.ToString();
            }
        }
    }

    float currentRecovery;
    public float CurrentRecovery {
        get { return currentRecovery; }
        set {
            currentRecovery = value;
            if (GameManager.instance != null && GameManager.instance.currentRecovery != null) {
                GameManager.instance.currentRecovery.text = currentRecovery.ToString();
            }
        }
    }

    float currentMoveSpeed;
    public float CurrentMoveSpeed {
        get { return currentMoveSpeed; }
        set {
            currentMoveSpeed = value;
            if (GameManager.instance != null && GameManager.instance.currentMoveSpeed != null) {
                GameManager.instance.currentMoveSpeed.text = currentMoveSpeed.ToString();
            }
        }
    }

    float currentAttackSpeedModifier;
    public float CurrentAttackSpeedModifier {
        get { return currentAttackSpeedModifier; }
        set {
            currentAttackSpeedModifier = value;
            if (GameManager.instance != null && GameManager.instance.currentAttackSpeedModifier != null) {
                GameManager.instance.currentAttackSpeedModifier.text = currentAttackSpeedModifier.ToString();
            }
        }
    }

    float currentMagnetism;
    public float CurrentMagnetism {
        get { return currentMagnetism; }
        set {
            currentMagnetism = value;
            if (GameManager.instance != null && GameManager.instance.currentMagnetism != null) {
                GameManager.instance.currentMagnetism.text = currentMagnetism.ToString();
            }
        }
    }
    #endregion
    
    [Header("XP and Level")]
    public int exp = 0;
    public int level = 1;
    public int expCap = 0;
    private ExpBar expBar;

    [Header("I Frames")]
    public float iFramesDuration = 0.5f;
    float iFramesTimer = 0f;
    bool isInvincible = false;

    [Header("Magnetism")]
    public float magnetSpeed = 5f;
    [HideInInspector]
    public List<GameObject> expGems;

    [Header("Movement")]
    private Rigidbody2D rb;
    private Vector2 moveDir = Vector2.zero;
    private Animator anim;
    private bool isFacingRight = true;

    InventoryManager inventory;
    public int weaponIndex = 0;
    public int itemIndex = 0;


    private void Awake() {
        if (PowerSelector.instance != null) { 
            stats = PowerSelector.GetData(); 
            PowerSelector.instance.DestroySingleton();
        } else Debug.LogWarning("PowerSelector instance is null. Using default stats.");

        inventory = GetComponent<InventoryManager>();

        currentHealth = stats.MaxHealth;
        currentRecovery = stats.Recovery;
        currentMoveSpeed = stats.MoveSpeed;
        currentMagnetism = stats.Magnetism;
    }

    public void Start() {
        expCap = levelRanges[0].expCapIncrease;
        expBar = GameObject.Find("ExpBar").GetComponent<ExpBar>();
        healthbar = GameObject.Find("Healthbar").GetComponent<Healthbar>();

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
        expGems = new List<GameObject>();

        AddWeaponController(stats.StartingWeapon);

        GameManager.instance.DisableScreens();

        if (GameManager.instance != null) {
            GameManager.instance.currentHealth.text = currentHealth.ToString();
            GameManager.instance.currentRecovery.text = currentRecovery.ToString();
            GameManager.instance.currentMoveSpeed.text = currentMoveSpeed.ToString();
            GameManager.instance.currentAttackSpeedModifier.text = currentAttackSpeedModifier.ToString();
            GameManager.instance.currentMagnetism.text = currentMagnetism.ToString();
        }
    }

    void Update() {
        IFrameTimer();

        Recover();

        InputManager();
        AnimationHandler();
        FlipHandler();

        MagnetismHandler();
    }

    void FixedUpdate() {
        Move();
    }

    // Levelling System ------------------------------------------------------- Levelling System 
    [System.Serializable]
    public class LevelRange 
    {
        public int startLevel;
        public int endLevel;
        public int expCapIncrease;
    }

    public List<LevelRange> levelRanges;

    public void GainXP(int amount) {
        Debug.Log("exp: " + exp + " gaining: " + amount + " cap: " + expCap);
        exp += amount;
        LevelUpCheck();
        expBar.UpdateBar(exp, expCap);
    }

    public void LevelUpCheck() {
        Debug.Log("Check level up");
        if (exp >= expCap) {
            Debug.Log("Level up");
            level++;
            GameManager.instance.StartLevelUp();
            exp -= expCap;

            int expCapIncrease = 0;
            foreach (LevelRange range in levelRanges) {
                if (level >= range.startLevel && level <= range.endLevel) {
                    expCapIncrease = range.expCapIncrease;
                    break;
                }
            }
            expCap += expCapIncrease;

        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if(col.CompareTag("ExpGem") ||
           col.CompareTag("Food")) {
            ICollectibles collectible = col.GetComponent<ICollectibles>();
            if (collectible != null) {
                collectible.Collect();
                Destroy(col.gameObject);
            }
        }
    }

    // Weapon System ----------------------------------------------------- Weapon & Item System 
    public void AddWeaponController(GameObject weaponPrefab) {
        if(weaponIndex >= InventoryManager.MaxSlots) {
            Debug.LogWarning("No more weapon slots available.");
            return;
        }

        for (int i = 0; i < weaponIndex; i++) {
            if (inventory.weaponSlots[i].stats.WeaponClass == weaponPrefab.GetComponent<WeaponController>().stats.WeaponClass) {
                inventory.LevelupItem(i);
            }
        }

        GameObject spawnedWeapon = Instantiate(weaponPrefab, transform.position, Quaternion.identity);
        spawnedWeapon.transform.parent = transform;
        inventory.AddWeapon(weaponIndex++, spawnedWeapon);
    }
    public void AddItem(GameObject itemPrefab) {
        if (itemIndex >= InventoryManager.MaxSlots) {
            Debug.LogWarning("No more item slots available.");
            return;
        }

        for (int i = 0; i < itemIndex; i++) {
            if (inventory.itemSlots[i].stats.ItemClass == itemPrefab.GetComponent<PassiveItem>().stats.ItemClass) {
                inventory.LevelupItem(i);
            }
        }

        float modifier = 0;

        switch (itemPrefab.GetComponent<PassiveItem>().stats.statToModify) {
            case PassiveItemScriptableObject.ModifiableStatType.MaxHealth:
                currentHealth *= 1 + (itemPrefab.GetComponent<PassiveItem>().stats.Multiplier / 100);
                break;
            case PassiveItemScriptableObject.ModifiableStatType.Recovery:
                currentRecovery *= 1 + (itemPrefab.GetComponent<PassiveItem>().stats.Multiplier / 100);
                break;
            case PassiveItemScriptableObject.ModifiableStatType.MoveSpeed:
                currentMoveSpeed *= 1 + (itemPrefab.GetComponent<PassiveItem>().stats.Multiplier / 100);
                break;
            case PassiveItemScriptableObject.ModifiableStatType.Magnetism:
                currentMagnetism *= 1 + (itemPrefab.GetComponent<PassiveItem>().stats.Multiplier / 100);
                break;
            case PassiveItemScriptableObject.ModifiableStatType.Damage:
                modifier *= 1 + (itemPrefab.GetComponent<PassiveItem>().stats.Multiplier / 100);
                inventory.ApplyModifier(modifier, 0);
                break;
            case PassiveItemScriptableObject.ModifiableStatType.Speed:
                modifier *= 1 + (itemPrefab.GetComponent<PassiveItem>().stats.Multiplier / 100);
                inventory.ApplyModifier(modifier, 1);
                break;
            case PassiveItemScriptableObject.ModifiableStatType.CooldownDuration:
                modifier *= 1 - (itemPrefab.GetComponent<PassiveItem>().stats.Multiplier / 100);
                inventory.ApplyModifier(modifier, 2);
                break;
            case PassiveItemScriptableObject.ModifiableStatType.Pierce:
                modifier *= 1 + (itemPrefab.GetComponent<PassiveItem>().stats.Multiplier / 100);
                inventory.ApplyModifier(modifier, 3);
                break;
            case PassiveItemScriptableObject.ModifiableStatType.ActiveDuration:
                modifier *= 1 - (itemPrefab.GetComponent<PassiveItem>().stats.Multiplier / 100);
                inventory.ApplyModifier(modifier, 5);
                break;
            case PassiveItemScriptableObject.ModifiableStatType.Size:
                modifier *= 1 + (itemPrefab.GetComponent<PassiveItem>().stats.Multiplier / 100);
                inventory.ApplyModifier(modifier, 6);
                break;
        }

        inventory.AddItem(itemIndex++, itemPrefab);
    }

    // Player Damage and Death ----------------------------------------------- Player Damage and Death 
    public void TakeDamage(float damage) {
        if(isInvincible) return;

        currentHealth -= damage;

        healthbar.UpdateBar(currentHealth, stats.MaxHealth);

        isInvincible = true;
        iFramesTimer = iFramesDuration;

        if (currentHealth <= 0) Die();
    }

    public void Heal(float amount) {
        currentHealth += amount;
        if (currentHealth > stats.MaxHealth) {
            currentHealth = stats.MaxHealth;
        }
        healthbar.UpdateBar(currentHealth, stats.MaxHealth);
    }

    public void Die() {
        if (!GameManager.instance.isGameOver) {
            GameManager.instance.AssignLevelReached(level);
            GameManager.instance.AssignWeaponsAndItems(inventory.weaponSlots, inventory.itemSlots);
            GameManager.instance.GameOver();
        }
    }

    public void IFrameTimer() {
        if (iFramesTimer > 0) iFramesTimer -= Time.deltaTime;
        else isInvincible = false;
    }

    void Recover() { if (currentHealth < stats.MaxHealth) currentHealth += currentRecovery * Time.deltaTime; }

    // Player Movement ----------------------------------------------------- Player Movement 
    void InputManager() {
        if (GameManager.instance.isGameOver) return;
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDir = new Vector2(moveX, moveY).normalized;
    }

    void Move() { rb.velocity = moveDir * currentMoveSpeed; }

    void AnimationHandler() { anim.SetBool("walk", rb.velocity.magnitude > 0.2f); }

    void FlipHandler() {
        if (moveDir.x > 0 && !isFacingRight) Flip();
        else if (moveDir.x < 0 && isFacingRight) Flip();
    }

    void Flip() {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // Magnetism ---------------------------------------------------------- Magnetism 
    void MagnetismHandler() {
        for (int i = 0; i < expGems.Count; i++) {
            GameObject gem = expGems[i];

            if (gem == null) continue;

            float distToGem = Vector2.Distance(transform.position, gem.transform.position);

            if (distToGem <= currentMagnetism) {
                gem.transform.position = Vector2.MoveTowards(gem.transform.position, transform.position, magnetSpeed * Time.deltaTime);
            }
        }
    }
}
