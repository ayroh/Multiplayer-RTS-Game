using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartElementsController : MonoBehaviour
{
    [SerializeField]
    private GameObject loginB, signUpB, playB, logOutB;
    public void OnEnable()
    {
        LoginCheck();
    }
    public void LoginCheck()
    {
        if (FirebaseManager.instance.firebaseUser != null)
        {
            playB.SetActive(true);
            signUpB.SetActive(false);
            loginB.SetActive(false);
            logOutB.SetActive(true);
        }
        else
        {
            playB.SetActive(false);
            logOutB.SetActive(false);
            signUpB.SetActive(true);
            loginB.SetActive(true);
        }
    }
}
