using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSelector : MonoBehaviour
{
    public static PowerSelector instance;
    public PlayerScriptableObject playerData;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Debug.LogWarning("PowerSelector instance already exists. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    public static PlayerScriptableObject GetData() { return instance.playerData; }

    public void SelectPower(PlayerScriptableObject power) { playerData = power; }

    public void DestroySingleton() {
        instance = null;
        Destroy(gameObject);
    }
}
