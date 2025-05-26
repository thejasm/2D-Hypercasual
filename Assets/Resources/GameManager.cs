using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    public TMP_Text currentHealth;
    public TMP_Text currentRecovery;
    public TMP_Text currentMoveSpeed;
    public TMP_Text currentAttackSpeedModifier;
    public TMP_Text currentMagnetism;

    private void Awake() {
        if(instance == null) {
            instance = this;
        }
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
                break;
            default:
                Debug.LogError("Unknown game state: " + currentState);
                break;
        }
    }

    public void ChangeState(GameState newState) {
        currentState = newState;
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
}
