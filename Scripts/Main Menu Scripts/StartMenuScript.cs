using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuScript : MonoBehaviour
{
    private void OnEnable() => EventManager.OnGameEnter += SelfSetState;

    private void OnDisable() => EventManager.OnGameEnter -= SelfSetState;

    public void SelfSetState(bool value) => this.gameObject.SetActive(false);

}
