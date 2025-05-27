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
        GameOver
    }

    public GameState currentState;
    private GameState prevState;

    public GameObject pauseScreen;
    public GameObject resultsScreen;

    [Header("Pause Screen Elements")]
    public TMP_Text currentHealth;
    public TMP_Text currentRecovery;
    public TMP_Text currentMoveSpeed;
    public TMP_Text currentAttackSpeedModifier;
    public TMP_Text currentMagnetism;

    public bool isGameOver = false;

    [Header("Results Screen Elements")]
    public TMP_Text levelReached;
    public TMP_Text timeSurvived;
    public List<Image> weaponList = new List<Image>(6);
    public List<Image> itemList = new List<Image>(6);

    private void Awake() {
        if(instance == null) instance = this;
    }

    private void Update() {
        switch (currentState) {
            case GameState.Gameplay:
                CheckForPauseAndResume();
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
            default:
                Debug.LogError("Unknown game state: " + currentState);
                break;
        }
    }

    public void ChangeState(GameState newState) { currentState = newState; }

    public void DisableScreens() {
        pauseScreen.SetActive(false);
        resultsScreen.SetActive(false);
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
        resultsScreen.SetActive(true);

    }

    public void AssignLevelReached(int level) {
        levelReached.text = level.ToString();
    }

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
}
