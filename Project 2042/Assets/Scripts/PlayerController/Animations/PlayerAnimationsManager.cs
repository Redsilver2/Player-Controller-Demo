using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.States;
using Player;


namespace Player.Animations
{

    public class PlayerAnimationsManager : MonoBehaviour
    {
        public Animator Anim;

        [SerializeField] string WalkingBoolName = "isWalking";
        [SerializeField] string JumpBoolName = "isJumping";
        [SerializeField] string CrouchBoolName = "isCrouching";
        [SerializeField] string LandedBoolName = "isLanded";

        public float IdleAnimationCooldown = 5f;
        [HideInInspector] public float _IdleCooldown;

        PlayerStateMachine States;

        void Start()
        {
            _IdleCooldown = IdleAnimationCooldown;
            States = PlayerController.instance.State;
        }


        public void PlayBaseAnimations(bool isCrouching, bool isWalking)
        {
            // Walk, Crouch, Idol
            Anim.SetBool(WalkingBoolName, isWalking);
            Anim.SetBool(CrouchBoolName, isCrouching);
        }

        public void PlayJumpAnimation(bool isJumping)
        {
            Anim.SetBool(JumpBoolName, isJumping);
        }

        public void PlayFallAnimation(bool isLanded)
        {
            Anim.SetBool(LandedBoolName, isLanded);
        }

        public void JumpAnimation()
        {
            States.JumpFonction();
        }

        public void IdleAnimationCoolDown()
        {
            IdleAnimationCooldown -= Time.deltaTime;
        }
    }
}
