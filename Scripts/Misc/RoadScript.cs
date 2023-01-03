using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadScript : MonoBehaviour
{

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Chibi") && !other.GetComponent<MinionControllerScript>().isInitialized)
            EventManager.AddTarget(other.gameObject);
    }
}
