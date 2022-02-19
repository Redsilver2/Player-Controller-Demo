using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.Effects;
using Player.States;
using Player.Stats;
using Player.Animations;

namespace Player
{
    [RequireComponent(typeof(PlayerAnimationsManager))]
    [RequireComponent(typeof(PlayerEffectsManager))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour, IDamagable
    {
        
        [SerializeField] public PlayerStats Stats;
        [SerializeField] public PlayerStateMachine State;
        [SerializeField] public PlayerEffectsManager Effects;
        [SerializeField] public PlayerAnimationsManager Animation;

        // Instance
        public static PlayerController instance { get; set; }


        void Awake()
        {
            if (!instance)
            {
                instance = this;
                DontDestroyOnLoad(instance);
            }
            else Destroy(gameObject);
        }

        void Start()
        {
            // Initilize Effects
            Effects = GetComponent<PlayerEffectsManager>();

            Animation.Anim = GetComponent<Animator>();

            // Stops All Particles
            Effects.StopAllParticles();

            // Initilize State
            State.Initilize(GetComponent<Rigidbody2D>(), GetComponent<BoxCollider2D>(), Effects, Animation);
        
        }

        void Update()
        {

            if (!instance || !Effects || !Animation)
            {
                return;
            }

            State.GetPlayerPosition(State.GroundRaycast);
            State.JumpCooldown();

            if (State.AxisX == 0)
            {
                Animation.IdleAnimationCoolDown();

                if (Animation.IdleAnimationCooldown <= 0f)
                {
                    Animation.PlayBaseAnimations(State.isCrouched, State.isWalking);
                    State.isWalking = false;
                    Effects.StopParticleEffect(Effects.WalkParticle);
                }
            }
            else
            {
                Animation.IdleAnimationCooldown = Animation._IdleCooldown;
            }

            if (State.JumpDelay <= 0f && Input.GetKeyDown(KeyCode.Space) && State.Jumped) 
            {
                if (!State.isDoubleJump)
                {
                    State.StateChanger(PlayerState.Jump);
                    State.Jumped = true;
                    Animation.PlayJumpAnimation(State.Jumped);
                }
                else
                {
                    State.JumpFonction();
                }
            }

            State.PlayerAction();   
        }

        public void TakeDamage(int DamageAmmount)
        {
                Stats.Health -= DamageAmmount;
                if(!Stats.CheckHealth()) { Die(); }
        }

        public void Die()
        {
            Stats.Health = 0;
            Stats.GameOver();
            State.StateChanger(PlayerState.Dead);
            State.PlayerAction();
            Effects.StopAllParticles();

            PlayerController Controller = GetComponent<PlayerController>();
            Controller.enabled = false; 
        }
    }
}
