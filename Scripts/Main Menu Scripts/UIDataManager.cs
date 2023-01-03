using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIDataManager : MonoBehaviour
{
    [SerializeField] private UserSO userSO;
    [SerializeField] private GameObject NetworkManager;
    [SerializeField] private GameObject OnlinePanel;


    private void OnEnable() {
        EventManager.OnCannonStatueAction += CannonSelection;
        SceneManager.sceneLoaded += OnSceneLoaded;
        //NetworkManager = NetworkManagerScript.instance.gameObject;
        //NetworkManagerScript.instance.OnlinePanel = OnlinePanel;
    }

    private void OnDisable() {
        EventManager.OnCannonStatueAction -= CannonSelection;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if(scene.name == "MenuScene") {
            NetworkManagerScript.instance.SetOnlinePanel(OnlinePanel);
            //if (NetworkManagerScript.instance != null)
            //    Destroy(NetworkManager);
            //else
            //    NetworkManagerScript.instance.gameObject.SetActive(true);
        }
    }

    void CannonSelection(int cannonID) => userSO.CurrentCannon = cannonID;
}
