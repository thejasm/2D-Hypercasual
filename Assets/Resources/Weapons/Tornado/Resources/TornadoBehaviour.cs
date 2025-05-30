using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TornadoBehaviour : ProjectileBehaviour {
    public float waveAmplitude = 1f; // How far the projectile moves sideways
    public float waveFrequency = 5f;
    private Vector2 perpendicularDir;
    private float lifetime = 0f;

    protected override void Start() {
        base.Start();
        perpendicularDir = new Vector2(-direction.y, direction.x).normalized;
    }

    protected override void Update() {
        base.Update();

        lifetime += Time.deltaTime;
        float sinValue = Mathf.Sin(lifetime * waveFrequency);
        Vector2 sineOffset = perpendicularDir * sinValue * waveAmplitude;
        transform.position += (Vector3)sineOffset;

        transform.rotation = Quaternion.identity;
    }
}
