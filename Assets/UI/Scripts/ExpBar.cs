using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpBar: MonoBehaviour
{
    private Slider slider;
    private Coroutine anim;
    public float animDuration = 0.2f;

    private void Start() {
        slider = GetComponent<Slider>();
    }

    public void UpdateBar(float exp, float expCap) {
        if (expCap != slider.maxValue) {
            anim = StartCoroutine(LevelUpAnim(exp, expCap));
        } else {
            if (anim != null) StopCoroutine(anim);
            anim = StartCoroutine(AnimateBar(exp, animDuration));
        }
    }

    private IEnumerator AnimateBar(float exp, float dur) {
        float startVal = slider.value;
        float time = 0;

        while (time < dur) {
            time += Time.deltaTime;
            slider.value = Mathf.Lerp(startVal, exp, time / dur);
            yield return null;
        }

        slider.value = exp;
        anim = null;
    }


    private IEnumerator LevelUpAnim(float newExp, float newExpCap) {
        yield return StartCoroutine(AnimateBar(slider.maxValue, animDuration));

        slider.value = 0;
        slider.maxValue = newExpCap;

        yield return StartCoroutine(AnimateBar(newExp, animDuration));

        anim = null;
    }
}
