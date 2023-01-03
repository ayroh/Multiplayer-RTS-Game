using System.Collections;
using UnityEngine.Pool;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnerScript : MonoBehaviour
{
    [SerializeField] private Transform blueBridgeSpawnTransform;
    [SerializeField] private Transform redBridgeSpawnTransform;
    [SerializeField] private Transform blueBridgeSpreadTransform;
    [SerializeField] private Transform redBridgeSpreadTransform;
    [SerializeField] private GameObject defaultChibiPrefab;
    private int poolSize = 12;
    private float waveInterval = 7f;
    private int waveSize = 7;
    private float inBetween = .5f;
    private ObjectPool<GameObject> pool;
    private bool isBlueBridge = true;
    public enum ChibiType { Default = 0 };

    void Start(){
        GameManager.instance.ReleaseMinion = ReleaseMinion;
        pool = new ObjectPool<GameObject>(Create, ActionOnGet, ActionOnRelease, null, true, poolSize, poolSize);
        GameObject[] tempRelease = new GameObject[poolSize];
        for(int i = 0;i < poolSize; ++i)
            tempRelease[i] = pool.Get();
        for (int i = 0; i < poolSize; ++i)
            ReleaseMinion(tempRelease[i]);
        StartCoroutine(poolCoroutine());

        //StartCoroutine(spawnChibiCoroutine(ChibiType.Default, true, 10, 0.3f));
        //StartCoroutine(spawnChibiCoroutine(ChibiType.Default, false, 10, 0.3f));
    }

    #region Pooling


    public IEnumerator poolCoroutine() {
        for(int i = 0;i < waveSize; ++i) {
            yield return new WaitForSeconds(inBetween);
            if (pool.CountActive == poolSize)
                continue;
            pool.Get();
        }
        yield return new WaitForSeconds(waveInterval);

        StartCoroutine(poolCoroutine());
    }

    public GameObject Create() => Instantiate(defaultChibiPrefab);

    private void ActionOnGet(GameObject obj) {
        if (isBlueBridge)
            obj.transform.position = new Vector3(Random.Range(blueBridgeSpawnTransform.position.x - 2f, blueBridgeSpawnTransform.position.x + 2f), blueBridgeSpawnTransform.position.y, Random.Range(blueBridgeSpawnTransform.position.z - 2f, blueBridgeSpawnTransform.position.z + 2f));
        else
            obj.transform.position = new Vector3(Random.Range(redBridgeSpawnTransform.position.x - 2f, redBridgeSpawnTransform.position.x + 2f), redBridgeSpawnTransform.position.y, Random.Range(redBridgeSpawnTransform.position.z - 2f, redBridgeSpawnTransform.position.z + 2f));
        obj.SetActive(true);
        obj.GetComponent<MinionControllerScript>().ResetMinion();
        isBlueBridge = !isBlueBridge;
    }

    private void ActionOnRelease(GameObject obj) {
        obj.SetActive(false);
    }

    private void ReleaseMinion(GameObject minion) => pool.Release(minion);

    #endregion


    #region Without Pooling

    //private IEnumerator spawnChibiCoroutine(ChibiType type, bool isBlueBridge, int number = 1, float inBetween = 0f){
    //    for (int i = 0;i < number;++i){
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

    //private void spawnChibi(ChibiType type, bool isBlueBridge, int number = 1){
    //    switch (type){
    //        case ChibiType.Default:
    //            for(int i = 0;i < number; i++){
    //                if (isBlueBridge){
    //                    GameObject Chibi = Instantiate(defaultChibiPrefab, new Vector3(Random.Range(blueBridgeSpawnTransform.position.x - 2f, blueBridgeSpawnTransform.position.x + 2f), blueBridgeSpawnTransform.position.y, Random.Range(blueBridgeSpawnTransform.position.z - 2f, blueBridgeSpawnTransform.position.z + 2f)), Quaternion.identity);
    //                    MinionControllerScript ChibiPatrolController = Chibi.GetComponent<MinionControllerScript>();
    //                    //ChibiPatrolController.SetNavMeshMapCollider(navMeshMap.gameObject.GetComponent<BoxCollider>());
    //                    //ChibiPatrolController.SetCastleTransforms(blueCastleTransform, redCastleTransform);
    //                    //ChibiPatrolController.SendToPoint(blueBridgeSpreadTransform.position);
    //                }
    //                else{
    //                    GameObject Chibi = Instantiate(defaultChibiPrefab, new Vector3(Random.Range(redBridgeSpawnTransform.position.x - 2f, redBridgeSpawnTransform.position.x + 2f), redBridgeSpawnTransform.position.y, Random.Range(redBridgeSpawnTransform.position.z - 2f, redBridgeSpawnTransform.position.z + 2f)), Quaternion.identity);
    //                    MinionControllerScript ChibiPatrolController = Chibi.GetComponent<MinionControllerScript>();
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
