using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [Header("Player informations")]
    [SerializeField] private float cc_MovementSmoothing = .05f;
    [SerializeField] private float cc_JumpForce = 10f;
    [SerializeField] private bool cc_AirControl = false;
    public Transform cc_GroundCheck;
    public LayerMask cc_WhatIsGround;
    public bool cc_IsFacingRight = true;
    public bool cc_IsGrounded = true;

    const float cc_GroundedRadius = .1f;

    private Vector3 cc_VelocityZero = Vector3.zero;
    private Rigidbody2D cc_Rigidbody2D;

    [Header("Events")]
    public UnityEvent OnLandEvent;

    private void Awake() {
        cc_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null) {
            OnLandEvent = new UnityEvent();
        }
    }

    private void FixedUpdate() {
        bool wasGrounded = cc_IsGrounded;
        cc_IsGrounded = false;

        /*cc_IsGrounded = cc_Rigidbody2D.IsTouchingLayers(cc_WhatIsGround);

        if (cc_IsGrounded && !wasGrounded) {
            OnLandEvent.Invoke();
        }*/

        Collider2D[] colliders = Physics2D.OverlapCircleAll(cc_GroundCheck.position, cc_GroundedRadius, cc_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++) {
            if (colliders[i].gameObject != gameObject) {
                cc_IsGrounded = true;
                if (!wasGrounded) {
                    OnLandEvent.Invoke();
                }
            }
        }
    }

    public void Move(float move, bool jump) {
        if (cc_IsGrounded || cc_AirControl) {
            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2((move * Time.fixedDeltaTime) * 10f, cc_Rigidbody2D.velocity.y);
            // And then smoothing it out and applying it to the character
            cc_Rigidbody2D.velocity = Vector3.SmoothDamp(cc_Rigidbody2D.velocity, targetVelocity, ref cc_VelocityZero, cc_MovementSmoothing);

            if ((move > 0 && !cc_IsFacingRight) || (move < 0 && cc_IsFacingRight)) {
                Flip();
            }
        }

        if (cc_IsGrounded && jump) {
            Jump();
        }
    }

    private void Jump() {
        //cc_Rigidbody2D.AddForce(new Vector2(0f, cc_JumpForce));
        cc_Rigidbody2D.velocity = new Vector2(cc_Rigidbody2D.velocity.x, cc_JumpForce);
    }

    private void Flip() {
        cc_IsFacingRight = !cc_IsFacingRight;
        //transform.Rotate(0f, 180f, 0f);

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
