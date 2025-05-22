using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCore : MonoBehaviour
{
    public PlayerScriptableObject stats;
    private Healthbar healthbar;

    [Header("Current Stats")]
    public float currentHealth;
    public float currentRecovery;
    public float currentMoveSpeed;
    public float currentAttackSpeedModifier;
    public float currentMagnetism;

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

        AddWeaponController(stats.StartingWeapon);
    }

    public void Start() {
        expCap = levelRanges[0].expCapIncrease;
        expBar = GameObject.Find("ExpBar").GetComponent<ExpBar>();
        healthbar = GameObject.Find("Healthbar").GetComponent<Healthbar>();

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
        expGems = new List<GameObject>();
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
        exp += amount;
        LevelUpCheck();
        expBar.UpdateBar(exp, expCap);
    }

    public void LevelUpCheck() {
        if (exp >= expCap) {
            level++;
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

        GameObject spawnedWeapon = Instantiate(weaponPrefab, transform.position, Quaternion.identity);
        spawnedWeapon.transform.parent = transform;
        inventory.AddWeapon(weaponIndex++, spawnedWeapon);
    }
    public void AddItem(GameObject itemPrefab) {
        if (itemIndex >= InventoryManager.MaxSlots) {
            Debug.LogWarning("No more item slots available.");
            return;
        }

        WeaponScriptableObject weaponStat = WeaponScriptableObject.CreateInstance<WeaponScriptableObject>();

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
                weaponStat.Damage *= 1 + (itemPrefab.GetComponent<PassiveItem>().stats.Multiplier / 100);
                inventory.ApplyModifier(weaponStat, 0);
                break;
            case PassiveItemScriptableObject.ModifiableStatType.Speed:
                weaponStat.Speed *= 1 + (itemPrefab.GetComponent<PassiveItem>().stats.Multiplier / 100);
                inventory.ApplyModifier(weaponStat, 1);
                break;
            case PassiveItemScriptableObject.ModifiableStatType.CooldownDuration:
                weaponStat.CooldownDuration *= 1 + (itemPrefab.GetComponent<PassiveItem>().stats.Multiplier / 100);
                inventory.ApplyModifier(weaponStat, 2);
                break;
            case PassiveItemScriptableObject.ModifiableStatType.Pierce:
                weaponStat.Pierce *= 1 + (itemPrefab.GetComponent<PassiveItem>().stats.Multiplier / 100);
                inventory.ApplyModifier(weaponStat, 3);
                break;
            case PassiveItemScriptableObject.ModifiableStatType.ActiveDuration:
                weaponStat.ActiveDuration *= 1 + (itemPrefab.GetComponent<PassiveItem>().stats.Multiplier / 100);
                inventory.ApplyModifier(weaponStat, 5);
                break;
            case PassiveItemScriptableObject.ModifiableStatType.Size:
                weaponStat.Size *= 1 + (itemPrefab.GetComponent<PassiveItem>().stats.Multiplier / 100);
                inventory.ApplyModifier(weaponStat, 6);
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

        Debug.Log("Player is dead");
    }

    public void IFrameTimer() {
        if (iFramesTimer > 0) iFramesTimer -= Time.deltaTime;
        else isInvincible = false;
    }

    void Recover() { if (currentHealth < stats.MaxHealth) currentHealth += currentRecovery * Time.deltaTime; }

    // Player Movement ----------------------------------------------------- Player Movement 
    void InputManager() {
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
