using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    private void OnEnable() {
        EventManager.OnHomePage += ChangeToMenuScene;
    }

    private void OnDisable() {
        EventManager.OnHomePage -= ChangeToMenuScene;
    }

    public void Play()
    {
        SceneManager.LoadScene(2);
    }

    public void LevelEndWinOrLose()
    {
        EventManager.LevelEndTriggered();
    }

    public void EnterMainMenu(bool unlocked) {
        EventManager.StartMainMenu(unlocked);
    }

    public void OpenMenu(int i) {
        EventManager.SelectMenu(i);
    }

    public void ChangeToModifyScreen() {
        SceneManager.LoadScene(3);
    }
    public void ChangeToMenuScene() {
        SceneManager.LoadScene(1);
    }
    public void ChangeToLoginScene()
    {
        SceneManager.LoadScene(0);
    }

    public void LeaveOnlineGame() => NetworkManagerScript.instance.LeaveGame();

    public void UnrankedPVPPlay() {
        PhotonNetwork.OfflineMode = false;
        NetworkManagerScript.instance.UnrankedPVPStart(); 
    }
}
