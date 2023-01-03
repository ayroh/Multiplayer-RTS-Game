using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class CastleScript : MonoBehaviourPun {

    [SerializeField] private UserSO userSO;
    private int maxHealth = 200;
    [SerializeField] private HealthBarMultiplayerScript healthBar;
    [SerializeField] private GameObject Cannon;
    [SerializeField] private GameObject blueBall;
    [SerializeField] private GameObject redBall;
    [NonSerialized] public int currentHealth;
    private bool isOnline;
    private SphereCollider col;

    // Skills
    private bool isBlue;
    private bool isBot;

    private void Awake() {
        isOnline = !PhotonNetwork.OfflineMode;
        currentHealth = maxHealth;
        col = GetComponent<SphereCollider>();
    }
    public SphereCollider GetCollider() => col;

    public void Init(bool IsBlue, bool IsBot = false, HealthBarMultiplayerScript HB = null) {
        isOnline = !PhotonNetwork.OfflineMode;
        if (HB != null) {
            healthBar = HB;
            healthBar.SetMaxHealth(maxHealth);
        }
        isBot = IsBot;
        isBlue = IsBlue;
        currentHealth = maxHealth;
        EnableCannon();

        if (!isBot) {
            switch (userSO.CurrentCannon) {
                case 0:
                    if (isBlue)
                        Cannon.GetComponent<CannonTowerScript>().Init(blueBall);
                    else
                        Cannon.GetComponent<CannonTowerScript>().Init(redBall);
                    break;
                case 1:
                    if (isBlue)
                        Cannon.GetComponent<RifleTowerScript>().Init(blueBall);
                    else
                        Cannon.GetComponent<RifleTowerScript>().Init(redBall);
                    break;
                case 2:
                    if (isBlue)
                        Cannon.GetComponent<ElectricTowerScript>().Init(blueBall);
                    else
                        Cannon.GetComponent<ElectricTowerScript>().Init(redBall);
                    break;
            }
        }

        if (isOnline) {
            if (isBlue) 
                MultiplayerMapDataScript.instance.blueCastle = this;
            else 
                MultiplayerMapDataScript.instance.redCastle = this;
            
        }
        else {
            if(TutorialManager.instance != null) {
                if (isBot)
                    TutorialManager.redCastle = this;
                else
                    TutorialManager.blueCastle = this;
            }
            else {
                if (isBot)
                    GameManager.instance.redCastle = this;
                else
                    GameManager.instance.blueCastle = this;
            }

        }
    }

    public Vector3 GetShootPoint() => Cannon.transform.Find("ShootPoint").position;

    public void SetHealthBar(HealthBarMultiplayerScript HB) => healthBar = HB;


    #region Enable/Disable

    public void EnableFire() {
        if (!photonView.IsMine && isOnline)
            return;
        if (isOnline) {
            switch (userSO.CurrentCannon) {
                case 0:
                    Cannon.GetComponent<CannonTowerScript>().CanFire();
                    break;
                case 1:
                    Cannon.GetComponent<RifleTowerScript>().CanFire();
                    break;
                case 2:
                    Cannon.GetComponent<ElectricTowerScript>().CanFire();
                    break;
            }
        }
        else {
            if (isBot) {
                switch (userSO.CurrentCannon) {
                    case 0:
                        Cannon.GetComponent<CannonBotScript>().EnableFire();
                        break;
                    case 1:
                        Cannon.GetComponent<RifleBotScript>().EnableFire();
                        break;
                    case 2:
                        Cannon.GetComponent<ElectricBotScript>().EnableFire();
                        break;
                }
            }
            else {
                switch (userSO.CurrentCannon) {
                    case 0:
                        Cannon.GetComponent<CannonTowerScript>().CanFire();
                        break;
                    case 1:
                        Cannon.GetComponent<RifleTowerScript>().CanFire();
                        break;
                    case 2:
                        Cannon.GetComponent<ElectricTowerScript>().CanFire();
                        break;
                }
            }
        }
    }
    public void EnableCannon() {
        if (isOnline) {
            switch (userSO.CurrentCannon) {
                case 0:
                    Cannon.GetComponent<CannonTowerScript>().enabled = true;
                    break;
                case 1:
                    Cannon.GetComponent<RifleTowerScript>().enabled = true;
                    break;
                case 2:
                    Cannon.GetComponent<ElectricTowerScript>().enabled = true;
                    break;
            }
        }
        else {
            if (isBot) {
                switch (userSO.CurrentCannon) {
                    case 0:
                        Cannon.GetComponent<CannonBotScript>().enabled = true;
                        break;
                    case 1:
                        Cannon.GetComponent<RifleBotScript>().enabled = true;
                        break;
                    case 2:
                        Cannon.GetComponent<ElectricBotScript>().enabled = true;
                        break;
                }
            }
            else {
                switch (userSO.CurrentCannon) {
                    case 0:
                        Cannon.GetComponent<CannonTowerScript>().enabled = true;
                        break;
                    case 1:
                        Cannon.GetComponent<RifleTowerScript>().enabled = true;
                        break;
                    case 2:
                        Cannon.GetComponent<ElectricTowerScript>().enabled = true;
                        break;
                }
            }
        }
    }


    public void DisableCannon() {
        if (isOnline) {
            switch (userSO.CurrentCannon) {
                case 0:
                    Cannon.GetComponent<CannonTowerScript>().enabled = false;
                    break;
                case 1:
                    Cannon.GetComponent<RifleTowerScript>().enabled = false;
                    break;
                case 2:
                    Cannon.GetComponent<ElectricTowerScript>().enabled = false;
                    break;
            }
        }
        else {
            if (isBot) {
                switch (userSO.CurrentCannon) {
                    case 0:
                        Cannon.GetComponent<CannonBotScript>().enabled = false;
                        break;
                    case 1:
                        Cannon.GetComponent<RifleBotScript>().enabled = false;
                        break;
                    case 2:
                        Cannon.GetComponent<ElectricBotScript>().enabled = false;
                        break;
                }
            }
            else {
                switch (userSO.CurrentCannon) {
                    case 0:
                        Cannon.GetComponent<CannonTowerScript>().enabled = false;
                        break;
                    case 1:
                        Cannon.GetComponent<RifleTowerScript>().enabled = false;
                        break;
                    case 2:
                        Cannon.GetComponent<ElectricTowerScript>().enabled = false;
                        break;
                }
            }
        }
    }

    #endregion

    #region Castle Hit
    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if(isOnline)
            healthBar.SetHealthMulti(currentHealth);
        else
            healthBar.SetHealth(currentHealth);
    }
    public void OnCastleHit(int damage) {
        TakeDamage(damage);
        if (currentHealth < 1) {
            // if bot loses send true(isBot), if player loses send false(isBot)
            if (isOnline)
                NetworkManagerScript.instance.EndGame();
                //PhotonNetwork.RaiseEvent(MultiplayerMapDataScript.END_GAME_EVENT, true, RaiseEventOptions.Default, SendOptions.SendReliable); // sends to only 1 player, can use classic events instead
            else
                EventManager.LevelStatueAction(isBot);
        }
    }
    #endregion


    public void SetCannonRotation(Vector3 rot) => Cannon.transform.rotation = Quaternion.LookRotation(rot);

}
