using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.Effects;
using Player.Animations;

namespace Player.States
{
    public enum PlayerState
    {
        Walk,
        Crouched,
        Jump,
        Fall,
        Landed,
        Dead
    }

    [System.Serializable]
    public class PlayerStateMachine
    {
        // States
        public PlayerState CurrentState = PlayerState.Walk;

        // Walking 
        #region Walk Settings
        [Header("Walk Settings")]
        public float WalkSpeed = 7f;
        public float RunSpeed = 9f;

        [HideInInspector] public float AxisX;
        float MoveSpeed;
        #endregion

        //Crouch
        #region Crouch Settings
        [Header("Crouch Settings")]
        public float CrouchWalkSpeed = 7f;
        public float CrouchRunSpeed = 9f;
        public float CrouchScale = 0.67f;

        [HideInInspector] public bool isCrouched = false;
        #endregion

        // Jumping
        #region Jump Settings
        [Header("Jump Settings")]
        [Range(0f, 800f)] [SerializeField] float JumpForce = 500f;
        [SerializeField] int JumpsAllowed = 2;
        public float JumpDelay = 0.02f;

        public LayerMask JumpLayer;
        #endregion

        // Private Jumping Values
        #region Private Jumping Settings
        float ResetJumpDelay;
        bool isJump = true;
        #endregion

        // Falling
        #region Fall Settings
        [Header("Fall Settings")]
        [SerializeField] float FallForce = 0.0002f;
        [Range(-0.8f, 2f)]  public float FallDelay = 1.25f;
        #endregion

        // Private Falling Values
        #region Private Falling Settings
        // Private Fall Values
        float ResetFallDelay;
        #endregion

        #region Wall Settings

        [Header("Wall Settings")]
        [SerializeField] float WallBoostForce = 5f;
        [SerializeField]  LayerMask WallLayer;
        [SerializeField]  LayerMask WallBoost;
        #endregion

        // Optional Settings 
        #region Raycast Settings
        [Header("Raycast Settings")]
        public float Radius = 5f;
        [SerializeField] public Transform GroundRaycast;

        Transform PlayerPos;
        #endregion

        // Optional Settings 
        #region Optional Settings
        [Header("Optional Settings")]
        public bool CanJump = true;
        public bool isDoubleJump = true;
        public bool isFalling = true;
        #endregion

        // Private Components
        #region Private Components
        Rigidbody2D Rb;
        BoxCollider2D collider;

        PlayerEffectsManager Effects;
        PlayerAnimationsManager Animation;

        RaycastHit2D FloorRaycast;

        // bool to Update State On Start;
        bool UpdateStateOnStart = true;

        //Keeps Track Of Jumps
        int JumpTime = 0;

        //bool Gives Wall Boost
        bool isWallBoost = false;

        // Check If Player is on the ground
        bool isTouchWall = true;

        bool CanFlipSprite = true;

        [HideInInspector] public bool Jumped = true;

        [HideInInspector] public bool isWalking = false;
        [HideInInspector] public bool isJumping = false;

        bool isLanded = true;

        // Keep Track Of Original Y Scale Of The Player Collider
        float OriginalColliderSizeX;
        float OriginalColliderSizeY;

        SpriteRenderer PlayerRenderer;

        #endregion

        // Action
        public Action PlayerAction;




        // Initilize Everything at Start
        public void Initilize(Rigidbody2D _rb, BoxCollider2D _collider, PlayerEffectsManager _EffectManager, PlayerAnimationsManager _Animation)
        {
            // Sets State
            CurrentState = PlayerState.Walk;

            // Sets Main Components 
            Rb = _rb;
            collider = _collider;
            Effects = _EffectManager;
            Animation = _Animation;

            // Sets MoveSpeed to a Walking Speed
            MoveSpeed = WalkSpeed;

            // Sets the Reset Jump Delay 
            ResetJumpDelay = JumpDelay;

            // Sets the Reset Fall Delay 
            ResetFallDelay = FallDelay;

            // Sets Bool
            Jumped = true;
            CanJump = true;
            isCrouched = false;
            isWalking = false;
            isJumping = false;
            isLanded = false;
        

            PlayerRenderer = PlayerController.instance.GetComponent<SpriteRenderer>();

            OriginalColliderSizeX = collider.transform.localScale.x;
            OriginalColliderSizeY = collider.transform.localScale.y;

            // Sets State
            StateChanger(CurrentState);

            //Sets State first After Stops
            UpdateStateOnStart = false;
        }

        // Change State 
        public void StateChanger(PlayerState NewState)
        {
            // Returns nothing if The New State is the same as the Current One
            if (!UpdateStateOnStart)
            {
                if (NewState == CurrentState)
                {
                    return;
                }
            }

            // Updates State
            CurrentState = NewState;

            //Sets New Action depending on the state;
            switch (CurrentState)
            {
                case PlayerState.Walk:
                    PlayerAction += Move;
                    break;

                case PlayerState.Crouched:
                    PlayerAction += Crouch;
                    break;

                case PlayerState.Jump:
                    if (!CanJump) return;
                    PlayerAction += Jump;
                    break;

                case PlayerState.Fall:
                    if (!isFalling) return;
                    PlayerAction += Fall;
                    break;

                case PlayerState.Landed:
                    PlayerAction += Landed;
                    break;


                case PlayerState.Dead:
                    RemoveAllActions();
                    PlayerAction += Die;
                    break;
            }
        }

        // Player Actions
        #region Player Controls

        void Move()
        {

            AxisX = Input.GetAxisRaw("Horizontal");

            // Change Walk Value to Run Value
            if (Input.GetKey(KeyCode.LeftShift))
            {
                MoveSpeed = RunSpeed;
            }
            else
            {
                if (!isCrouched)
                {
                    MoveSpeed = WalkSpeed;
                }
                else
                {     
                     MoveSpeed = CrouchWalkSpeed;                  
                }
            }

            // Play Audio Here 

            if (Input.GetKeyDown(KeyCode.C) && CanDoStuff())
            {
                StateChanger(PlayerState.Crouched);
                isCrouched = true;
            }
           
            if(Input.GetKeyUp(KeyCode.C))
            { 
                StateChanger(PlayerState.Walk);
                Animation.PlayBaseAnimations(isCrouched, isWalking);
                isCrouched = false;
            }

            if(!isCrouched)
            {
                collider.size = new Vector2(OriginalColliderSizeX, OriginalColliderSizeY);
            }

            if (AxisX == 1 || AxisX == -1)
            {
                isWalking = true;
                PlayerController.instance.transform.localScale = new Vector3(AxisX, PlayerController.instance.transform.localScale.y, PlayerController.instance.transform.localScale.z);
                Animation.PlayBaseAnimations(isCrouched, isWalking);

                if (!Effects.WalkParticle.isPlaying && CanDoStuff())
                {
                    Effects.PlayEffect(Effects.WalkParticle,Effects.WalkParticle.main.startLifetimeMultiplier);
                }
            }
 
            // Moves Player
            Rb.velocity = new Vector2(Input.GetAxis("Horizontal") * MoveSpeed, Rb.velocity.y);
        }

        public void Crouch()
        {
            Effects.StopParticleEffect(Effects.WalkParticle);
            Animation.PlayBaseAnimations(isCrouched, isWalking);
            collider.size = new Vector2(collider.size.x, CrouchScale);
        }

        public void JumpFonction()
        {
            PlayerAction -= Crouch;
            collider.size = new Vector2(OriginalColliderSizeX, OriginalColliderSizeY);

            Effects.StopParticleEffect(Effects.WalkParticle);

            if (Jumped)
            {
                Effects.PlayEffect(Effects.JumpParticle, Effects.JumpParticlePos, Effects.JumpParticle.main.startLifetimeMultiplier, false);
                Rb.AddForce(new Vector2(0f, JumpForce));
                JumpTime++;


                if (JumpTime == JumpsAllowed || !isDoubleJump)
                {
                    isLanded = false;
                    Jumped = false;
                    Animation.PlayFallAnimation(isLanded);

                    CurrentState = PlayerState.Jump;
                    isJumping = false;
                }
                else
                {
                    CurrentState = PlayerState.Jump;
                }
            }

            Animation.PlayJumpAnimation(isJumping);
        }

        public void Jump()
        {
            isTouchingWall(Vector2.left);
            isTouchingWall(Vector2.right);


            FallCooldown();

            if (FallDelay <= 0)
            {           
                StateChanger(PlayerState.Fall);
                Effects.PlayEffect(Effects.FallParticle);
                PlayerAction -= Jump;
            }
        }
        public void Fall()
        {
            isTouchingWall(Vector2.left);
            isTouchingWall(Vector2.right);

            Effects.StopParticleEffect(Effects.WalkParticle);

            // Play Animation Here
            // Play Audio Here 

            if (!Effects.FallParticle.isPlaying)
            {
                Effects.PlayEffect(Effects.FallParticle);
            }

            Rb.velocity -= new Vector2(0f, FallForce);       

            RaycastHit2D hitInfo = Physics2D.Raycast(PlayerPos.position, -Vector2.up, Radius);

            FloorRaycast = hitInfo;

            if (hitInfo.collider.IsTouchingLayers(JumpLayer))
            {
                StateChanger(PlayerState.Landed);
                PlayerAction -= Fall;
            }
        }
        public void Landed()
        {
            // Play Audio Here 

            isLanded = true;

            Effects.StopParticleEffect(Effects.FallParticle);
            Effects.StopParticleEffect(Effects.WalkParticle);

            CanFlipSprite = false;

            PlayerRenderer.flipX = false;

            // Restrict Effect Depending On The Jump Force Value
            if (JumpForce >= Effects.JumpForceLimit) {

                // Plays Effect Depending If Its Playing Or Not
                if (!Effects.LandParticle.isPlaying)
                {
                    Effects.PlayEffect(Effects.LandParticle, Effects.LandParticlePos, Effects.LandParticle.main.startLifetimeMultiplier, true);
                }
                else
                {
                    Effects.StopParticleEffect(Effects.LandParticle);
                    Effects.PlayEffect(Effects.LandParticle, Effects.LandParticlePos, Effects.LandParticle.main.startLifetimeMultiplier, true);
                }
            }

            Animation.PlayFallAnimation(isLanded);

            // Resets Values 
            ResetValues();

            // Change States

            if (!isCrouched)
            {
                StateChanger(PlayerState.Walk);
            }
            else
            {
                StateChanger(PlayerState.Crouched);
            }

            CanFlipSprite = true;

            // Removes The Landed Action
            PlayerAction -= Landed;
        }

        #region Rest/Remove Values
        void ResetValues()
        {
            JumpDelay = ResetJumpDelay;
            FallDelay = ResetFallDelay;
            Effects.WalkInterval = Effects._WalkInterval;

            JumpTime = 0;
            CanJump = true;
            Jumped = true;
            isWallBoost = false;
        }
        void RemoveAllActions()
        {
            PlayerAction -= Move;
            PlayerAction -= Jump;
            PlayerAction -= Fall;
            PlayerAction -= Landed;
        }
        #endregion

        public void Die()
        {
            // Play Animation Here
            Debug.Log(1010);
        }

        void isTouchingWall(Vector2 Pos)
        {
            Debug.DrawRay(PlayerPos.position, Pos, Color.red);
            RaycastHit2D hitInfo = Physics2D.Raycast(PlayerPos.position, Pos);

            if (!hitInfo || hitInfo.collider.IsTouchingLayers(WallLayer) && CurrentState == PlayerState.Jump)
            {
                return;
            }

            if (hitInfo.collider.IsTouchingLayers(WallLayer))
            {
                isTouchWall = true;
                
                if (isTouchWall)
                {
                    PlayerAction -= Move;
                    PlayerRenderer.flipX = true;
                    StateChanger(PlayerState.Fall);
                }
            }
            else
            {
                isTouchWall = false;
            }
   
      
            if (hitInfo.collider.IsTouchingLayers(WallBoost))
            {
                if (!isWallBoost)
                {
                    isWallBoost = true;
                    Rb.velocity += new Vector2(0f, WallBoostForce);
                }
            }
        }

        #endregion

        #region Cooldowns
        public void JumpCooldown()
        {
            if (JumpDelay > 0f)
            {
                JumpDelay -= Time.deltaTime;
            }
        }

        public void FallCooldown()
        {
            if (FallDelay > 0f)
            {
                FallDelay -= Time.deltaTime;
            }
        }

        bool CanDoStuff()
        {
            return CurrentState != PlayerState.Jump && CurrentState != PlayerState.Fall && CurrentState != PlayerState.Landed;
        }

        #endregion

        public void GetPlayerPosition(Transform PlayerPosition)
        {
            PlayerPos = PlayerPosition;
        }
    }
}
