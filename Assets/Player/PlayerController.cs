using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 moveDir = Vector2.zero;
    private Animator anim;
    private SpriteRenderer sprite;

    public float moveSpeed = 5f;

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update(){
        InputManager();
        AnimationHandler();
    }

    void FixedUpdate() {
        Move();
    }

    void InputManager() {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        moveDir = new Vector2(moveX, moveY).normalized;
    }

    void Move() {
        rb.velocity = new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
    }

    void AnimationHandler() {
        if (rb.velocity.magnitude > 0.1) anim.SetBool("walk", true);
        else anim.SetBool("walk", false);

        if(moveDir.x > 0) sprite.flipX = false;
        else if(moveDir.x < 0) sprite.flipX = true;
    }
}
