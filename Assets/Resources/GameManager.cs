using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState {
        Gameplay,
        Paused,
        GameOver,
        LevelUp
    }

    public GameState currentState;
    private GameState prevState;

    public GameObject pauseScreen;
    public GameObject resultsScreen;
    public GameObject levelUpScreen;

    [Header("Pause Screen Elements")]
    public TMP_Text currentHealth;
    public TMP_Text currentRecovery;
    public TMP_Text currentMoveSpeed;
    public TMP_Text currentAttackSpeedModifier;
    public TMP_Text currentMagnetism;

    public bool isGameOver = false;
    public bool isUpgrading = false;

    [Header("Results Screen Elements")]
    public TMP_Text levelReached;
    public TMP_Text timeSurvived;
    public List<Image> weaponList = new List<Image>(6);
    public List<Image> itemList = new List<Image>(6);

    [Header("Timer")]
    public float timeLimit;
    float timerTime;
    public TMP_Text timerText;

    public GameObject player;

    private void Awake() {
        if(instance == null) instance = this;
    }

    private void Update() {
        switch (currentState) {
            case GameState.Gameplay:
                CheckForPauseAndResume();
                UpdateTimer();
                break;

            case GameState.Paused:
                CheckForPauseAndResume();
                break;

            case GameState.GameOver:
                if (!isGameOver) {
                    isGameOver = true;
                    Time.timeScale = 0f;
                    DisplayResults();
                }
                break;

            case GameState.LevelUp:
                if (!isUpgrading) {
                    isUpgrading = true;
                    levelUpScreen.SetActive(true);
                    Time.timeScale = 0f;
                }
                break;
            default:
                Debug.LogError("Unknown game state: " + currentState);
                break;
        }
    }

    public void ChangeState(GameState newState) { currentState = newState; }

    public void DisableScreens() {
        pauseScreen.SetActive(false);
        resultsScreen.SetActive(false);
        levelUpScreen.SetActive(false);
    }

    public void PauseGame() {
        if(currentState == GameState.Paused) {
            Debug.LogWarning("Game is already paused.");
            return;
        }

        prevState = currentState;
        ChangeState(GameState.Paused);
        pauseScreen.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log("Game Paused");
    }

    public void ResumeGame() {
        if(currentState == GameState.Paused) {
            ChangeState(prevState);
            pauseScreen.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    void CheckForPauseAndResume() {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            if (currentState == GameState.Paused) ResumeGame();
            else PauseGame();
        }
    }

    public void GameOver() { ChangeState(GameState.GameOver); }

    void DisplayResults() {
        AssignTimeSurvived();
        resultsScreen.SetActive(true);

    }

    public void AssignLevelReached(int level) { levelReached.text = level.ToString(); }
    public void AssignTimeSurvived() { timeSurvived.text = timerText.text; }

    public void AssignWeaponsAndItems(List<WeaponController> weapons, List<PassiveItem> items) {
        if(weapons.Count != weaponList.Count || items.Count != itemList.Count) {
            Debug.LogError("Weapons or items list size does not match the UI elements.");
            return;
        }

        for (int i = 0; i < weaponList.Count; i++) {
            if (weapons[i] == null) {
                weaponList[i].enabled = false;
                continue;
            }
            SpriteRenderer sr = weapons[i].gameObject.GetComponent<SpriteRenderer>();
            weaponList[i].sprite = sr.sprite;
            weaponList[i].enabled = true;
        }

        for (int i = 0; i < itemList.Count; i++) {
            if (items[i] == null) {
                itemList[i].enabled = false;
                continue;
            }
            SpriteRenderer sr = items[i].gameObject.GetComponent<SpriteRenderer>();
            itemList[i].sprite = sr.sprite;
            itemList[i].enabled = true;
        }
    }

    void UpdateTimer() { 
        timerTime += Time.deltaTime;
        UpdateTimerDisplay();

        if (timerTime >= timeLimit) {
            GameOver();
        }
    }

    void UpdateTimerDisplay() {
        int minutes = Mathf.FloorToInt(timerTime / 60);
        int seconds = Mathf.FloorToInt(timerTime % 60);

        timerText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
    }

    public void StartLevelUp() { 
        ChangeState(GameState.LevelUp);
        player.SendMessage("RemoveAndApplyUpgrades");
    }

    public void EndLevelUp() {
        isUpgrading = false;
        Time.timeScale = 1f;
        levelUpScreen.SetActive(false);
        ChangeState(GameState.Gameplay);
    }
}
