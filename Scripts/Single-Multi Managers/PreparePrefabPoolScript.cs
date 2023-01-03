using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class PreparePrefabPoolScript : MonoBehaviour {
    public List<GameObject> Prefabs;

    void Awake() {
        DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
        if (pool != null && this.Prefabs != null) {
            foreach (GameObject prefab in this.Prefabs) {
                if (!pool.ResourceCache.ContainsKey(prefab.name) && prefab != null)
                    pool.ResourceCache.Add(prefab.name, prefab);
                //print("prefab: " + prefab + " | name: " + prefab.name);
            }
        }
    }
}