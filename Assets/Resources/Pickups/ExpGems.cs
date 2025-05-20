using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpGems : MonoBehaviour, ICollectibles
{
    public int expValue = 1;
    public void Collect() {
        PlayerCore player = FindObjectOfType<PlayerCore>();
        player.GainXP(expValue);
    }
}
