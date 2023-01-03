using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SpawnerMultiplayerScript : MonoBehaviour {
    [SerializeField] private Transform blueBridgeSpawnTransform;
    [SerializeField] private Transform redBridgeSpawnTransform;
    [SerializeField] private Transform blueBridgeSpreadTransform;
    [SerializeField] private Transform redBridgeSpreadTransform;
    [SerializeField] private Transform navMeshMap;
    [SerializeField] private GameObject defaultChibiPrefab;
    private ObjectPool<GameObject> pool;
    private int poolSize = 12;
    private float waveInterval = 7f;
    private int waveSize = 7;
    private float inBetween = .5f;
    private bool isBlueBridge = true;
    public enum ChibiType { Default = 0 };

    private void Awake() {
        if (!PhotonNetwork.IsMasterClient)
            return;
        NetworkManagerScript.instance.spawnerScript = this;
        NetworkManagerScript.instance.ReleaseMinion = ReleaseMinion;
        pool = new ObjectPool<GameObject>(Create, ActionOnGet, ActionOnRelease, null, false, poolSize, poolSize);
    }

    #region Pooling

   GameObject[] tempRelease;
    public void StartSpawning() => StartCoroutine(ReleaseWait());

    public IEnumerator ReleaseWait() {
        yield return new WaitForSeconds(1f);
        tempRelease = new GameObject[poolSize];
        for (int i = 0; i < poolSize; ++i) {
            tempRelease[i] = pool.Get();
            tempRelease[i].GetComponent<MinionControllerScript>().PoolReleaseMulti();
        }

        yield return new WaitForSeconds(3f);
        for (int i = 0; i < poolSize; ++i)
            ReleaseMinion(tempRelease[i]);
        StartCoroutine(poolCoroutine());
    }

    public IEnumerator poolCoroutine() {
        for (int i = 0; i < waveSize; ++i) {
            yield return new WaitForSeconds(inBetween);
            if (pool.CountActive == poolSize)
                continue;
            pool.Get();
        }
        yield return new WaitForSeconds(waveInterval);

        StartCoroutine(poolCoroutine());
    }

    public GameObject Create() => PhotonNetwork.InstantiateRoomObject(defaultChibiPrefab.name, new Vector3(blueBridgeSpawnTransform.position.x + 5f, 0f, blueBridgeSpawnTransform.position.z), Quaternion.identity);

    private void ActionOnGet(GameObject obj) {
        Vector3 newPos;
        if (isBlueBridge)
            newPos = new Vector3(Random.Range(blueBridgeSpawnTransform.position.x - 2f, blueBridgeSpawnTransform.position.x + 2f), blueBridgeSpawnTransform.position.y, Random.Range(blueBridgeSpawnTransform.position.z - 2f, blueBridgeSpawnTransform.position.z + 2f));
        else
            newPos = new Vector3(Random.Range(redBridgeSpawnTransform.position.x - 2f, redBridgeSpawnTransform.position.x + 2f), redBridgeSpawnTransform.position.y, Random.Range(redBridgeSpawnTransform.position.z - 2f, redBridgeSpawnTransform.position.z + 2f));
        obj.GetComponent<MinionControllerScript>().PoolResetMulti(newPos);
        //obj.SetActive(true);
        //obj.GetComponent<MinionControllerScript>().SetPositionMulti();
        //obj.GetComponent<MinionControllerScript>().ResetMinionMulti();
        isBlueBridge = !isBlueBridge;
    }

    private void ActionOnRelease(GameObject obj) {
        obj.GetComponent<MinionControllerScript>().PoolReleaseMulti();
    }

    private void ReleaseMinion(GameObject minion) => pool.Release(minion);

    #endregion

    #region Without Pooling

    //private IEnumerator spawnChibiCoroutine(ChibiType type, bool isBlueBridge, int number = 1, float inBetween = 0f) {
    //    for (int i = 0; i < number; ++i) {
    //        spawnChibi(type, isBlueBridge);
    //        yield return new WaitForSeconds(inBetween);
    //    }
    //    yield return new WaitForSeconds(15);
    //    StartCoroutine(spawnChibiCoroutine(ChibiType.Default, isBlueBridge, number, inBetween));
    //}

    ///*public void SetCastles(Transform blueCastle, Transform redCastle) {
    //    blueCastleTransform = blueCastle;
    //    redCastleTransform = redCastle;
    //}*/
    //private void spawnChibi(ChibiType type, bool isBlueBridge, int number = 1) {
    //    switch (type) {
    //        case ChibiType.Default:
    //            for (int i = 0; i < number; i++) {
    //                if (isBlueBridge) {
    //                    PhotonNetwork.Instantiate(defaultChibiPrefab.name, new Vector3(Random.Range(blueBridgeSpawnTransform.position.x - 2f, blueBridgeSpawnTransform.position.x + 2f), blueBridgeSpawnTransform.position.y, Random.Range(blueBridgeSpawnTransform.position.z - 2f, blueBridgeSpawnTransform.position.z + 2f)), Quaternion.identity);
    //                    //PatrolController ChibiPatrolController = Chibi.GetComponent<PatrolController>();
    //                    //ChibiPatrolController.SetNavMeshMapCollider(navMeshMap.gameObject.GetComponent<BoxCollider>());
    //                    //ChibiPatrolController.SetCastleTransforms(blueCastleTransform, redCastleTransform);
    //                    //ChibiPatrolController.SendToPoint(blueBridgeSpreadTransform.position);
    //                }
    //                else {
    //                    PhotonNetwork.Instantiate(defaultChibiPrefab.name, new Vector3(Random.Range(redBridgeSpawnTransform.position.x - 2f, redBridgeSpawnTransform.position.x + 2f), redBridgeSpawnTransform.position.y, Random.Range(redBridgeSpawnTransform.position.z - 2f, redBridgeSpawnTransform.position.z + 2f)), Quaternion.identity);
    //                    //PatrolController ChibiPatrolController = Chibi.GetComponent<PatrolController>();
    //                    //ChibiPatrolController.SetNavMeshMapCollider(navMeshMap.gameObject.GetComponent<BoxCollider>());
    //                    //ChibiPatrolController.SetCastleTransforms(blueCastleTransform, redCastleTransform);
    //                    //ChibiPatrolController.SendToPoint(redBridgeSpreadTransform.position);
    //                }

    //            }
    //            break;
    //    }
    //}

    #endregion
}
