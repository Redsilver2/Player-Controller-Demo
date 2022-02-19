using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

namespace Player.Effects
{
    [System.Serializable]
    public class PlayerEffectsManager : MonoBehaviour
    {

        #region Walk Particle
        [Header("Jump Particle Settings")]
        public ParticleSystem WalkParticle;
        public float WalkInterval = 1.5f;
        #endregion

        #region Jump Particle
        [Header("Jump Particle Settings")]
        public ParticleSystem JumpParticle;
        public Transform JumpParticlePos;
        #endregion

        #region Fall Particle
        [Header("Fall Particle Settings")]
        public ParticleSystem FallParticle;
        public Transform FallParticlePos;
        #endregion

        #region Land Particle
        [Header("Land Particle Settings")]
        public ParticleSystem LandParticle;
        public Transform LandParticlePos;
        public float JumpForceLimit = 200f;
        #endregion

       [field:HideInInspector]
       public float _WalkInterval;

        void Start()
        {
            _WalkInterval = WalkInterval;
        }

        #region Stop Particles
        public void StopParticleEffect(ParticleSystem system)
        {
            system.Stop();
        }
       
        public void StopAllParticles()
        {
            LandParticle.Stop();
            FallParticle.Stop();
            JumpParticle.Stop();
            WalkParticle.Stop();
        }
        #endregion

        #region Play Effects
        public void PlayEffect(ParticleSystem system)
        {
            if (system == null)
            {
                return;
            }

            system.Play();
        }
        public void PlayEffect(ParticleSystem system, float time)
        {
            if (system == null)
            {
                return;
            }

            if(time > 0f)
            {
                time -= Time.deltaTime;
            }

            if (time <= 0f)
            {
                system.Play();
            }
        }
        public void PlayEffect(ParticleSystem system, Transform position,  float Time, bool ChangePosition)
        {
            if(position == null || system == null)
            {
                return;
            }

            StartCoroutine(EffectPlayer(system, position, Time, ChangePosition));
        }
        IEnumerator EffectPlayer(ParticleSystem system, Transform position, float Time, bool ChangePosition)
        {
            GameObject Object = system.gameObject;
            
            if (ChangePosition)
            {
                Object.transform.parent = null;
            }

            system.Play();

            yield return new WaitForSeconds(Time + 1f);

            if (ChangePosition)
            {
                Object.transform.parent = position;
                Object.transform.position = position.position;
            }

            system.Stop();
        }
        #endregion
    }
}
