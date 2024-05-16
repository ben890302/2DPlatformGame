using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerControl : MonoBehaviour
{
    [Header("Rigidbody")]
    private Rigidbody2D rb;
    private float playerGravity;

    [Header("Move")]
    private float movement;
    public bool isWalking;
    public float walkSpeed;

    [Header("Jump")]
    public bool isJumping;
    public float jumpPower;
    public float jumpMaxTime;
    private float jumpTime;

    [Header("Wall Slide")]
    public bool isWallSliding;
    public float wallSlideSpeed;

    [Header("Wall Jump")]
    public bool isWallJumping;
    private float wallJumpDirection;
    public float wallJumpDuration;
    private float wallJumpTime;
    public Vector2 wallJumpPower;

    [Header("Roll")]
    private bool canRoll;
    public bool isRolling;
    public float rollSpeed;

    [Header("Dash")]
    private bool canDash;
    public bool isDashing;
    public float dashPower;
    public float dashDuration;
    public float dashCooldown;

    [Header("Attack")]
    public bool isAttacking;

    [Header("Flip")]
    public bool isFacingRight;

    [Header("Ground Check")]
    public bool isGrounded;
    public Transform groundCheckPos;
    public Vector2 groundCheckSize;
    public LayerMask groundLayer;

    [Header("Wall Check")]
    public bool onWall;
    public Transform wallCheckPos;
    public Vector2 wallCheckSize;
    public LayerMask wallLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerGravity = rb.gravityScale;
        canRoll = true;
        canDash = true;
        isFacingRight = true;
    }

    private void FixedUpdate()
    {
        if(isDashing)
        {
            return;
        }
        GroundCheck();
        WallCheck();
        WallSlide();
        CanWallJump();
        if(!isWallJumping && !isDashing && !isRolling)
        {
            rb.velocity = new Vector2(movement * walkSpeed, rb.velocity.y);
            Flip();
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>().x;
        if (context.performed)
        {
            isWalking = true;
        }
        if(context.canceled)
        {
            isWalking = false;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpTime > 0)
        {
            if (context.performed)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
                jumpTime--;
            }
            if (context.canceled)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
                jumpTime--;
            }
        }
    }

    private void WallSlide()
    {
        if (!isGrounded && onWall)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }

    public void WallJump(InputAction.CallbackContext context)
    {
        if (context.performed && wallJumpTime > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpTime = 0f;
            if (transform.localScale.x != wallJumpDirection)
            {
                isFacingRight = !isFacingRight;
                Vector2 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
            Invoke(nameof(CancelWallJump), wallJumpDuration);
        }
    }

    private void CanWallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpTime = 1f;
            CancelInvoke(nameof(CancelWallJump));
        }
        else
        {
            wallJumpTime = 0f;
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }

    public void Roll(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && canRoll)
        {
            isRolling = true;
            canRoll = false;
            rb.velocity = new Vector2(transform.localScale.x * rollSpeed, rb.velocity.y);
        }
    }

    public void EndRoll()
    {
        isRolling = false;
        canRoll = true;
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {

        canDash = false;
        isDashing = true;
        float dashDirection = isFacingRight ? 1f : -1f;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(dashDirection * dashPower, 0f);
        yield return new WaitForSeconds(dashDuration);
        rb.gravityScale = playerGravity;
        rb.velocity = new Vector2(0f, rb.velocity.y);
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void Attack(InputAction.CallbackContext context)
    {
        isAttacking = true;
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    private void Flip()
    {
        if (movement > 0 && !isFacingRight || movement < 0 && isFacingRight)
        {
            isFacingRight = !isFacingRight;
            Vector2 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            isGrounded = true;
            jumpTime = jumpMaxTime;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void WallCheck()
    {
        if (Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer))
        {
            onWall = true;
        }
        else
        {
            onWall = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }
}
