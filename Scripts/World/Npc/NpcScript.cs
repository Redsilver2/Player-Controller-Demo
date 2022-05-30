using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Player.Interface;
using Dialogue;
using Dialogue.Interface;

namespace Npc {

    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class NpcScript : MonoBehaviour
    {
        [SerializeField] public NpcDialogueData Data;
        [SerializeField] public NpcDialogueData AfterData;

        [HideInInspector] public  NpcDialogueData MainData;
        [HideInInspector] public bool istalking = false;
        [HideInInspector] public AudioSource Audio;
       
        void Start()
        {
            Audio = GetComponent<AudioSource>();

            if (Data != null)
            {
                MainData = Data;
                Data.Initilize();
            }
        }

        void Update()
        {
            MainData.MakingChoice = DialogueInterfaceManager.instance.isChoosing;

            if (Input.GetKeyDown(KeyCode.E) && Data.CurrentState == DialogueState.Stopped && istalking)
            {
              
                PlayerInterfaceManager.instance.DisableAllUI(false);
                PlayerController.instance.State.Rb.velocity = new Vector3(0, 0, 0);
                PlayerController.instance.enabled = false;
                DialogueInterfaceManager.instance.Initilize(MainData);
                istalking = false;
            }

            if (Data.CurrentState != DialogueState.Read) { return; }
            
            Data.DialogueAction();
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                FlipSprite();
            }

            DialogueInterfaceManager.instance.Audio = Audio;
            DialogueInterfaceManager.instance.Npc = this;
        }

        void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {      
                if(PlayerController.instance.State.AxisX == 0 && MainData.CurrentState == DialogueState.Stopped)
                {
                    istalking = true;
                }
               
                if(MainData.CurrentState == DialogueState.Read)
                {
                    istalking = false;
                }                
            }
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                FlipSprite();
            }

           DialogueInterfaceManager.instance.Audio = null;
           DialogueInterfaceManager.instance.Npc = null;
        }

        void FlipSprite()
        {
            transform.localScale = new Vector3(-PlayerController.instance.transform.localScale.x, 1f, 1f);
        }
    }
}
