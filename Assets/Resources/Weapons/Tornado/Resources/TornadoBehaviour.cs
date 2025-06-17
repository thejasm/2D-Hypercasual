using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TornadoBehaviour: ProjectileBehaviour {
    public float speed = 5f;
    public float rotationalAcceleration = 30f;

    private float currentAngularVelocity = 0f;
    private Vector2 moveDirection;

    protected override void Start() {
        weaponController = FindAnyObjectByType<TornadoController>();
        moveDirection = direction.normalized;
        projectilePierce = 99;
        Destroy(gameObject, destroyAfter);
    }

    protected override void Update() {
        base.Update();

        currentAngularVelocity += rotationalAcceleration * Time.deltaTime;

        float angleDelta = currentAngularVelocity * Time.deltaTime;
        moveDirection = Quaternion.Euler(0, 0, angleDelta) * moveDirection;

        transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);

        transform.rotation = Quaternion.identity;
    }
}