using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPickup : MonoBehaviour, ICollectibles
{
    public float healthAmount;
    public void Collect() {
        PlayerCore player = FindObjectOfType<PlayerCore>();
        player.Heal(healthAmount);
    }

}
