using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    private Slider slider;

    private void Start() {
        slider = GetComponent<Slider>();
    }

    public void UpdateBar(float hp, float maxhp) {
        slider.maxValue = maxhp;
        slider.value = hp;
    }
}
