using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthbarTemp : MonoBehaviour
{
    public PlayerCore playerCore;

    void Update()
    {
        Vector3 scale = new Vector3(
            playerCore.currentHealth / playerCore.stats.MaxHealth,
            this.transform.localScale.y,
            this.transform.localScale.z
        );
        this.transform.localScale = scale;
    }
}
