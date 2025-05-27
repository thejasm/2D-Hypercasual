using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTemp : MonoBehaviour
{
    public PlayerCore playerCore;
    TMPro.TextMeshProUGUI TMP;

    private void Start() {
        TMP = GetComponent<TMPro.TextMeshProUGUI>();
    }
    void Update()
    {
        TMP.text = playerCore.level.ToString();
    }
}
