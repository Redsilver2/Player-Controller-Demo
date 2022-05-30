using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using Player;
using Player.Interface;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
public class BonFireScript : MonoBehaviour
{
    public VideoPlayer Video;
    public RawImage image;

    public GameObject AbilitiesUI;
 
   static int UpgradeCostMaxHealth = 30;
   static int UpgradeCostMaxEndurance = 50;

    bool isBonFireLit = false;
    bool isPlayerInside = false;
    bool PlayAnimation = true;
    bool isInteracting = false;
   
    Animator Anim;

    void Start()
    {
        Anim = GetComponent<Animator>();
        AbilitiesUI.SetActive(false);
    }

    void Update()
    {
        if(!isPlayerInside) { return; }

        if (Input.GetKeyDown(KeyCode.E) && isPlayerInside && !isBonFireLit)
        {
            isBonFireLit = true;
            PlayFireAnimation(isBonFireLit);
        }

        if (!isBonFireLit) { return; }

        if (Input.GetKeyDown(KeyCode.E) && isPlayerInside && isBonFireLit && !PlayAnimation && !isInteracting && PlayerController.instance.State.AxisX == 0)
        {
            Debug.Log(100);
            ResetPlayerStats();
            PlayerController.instance.State.Rb.velocity = new Vector3(0, 0, 0);
            EnablePlayer(false);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isPlayerInside = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isPlayerInside = false;
        }
    }

    void PlayFireAnimation(bool istrue)
    {
        Anim.SetBool("SetFire", istrue); 
    }
    public void PlayVideo()
    {
        Color currColor = image.color;

        if (!PlayAnimation) {

            if (!Video.isPlaying)
            {
                currColor.a = 0f;
                image.color = currColor;
            }

            return;
        }

        currColor.a = 1f;
        image.color = currColor;

        Video.Play();

        PlayAnimation = false;
    }


    public void UpgradeHealth(int UpgradeAmmount)
    {
        if(PlayerController.instance.Stats.MoneyAmmount >= UpgradeCostMaxHealth)
        {
            PlayerController.instance.Stats.MoneyAmmount -= UpgradeCostMaxHealth;
            PlayerController.instance.Stats.MaxHealth += UpgradeAmmount;

            UpgradeCostMaxHealth += 50;
        }
        else
        {
            return;
        }
    }

    public void UpgradeEndurance(int UpgradeAmmount)
    {
        if (PlayerController.instance.Stats.MoneyAmmount >= UpgradeCostMaxEndurance)
        {
            PlayerController.instance.Stats.MoneyAmmount -= UpgradeCostMaxEndurance;
            PlayerController.instance.Stats.MaxEndurance += UpgradeAmmount;
    
            UpgradeCostMaxEndurance += 50;
        }
        else
        {
            return;
        }

    }

    public void ResetPlayerStats()
    {        
        PlayerController.instance.Stats.Health = PlayerController.instance.Stats.MaxHealth;
        PlayerController.instance.Stats.Endurance = PlayerController.instance.Stats.Endurance;
    }

    public void EnablePlayer(bool isTrue)
    {
        ResetPlayerStats();
        PlayerInterfaceManager.instance.ResetValues();

        PlayerController.instance.enabled = isTrue;
        PlayerInterfaceManager.instance.DisableAllUI(isTrue);

        if(isTrue == false)
        {
            isInteracting = true;
            AbilitiesUI.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            isInteracting = false;
            AbilitiesUI.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
