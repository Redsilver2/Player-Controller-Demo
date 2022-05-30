using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Player;
using Player.States;
using Player.Stats;

namespace Player.Interface
{
    public class PlayerInterfaceManager : MonoBehaviour
    {
        [SerializeField] Image HealthFill;
        [SerializeField] GameObject _HealthFillGameObject;

        [SerializeField] Image JumpFill;
        [SerializeField] GameObject _JumpFillGameObject;

        float JumpFillValue;
        float HealthFillValue;

        PlayerStats Stats;
        PlayerStateMachine State;

        public static PlayerInterfaceManager instance { get; set; }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(instance);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            Initilize();
            Stats.Initilize();
            JumpFillValue = State.JumpDelay;
            ResetValues();
        }

        void Update()
        {
            if (PlayerController.instance.enabled)
            {
                UpdateJumpFillAmmount();
            }
            else
            {
                return;
            }
        }

        void Initilize()
        {
            State = PlayerController.instance.State;
            Stats = PlayerController.instance.Stats;
        }
        void UpdateJumpFillAmmount()
        {
            JumpFill.fillAmount = State.JumpDelay / JumpFillValue;
        }

        void UpdateEnduranceFillAmmount()
        {
                      
        }

        public void UpdateHealthFillAmmount()
        {
            HealthFill.fillAmount = Stats.Health / HealthFillValue;
        }

        public void ResetValues()
        {
            HealthFillValue = Stats.MaxHealth;
            UpdateHealthFillAmmount();
        }

        public void DisableUI(GameObject UI, bool isTrue)
        {
            UI.SetActive(isTrue);
        }

        public void DisableAllUI(bool isTrue)
        {
            _JumpFillGameObject.SetActive(isTrue);
            _HealthFillGameObject.SetActive(isTrue);
        }
    }
}
