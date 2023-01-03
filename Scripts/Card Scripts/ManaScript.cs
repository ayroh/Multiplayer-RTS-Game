using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ManaScript : MonoBehaviour
{

    private static int Mana = 5;
    public static int mana { get { return Mana; } set { Mana = value; } }
    private float cooldown = 1.7f;
    [SerializeField] private TextMeshProUGUI manaText;

    private void Start() {
        if (!PhotonNetwork.OfflineMode)
            NetworkManagerScript.instance.manaScript = this;
        mana = 5;
        manaText.text = mana.ToString();
        StartCoroutine(AddMana());
    }

    private IEnumerator AddMana() {
        yield return new WaitForSeconds(cooldown);
        if(mana < 10) 
            manaText.text = (++Mana).ToString();
        StartCoroutine(AddMana());
    }

    public bool RemoveMana(int CardMana) {
        if (mana < CardMana)
            return false;
        Mana -= CardMana;
        manaText.text = mana.ToString();
        return true;
    }
}
