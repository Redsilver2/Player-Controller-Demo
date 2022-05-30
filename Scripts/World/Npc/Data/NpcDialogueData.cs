using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Npc;
using Dialogue.Interface;

namespace Dialogue
{
    public enum DialogueState
    {
        Read,
        Stopped
    }

    [CreateAssetMenu(menuName = "Dialogue Menu", fileName = "New Dialogue", order = 0)]
    public class NpcDialogueData : ScriptableObject
    {
        [SerializeField] public DialogueState CurrentState = DialogueState.Stopped;
        [SerializeField] TextData [] Texts;

        [Header("Choices System")]
        [SerializeField] public string[] ChoicesText;

        public NpcDialogueData[] NextData;

        public bool isChoosing = false;

        public bool MakingChoice { get; set; }

        public TextData[] GetData { get => Texts; }

        public Action DialogueAction { get; set; }

        void Awake()
        {
            Initilize();
        }

        public void Initilize()
        {
            // Sets Current Dialogue State
            CurrentState = DialogueState.Stopped;

            // Changes State
            StateChanger(CurrentState);
        }

        public void StateChanger(DialogueState NewState)
        {
            if(CurrentState == NewState) { return; }
            CurrentState = NewState;

            switch (CurrentState)
            {
                case DialogueState.Stopped:
                    DialogueAction -= DialogueInterfaceManager.instance.ReadText;
                    break;

                case DialogueState.Read:
                    DialogueAction += DialogueInterfaceManager.instance.ReadText;
                    break;
            }
        }
        public bool IsMakingChoices() { return MakingChoice; }
    }

    [System.Serializable]
    public class TextData
    {
        [Header("Dialogue Text Setting")]
        public string NameText;
        public string MainText;

        [Header("Dialogue Audio Settings")]
        public AudioClip[] Clips;

        [Header("Dialogue Speed Settings")]
        public float TypeSpeed;

        [Header("Dialogue Option Settings")]
        public bool PlayTextAuto;
    }
}

