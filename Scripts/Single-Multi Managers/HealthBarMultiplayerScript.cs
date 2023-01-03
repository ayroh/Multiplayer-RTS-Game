using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class HealthBarMultiplayerScript : MonoBehaviourPun
{

	public Slider slider;
	public Gradient gradient;
	public Image fill;

    private void Awake() {
		SetMaxHealth(200);
		SetHealth(200);
    }

    [PunRPC]
	public void SetMaxHealth(int health)
	{
		slider.maxValue = health;
		slider.value = health;

		//fill.color = gradient.Evaluate(1f);
	}

	[PunRPC]
	public void SetHealth(int health){
		slider.value = health;
		fill.color = gradient.Evaluate(slider.normalizedValue);
	}

	public void SetHealthMulti(int health) => photonView.RpcSecure("SetHealth", RpcTarget.All, false, health);
	public void SetMaxHealthMulti(int health) => photonView.RpcSecure("SetMaxHealth", RpcTarget.All, false, health);
}
