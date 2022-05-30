using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Dialogue;
using Npc;
using Player;
using Player.Interface;

namespace Dialogue.Interface
{
    public class DialogueInterfaceManager : MonoBehaviour
    {
        [Header("BG Settings")]
        [SerializeField] GameObject DialogueBG;

        [Header("Display Settings")]
        [SerializeField] TextMeshProUGUI NameDisplay;
        [SerializeField] Text MainTextDisplay;

        [Header("Buttons Settings")]
        [SerializeField] GameObject NextTextButton;
        [SerializeField] GameObject[] ChoicesButton;

        int Index = 0;

        bool UpdateOnStart = true;

        [HideInInspector] public List<Text> ChoicesButtonText;

        NpcDialogueData Data;

        public bool isChoosing { get; set; }

        public AudioSource Audio { get; set; }

        public NpcScript Npc { get; set; }

        public static DialogueInterfaceManager instance { get; set; }

        void Awake()
        {
            if(instance == null)
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
            DisableAllUI(false);
            ResetTextData(true);

            UpdateOnStart = false;

            for(int I = 0; I <= ChoicesButton.Length - 1; I++)
            {
                ChoicesButtonText.Add(ChoicesButton[I].GetComponentInChildren<Text>());
            }
        }

        public void Initilize(NpcDialogueData _Data)
        {
            Data = _Data;
            Initilize();
        }

        public void Initilize()
        {
            isChoosing = false;
            Data.StateChanger(DialogueState.Read);
            DisableAllUI(true);
            DisableAllChoicesButton(false, false);
            StartText();
        }

        void StartText()
        {
            StartCoroutine(ShowText());
        }

        public void ReadText()
        {

            if (MainTextDisplay.text == Data.GetData[Index].MainText && Data.GetData[Index].PlayTextAuto == true)
            {
                TextLogic();
            }

            if (Input.GetKeyDown(KeyCode.E) && !Npc.MainData.IsMakingChoices() && MainTextDisplay.text == Data.GetData[Index].MainText && Data.GetData[Index].PlayTextAuto == false)
            { 
                TextLogic();
            }
        }

        public void TextLogic()
        {         
                if (Index < Data.GetData.Length - 1)
                {       
                       Cursor.visible = false;
                       Cursor.lockState = CursorLockMode.Locked;
                       NextTextButton.SetActive(false);

                       Index++;
                       ResetText();
                       StartText();            
                }
                else
                {              
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;

                    if (!Data.isChoosing)
                    {
                           NextTextButton.SetActive(false);
                    }

                    DisableAllUI(false);
                    ResetTextData(true);     
                }
        }

        IEnumerator ShowText()
        {
            foreach (char C in Data.GetData[Index].MainText.ToCharArray())
            {
                MainTextDisplay.text += C;

                if (!Audio.isPlaying)
                {
                    Audio.clip = Data.GetData[Index].Clips[Random.Range(0, Data.GetData[Index].Clips.Length)];
                    Audio.Play();                 
                }

                yield return new WaitForSeconds(Data.GetData[Index].TypeSpeed);

                if (MainTextDisplay.text == Data.GetData[Index].MainText)
                {
                    Audio.loop = false;

                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;

                    NextTextButton.SetActive(true);

                    if(Index == Data.GetData.Length - 1)
                    {
                        if (Data.isChoosing)
                        {
                            NextTextButton.SetActive(false);
                            DisableAllChoicesButton(true, true);
                            isChoosing = true;
                        }
                    }
                }
            }
        }

        void DisableAllChoicesButton(bool isTrue, bool UpdateText)
        {
            for (int I = 0; I <= Data.ChoicesText.Length - 1; I++)
            {
                ChoicesButton[I].SetActive(isTrue);
              
                if (UpdateText)
                {
                    ChoicesButtonText[I].text = Data.ChoicesText[I];
                }
            }
        }

        public void Choices(int Index)
        {
            Data.StateChanger(DialogueState.Stopped);
            ResetTextData(false);
            DisableAllChoicesButton(false, false);
            Npc.MainData = Data.NextData[Index];
            Initilize(Npc.MainData);
        }

        void DisableAllUI(bool istrue)
        {
            DialogueBG.SetActive(istrue);

            if (UpdateOnStart)
            {
                NextTextButton.SetActive(istrue);

                foreach (GameObject G in ChoicesButton)
                {
                    G.SetActive(false);
                }
            }

            MainTextDisplay.transform.gameObject.SetActive(istrue);
            NameDisplay.transform.gameObject.SetActive(istrue);
        }

        void ResetTextData(bool enablePlayer)
        {
          

            if (Data != null)
            {
                Data.StateChanger(DialogueState.Stopped);

                if (!Data.isChoosing)
                {
                    // Changes Data
                    Npc.MainData = Npc.AfterData;
                }
            }

            if (enablePlayer)
            {
                PlayerInterfaceManager.instance.DisableAllUI(true);
                PlayerController.instance.enabled = true;
            }
      
            // Resets Index
            Index = 0;

            // Resets Text
            ResetText();

            isChoosing = false;
        }

        void ResetText()
        {
            MainTextDisplay.text = "";
            NameDisplay.text = "";
        }
    }
}
