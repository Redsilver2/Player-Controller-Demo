using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemmy.States;

namespace Enemmy
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnnemyScript : MonoBehaviour
    {
        [SerializeField] EnemmyStateMachine State = new EnemmyStateMachine();

        void Start()
        {
            State.Initilize(this, GetComponent<Rigidbody2D>());         
        }

        void Update()
        {
            State.EnemmyAction();
        }

        public void Attack(Collision2D other)
        {
            if(other.collider.TryGetComponent(out IDamagable damagable))
            {
                damagable.TakeDamage(1);
            }
        }
    }
}
