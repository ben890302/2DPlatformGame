using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorControl : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private PlayerControl playerControl;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerControl = GetComponent<PlayerControl>();
    }

    private void FixedUpdate()
    {
        animator.SetBool("IsWalking", playerControl.isWalking);
        animator.SetBool("IsGrounded", playerControl.isGrounded);
        animator.SetBool("OnWall", playerControl.onWall);
        animator.SetBool("IsRolling", playerControl.isRolling);
        animator.SetBool("IsDashing", playerControl.isDashing);
        animator.SetFloat("YVelocity", rb.velocity.y);
    }
}
