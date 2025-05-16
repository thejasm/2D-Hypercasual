using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 moveDir = Vector2.zero;
    private Animator anim;
    private bool isFacingRight = true;

    public float moveSpeed = 5f;

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update(){
        InputManager();
        AnimationHandler();
        FlipHandler();
    }

    void FixedUpdate() {
        Move();
    }

    void InputManager() {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDir = new Vector2(moveX, moveY).normalized;
    }

    void Move() {
        rb.velocity = moveDir * moveSpeed;
    }

    void AnimationHandler() {
        anim.SetBool("walk", rb.velocity.magnitude > 0.2f);
    }

    void FlipHandler() {
        if (moveDir.x > 0 && !isFacingRight) Flip();
        else if (moveDir.x < 0 && isFacingRight) Flip();
    }

    void Flip() {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
