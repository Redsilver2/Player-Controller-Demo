using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemmy;
using Player;

namespace Enemmy.States
{
    public enum EnemmyStates
    {
         Target,
         Walk,
         Dead
    }

    [System.Serializable]
    public class EnemmyStateMachine
    {
        [SerializeField] public EnemmyStates CurrentState = EnemmyStates.Walk;

        public LayerMask Ground;

        [SerializeField] public float WalkingSpeed = 5f;
        [SerializeField] public float ForceDown = 5f;
        [SerializeField] public float WaitingTime = 4f;
        [SerializeField] public float SpotRange = 4f;
        [SerializeField] List<Transform> Waypoints;
        [SerializeField] bool RandomPath = false;




        // Keeps Track Of Current Traget
        int Index = 0;


        float MoveSpeed;
        float WaitTime;
       
        float DistanceToTarget;
        float DistanceToPlayer;

        bool UpdateStateOnStart = true;

        public Action EnemmyAction;

        EnnemyScript Enemmy;
        Rigidbody2D rb;


        public void Initilize(EnnemyScript _Enemmy, Rigidbody2D _rb)
        {
            Enemmy = _Enemmy;
            rb = _rb;

            WaitTime = WaitingTime;
            MoveSpeed = WalkingSpeed;

            StateChanger(CurrentState);
            UpdateStateOnStart = false;
        }

        public void StateChanger(EnemmyStates NewState)
        {
            if (!UpdateStateOnStart)
            {
                if (CurrentState == NewState)
                {
                    return;
                }
            }

            CurrentState = NewState;

            switch (CurrentState)
            {
                case EnemmyStates.Walk:
                    EnemmyAction += Move;
                    break;

                case EnemmyStates.Target:
                    EnemmyAction += MoveToTarget;
                    break;

                case EnemmyStates.Dead:
                    RemoveAllActions();
                    break;
            }
        }

        public void Move()
        {

            PlayerDead();

            DistanceToTarget = Vector2.Distance(Enemmy.transform.position, Waypoints[Index].position);
            DistanceToPlayer = Vector2.Distance(Enemmy.transform.position, PlayerController.instance.transform.position);

            RaycastGround();

            if(DistanceToPlayer <= SpotRange)
            {
                StateChanger(EnemmyStates.Target);
                EnemmyAction -= Move;
            }

            if (DistanceToTarget >= Mathf.Epsilon + 1)
            {
                if (CurrentState == EnemmyStates.Target || CurrentState == EnemmyStates.Dead)
                {
                    MoveToWaypoint();
                }
            }
            else
            {
               
                if (WaitingTime <= 0)
                {
                    if (!RandomPath)
                    {
                        if (Index != Waypoints.Count - 1)
                        {
                            Index++;
                            Debug.Log("Arrived at destination");
                        }
                        else
                        {
                            Index = 0;
                            Debug.Log("Reset Destination");
                        }
                    }
                    else
                    {
                        Index = UnityEngine.Random.Range(0, Waypoints.Count);
                    }

                    WaitingTime = WaitTime;
                }
                else
                {
                    WaitingTime -= Time.deltaTime;
                }
            }
        }
        void MoveToWaypoint()
        {
            Enemmy.transform.position = Vector2.MoveTowards(Enemmy.transform.position, Waypoints[Index].position, MoveSpeed * Time.deltaTime);
        }

        void MoveToTarget()
        {
            PlayerDead();

            if(DistanceToPlayer > SpotRange)
            {
                StateChanger(EnemmyStates.Walk);
                EnemmyAction -= MoveToTarget;
            }
            else
            {
                Debug.Log(131313);
                Enemmy.transform.position = Vector2.MoveTowards(Enemmy.transform.position,PlayerController.instance.transform.position, MoveSpeed * Time.deltaTime);
            }
        }


        void RaycastGround()
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(rb.gameObject.transform.position, Vector2.down);

            if (hitInfo.collider.IsTouchingLayers(Ground))
            {
                return;
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -ForceDown);
            }
        }

        void PlayerDead()
        {
            if(PlayerController.instance.State.CurrentState == Player.States.PlayerState.Dead)
            {
                RemoveAllActions();
                return;
            }
        }

        void RemoveAllActions()
        {
            EnemmyAction -= Move;
            EnemmyAction -= MoveToTarget;
        }
    }
}
