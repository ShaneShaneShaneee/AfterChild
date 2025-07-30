using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IMovable
{
    public MovementStats MovementStats;

    [SerializeField] GameObject ObjectModel;
    [SerializeField] Rigidbody ObjectRigidbody;
    [SerializeField] Collider ObjectCollider;

    [SerializeField] float FallMultiplier = 2.5f;
    [SerializeField] float JumpCount;
    [SerializeField] float DashValue;

    float TotalJumpCount = 1;
    float LeftRightMovement;
    float UpDownMovement;

    float MaxJumpCharge = 0.1f;
    float CurrentJumpCharge;

    float DashCooldown = 1f;
    float CurrentDashCooldown;

    bool MaxCharge;
    [SerializeField] bool IsGrounded = false;
    [SerializeField] bool CanDoubleJump = false;
    [SerializeField] bool CanDash = true;
    bool IsFalling; 

    [SerializeField] TMP_Text ControlText;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.IsPaused) return;
        if (GameManager.IsGameOver) return;

        Move();

        #region//Jump Mechanics

        if (CanDoubleJump)
        {
            TotalJumpCount = 2;
        }
        else
        {
            TotalJumpCount = 1;
        }

        bool jumpInput = Input.GetKeyDown(KeyCode.Space);
        bool jumpHold = Input.GetKey(KeyCode.Space);
        bool jumpReleased = Input.GetKeyUp(KeyCode.Space);

        if (MaxCharge || (jumpReleased && transform.position.y > 0))
        {
            if (!IsFalling)
            {
                IsFalling = true;
                LeanTween.value(UpDownMovement, -FallMultiplier, 0.15f).setOnUpdate((float value) => UpDownMovement = value);
            }

        }

        if (!IsGrounded && jumpHold && !MaxCharge)
        {
            CurrentJumpCharge += Time.deltaTime;

            if (CurrentJumpCharge >= MaxJumpCharge)
            {
                CurrentJumpCharge = 0;
                MaxCharge = true;
            }
        }
        else
        {
            CurrentJumpCharge = 0;
        }

        if (jumpInput)// Space for jump
        {
            Jump();

            if (transform.position.y > 0)
            {
                ControlText.text = "Space (2x) - Double Jump";
            }
            else
            {
                ControlText.text = "Space - Jump";
            }
        }

        if (jumpReleased)
        {
            MaxCharge = false;
            IsFalling = false;
        }

        #endregion

        #region//Dash Mechanics
        if (!CanDash)
        {
            CurrentDashCooldown += Time.deltaTime;

            if (CurrentDashCooldown >= DashCooldown)
            {
                CurrentDashCooldown = 0;
                CanDash = true;
            }
        }
        #endregion
    }

    public void Knockback()
    {

    }

    public void Move()
    {
        ObjectRigidbody.linearVelocity = new Vector3(LeftRightMovement * MovementStats.MovementSpeed, UpDownMovement * MovementStats.JumpForce, 0);

        //For DEBUG
        if (Input.GetKeyDown(KeyCode.A))
        {
            ControlText.text = "A - Left";
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            ControlText.text = "D - Right";
        }

        if (Input.GetKey(KeyCode.A)) // A for left
        {
            LeftRightMovement = -1;
            
        }
        else if (Input.GetKey(KeyCode.D)) // D for right
        {
            LeftRightMovement = 1;
        }
        else
        {
            LeftRightMovement = 0;
        }

        if (CanDash && Input.GetKeyDown(KeyCode.LeftShift)) //left shift for dash
        {
            Dash();
        }
    }

    public void Jump()
    {
        if (JumpCount < TotalJumpCount)
        {
            JumpCount++;
            UpDownMovement = 1;
        }
    }

    public void Dash()
    {
        if (!LeanTween.isTweening(gameObject))
        {
            LeanTween.moveX(gameObject, (LeftRightMovement * DashValue), 0.25f).setEaseOutCubic();
        }

        if (transform.position.y > 0)
        {
            ControlText.text = "Space + LShift - Air Dash";
        }
        else
        {
            ControlText.text = "LShift - Dash";
        }

        CanDash = false;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            IsGrounded = true;
            JumpCount = 0;
            UpDownMovement = 0;
            MaxCharge = false;
            IsFalling = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            IsGrounded = false;
        }
    }
}
