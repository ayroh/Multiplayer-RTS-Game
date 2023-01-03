using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Random = UnityEngine.Random;
using Photon.Pun;
using UnityEngine.Events;

public class MinionControllerScript : MonoBehaviourPun{

    // Minion Variables
    [SerializeField] private int hitDamage = 4;
    [SerializeField] private Renderer mainRenderer;
    [SerializeField] private Material mainMaterial;
    private Animator animator;
    private float baseSpeed;
    private bool isChromancer = false;

    // Map Variables
    private NavMeshAgent navAgent;
    private BoxCollider navMeshMapCollider;

    // Other scripts has to access these
    [NonSerialized] public bool isAir = false;
     public bool isBlue = false; // can be change is more than 2 colors need to implemented
     public bool isInitialized = false;

    // Target Variables
    private GameObject targetObject = null;
    private Vector3[] destinations;
    private int currentDestination = 0;

    // Network Variables
    private Coroutine SyncCoroutine = null;
    private bool waitingSignalFromServer = true;
    private bool signalCameFromServer = false;
    private bool isOnline;


    private void OnEnable() => EventManager.OnWallDestroyed += SetAttackToCastleMulti; // When Destructible Wall has destructed than this sets destination to enemy castle according to color

    private void OnDisable() => EventManager.OnWallDestroyed -= SetAttackToCastleMulti;


    private void Awake() {
        isOnline = !PhotonNetwork.OfflineMode;
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        baseSpeed = navAgent.speed;
        if (TutorialManager.instance == null) {
            if (isOnline)
                SetNavMeshMapCollider(MultiplayerMapDataScript.instance.roadMainCollider);
            else
                SetNavMeshMapCollider(GameManager.instance.roadMainCollider);
            if (isOnline) {
                if (PhotonNetwork.IsMasterClient) {
                    Vector3[] tempDests = new Vector3[3];
                    for (int i = 0; i < tempDests.Length; ++i)
                        tempDests[i] = NewWanderPoint();
                    photonView.RPC("NewDestinationsMulti", RpcTarget.AllViaServer, tempDests);
                }
            }
            else
                NewDestinations();
        }
    }
    

    private void Update() {
        if ((!waitingSignalFromServer || targetObject != null) && !navAgent.pathPending && navAgent.remainingDistance < 1f) { //  && !towerDestination navAgent.remainingDistance < 1f
            if (isInitialized) {
                if(targetObject == null)
                    SetDestinationToCastle();
                else
                    animator.SetBool("Jump", true);
            }
            else {
                if (isOnline) {
                    waitingSignalFromServer = true;
                    if (!PhotonNetwork.IsMasterClient)
                        StartCoroutine(SendSignalToMasterCoroutine());
                }
                else
                    NextDestination();
            }
        }
    }

    #region Position Sync

    private IEnumerator SendSignalToMasterCoroutine() {
        do {
            photonView.RPC("SignalMasterClient", RpcTarget.Others);
            yield return new WaitForSeconds(1f);
        }
        while (waitingSignalFromServer);
    }

    [PunRPC]
    private void SignalMasterClient() {
        signalCameFromServer = true;
        if(SyncCoroutine == null)
            SyncCoroutine = StartCoroutine(RunningSyncCoroutine((currentDestination + 1) % destinations.Length));

    }

    private IEnumerator RunningSyncCoroutine(int nextDestination) {

        yield return new WaitUntil(() => waitingSignalFromServer == true);
        do {
            photonView.RPC("StartRunningSync", RpcTarget.AllViaServer, nextDestination as object);
            yield return new WaitForSeconds(1f);
        }
        while (signalCameFromServer);
    }

    [PunRPC]
    private void StartRunningSync(object nextDestination) {
        if (PhotonNetwork.IsMasterClient)
            SyncCoroutine = null;
        waitingSignalFromServer = false;
        signalCameFromServer = false;
        NextDestination((int)nextDestination);
    }

    [PunRPC]
    private void SyncPosition(float x, float z) => transform.position = new Vector3(x, transform.position.y, z);

    #endregion

    #region Trigger / Die

    // Changes color and destination of chibi if not initialized or are same color already
    [PunRPC]
    public void TriggerChibi(bool IsBlue) {
        if (animator.GetBool("Jump"))
            return;
        if (isBlue != IsBlue || !isInitialized) {
            if (!isInitialized) {
                isInitialized = true;
                if (isChromancer)
                    transform.localScale = new Vector3(1f, 1f, 1f);
                else
                    transform.DOScale(2f, 0.5f).SetEase(Ease.OutBounce); // setease bak
                navAgent.speed = baseSpeed + 2f;
            }
            isBlue = IsBlue;
            if (!navAgent.enabled)
                navAgent.enabled = true;
            
            if (IsBlue) {
                if (isOnline) 
                    mainRenderer.material = MultiplayerMapDataScript.instance.blueItemType.material;
                else {
                    if(TutorialManager.instance == null) 
                        mainRenderer.material = GameManager.instance.blueItemType.material;
                    else
                        mainRenderer.material = TutorialManager.instance.blueItemType.material;
                }
            }
            else {
                if (isOnline)
                    mainRenderer.material = MultiplayerMapDataScript.instance.redItemType.material;
                else {
                    if(TutorialManager.instance == null)
                        mainRenderer.material = GameManager.instance.redItemType.material;
                    else 
                        mainRenderer.material = TutorialManager.instance.redItemType.material;
                }
            }
            SetDestinationToCastle();
        }
    }

    public void TriggerChibiMulti(bool IsBlue) {
        if (animator.GetBool("Jump"))
            return;
        photonView.RPC("TriggerChibi", RpcTarget.AllViaServer, IsBlue);
    }


    private void DIEOther() => photonView.RPC("DIE", RpcTarget.Others);


    [PunRPC]
    public void DIE() {
        if (isOnline) {
            if (targetObject == null) {
                SetAttackToCastleMulti(true);
                return;
            }
            if (isChromancer) {
                if (photonView.IsMine) {
                    if (targetObject.TryGetComponent<CastleScript>(out CastleScript castleScript))
                        castleScript.OnCastleHit(hitDamage);
                    else if (targetObject.TryGetComponent<DestructibleWall>(out DestructibleWall wallScript))
                        wallScript.TakeDamage(hitDamage);
                    else if (targetObject.TryGetComponent<MGDScript>(out MGDScript mgdScript))
                        mgdScript.TakeDamage(hitDamage);
                    gameObject.SetActive(false);
                    KillMinion();
                }
            }
            else if ((!photonView.IsMine && !isBlue) || (photonView.IsMine && isBlue)) {
                if (targetObject.TryGetComponent<CastleScript>(out CastleScript castleScript))
                    castleScript.OnCastleHit(hitDamage);
                else if (targetObject.TryGetComponent<DestructibleWall>(out DestructibleWall wallScript))
                    wallScript.TakeDamage(hitDamage);
                else if (targetObject.TryGetComponent<MGDScript>(out MGDScript mgdScript))
                    mgdScript.TakeDamage(hitDamage);
                if (PhotonNetwork.IsMasterClient)
                    ReleaseMinion();
                else 
                    photonView.RPC("ReleaseMinion", RpcTarget.Others);
            }
            gameObject.SetActive(false);
        }
        else {
            if (targetObject == null) {
                SetAttackToCastle();
                return;
            }
            if (targetObject.TryGetComponent<CastleScript>(out CastleScript castleScript))
                castleScript.OnCastleHit(hitDamage);
            else if (targetObject.TryGetComponent<DestructibleWall>(out DestructibleWall wallScript))
                wallScript.TakeDamage(hitDamage);
            else if (targetObject.TryGetComponent<MGDScript>(out MGDScript mgdScript))
                mgdScript.TakeDamage(hitDamage);
            if (TutorialManager.instance != null || isChromancer)
                Destroy(gameObject);
            else
                GameManager.instance.ReleaseMinion(gameObject);
        }
    }

    #endregion

    #region Pooling / Kill / Reset

    [PunRPC]
    private void ReleaseMinion() => NetworkManagerScript.instance.ReleaseMinion(gameObject);

    public void PoolResetMulti(Vector3 newPos) {
        SetPositionMulti(newPos);
        ResetMinionMulti();
    }


    public void PoolReleaseMulti() => photonView.RPC("PoolRelease", RpcTarget.AllViaServer);

    [PunRPC]
    public void PoolRelease() => gameObject.SetActive(false);


    public void ResetMinionMulti() => photonView.RPC("ResetMinion", RpcTarget.AllViaServer);

    [PunRPC]
    public void ResetMinion() {
        if (animator.GetBool("Jump"))
            return; 
        targetObject = null;
        isInitialized = false;
        if (isChromancer)
            transform.DOScale(1f, .5f).SetEase(Ease.OutBounce);
        else
            transform.DOScale(1.5f, 0.5f).SetEase(Ease.OutBounce); 
        navAgent.speed = baseSpeed;
        mainRenderer.material = mainMaterial;
        isBlue = false;
        NextDestination();
        waitingSignalFromServer = false;
        signalCameFromServer = false;
    }

    [PunRPC]
    private void KillMinion() => PhotonNetwork.Destroy(gameObject);

    #endregion

    #region Setters / Destination

    public void NewDestinations() {
        if (destinations == null)
            destinations = new Vector3[3];
        for (int i = 0; i < destinations.Length; ++i)
            destinations[i] = NewWanderPoint();
    }

    [PunRPC]
    private void NewDestinationsMulti(Vector3[] newDests) {
        if (destinations == null)
            destinations = new Vector3[newDests.Length];
        for (int i = 0; i < newDests.Length; ++i)
            destinations[i] = newDests[i];
    }

    private void CurrentDestination() {
        if (targetObject != null)
            return;
        navAgent.SetDestination(destinations[currentDestination]);
    }

    public void NextDestination(int nextDestination = -1) {
        if (targetObject != null)
            return;
        if (nextDestination != -1)
            currentDestination = nextDestination;
        else
            currentDestination = (currentDestination + 1) % destinations.Length;
        navAgent.SetDestination(destinations[currentDestination]);
    }



    public void SendToPoint(Vector3 TargetPoint) {
        if (isOnline)
            photonView.RPC("SendToPointMulti", RpcTarget.All, TargetPoint);
        else { 
            if (navAgent.enabled == false)
                navAgent.enabled = true;
            waitingSignalFromServer = false;
            navAgent.SetDestination(TargetPoint);
            isAir = false;
        }
    }

    [PunRPC]
    public void SendToPointMulti(Vector3 TargetPoint) {
        if (navAgent.enabled == false)
            navAgent.enabled = true;
        waitingSignalFromServer = false;
        navAgent.SetDestination(TargetPoint);
        isAir = false;
    }


    private Vector3 NewWanderPoint() => new Vector3(Random.Range(navMeshMapCollider.bounds.min.x, navMeshMapCollider.bounds.max.x), navMeshMapCollider.bounds.center.y, Random.Range(navMeshMapCollider.bounds.min.z, navMeshMapCollider.bounds.max.z));

    public void SetNavMeshMapCollider(BoxCollider Col) => navMeshMapCollider = Col;


    public void SetChromancerMinion(bool IsBlue, int HitDamage) {
        if (isOnline) {
            photonView.RPC("SetChromancerMinionMulti", RpcTarget.AllViaServer, IsBlue, HitDamage);
        }
        else {
            isChromancer = true;
            hitDamage = HitDamage;
            //if (isBlue)
            //    targetObject = GameManager.instance.redCastle.gameObject;
            //else
            //    targetObject = GameManager.instance.blueCastle.gameObject;
            TriggerChibi(IsBlue);
        }

        
    }
    

    [PunRPC]
    private void SetChromancerMinionMulti(bool IsBlue, int HitDamage) {
        isChromancer = true;
        hitDamage = HitDamage;
        //if (isBlue)
        //    targetObject = MultiplayerMapDataScript.instance.redCastle.gameObject;
        //else
        //    targetObject = MultiplayerMapDataScript.instance.blueCastle.gameObject;
        TriggerChibi(IsBlue);
    }

    public void SetTarget(GameObject newTarget) => targetObject = newTarget;

    public void SetPositionMulti(Vector3 newPos) => photonView.RPC("SetPosition", RpcTarget.AllViaServer, newPos);

    [PunRPC]
    public void SetPosition(Vector3 newPos) {
        transform.position = newPos;
        gameObject.SetActive(true);
    }


    [PunRPC]
    public void SetNavAgentSpeedMulti(float newSpeed) => navAgent.speed = newSpeed;

    public void SetNavAgentSpeed(float newSpeed) {
        if (isOnline)
            photonView.RPC("SetNavAgentSpeedMulti", RpcTarget.AllViaServer, newSpeed);
        else
            navAgent.speed = newSpeed;
    }

    #endregion

    #region Set Attack to Castle/Wall
    public void SetDestinationToCastle() {
        if (isOnline) {
            if (!isInitialized) {
                CurrentDestination();
                return;
            }
            if (navAgent.enabled == false)
                navAgent.enabled = true;
            SetAttackToCastleMulti(true);
        }
        else {
            if (!isInitialized)
                CurrentDestination();
            else
                SetAttackToCastle();
        }
    }


    // When Destructible Wall has destructed than this sets destination to enemy castle according to color
    private void SetAttackToCastleMulti(bool value) { // bool kaldýr
        if (!isInitialized)
            return;
        if (isOnline)
            photonView.RPC("SetAttackToCastle", RpcTarget.AllViaServer);
        else
            SetAttackToCastle();
    }

    [PunRPC]
    private void SetAttackToCastle() { // might be punrpc if both clients need to control this movement
        if (!gameObject.activeInHierarchy)
            return;
        if (!navAgent.enabled)
            navAgent.enabled = true;
        if (isOnline) {
            if (isBlue)
                targetObject = MultiplayerMapDataScript.instance.redCastle.gameObject;
            else
                targetObject = MultiplayerMapDataScript.instance.blueCastle.gameObject;
        }
        else {
            if(TutorialManager.instance == null) {
                if (isBlue)
                    targetObject = GameManager.instance.redCastle.gameObject;
                else
                    targetObject = GameManager.instance.blueCastle.gameObject;
            }
            else {
                if (isBlue)
                    targetObject = TutorialManager.redCastle.gameObject;
                else
                    targetObject = TutorialManager.blueCastle.gameObject;
            }

        }
        navAgent.SetDestination(targetObject.GetComponent<CastleScript>().GetCollider().ClosestPoint(transform.position));
        isAir = false;
    }

    public void SetAttackToWallMulti(int wallViewID) => photonView.RPC("SetAttackToWall", RpcTarget.AllViaServer, wallViewID);

    [PunRPC]
    private void SetAttackToWall(int wallViewID) {
        GameObject wall = PhotonView.Find(wallViewID).gameObject;
        if (!navAgent.enabled)
            navAgent.enabled = true;
        navAgent.SetDestination(wall.transform.position);
        targetObject = wall;
    }

    public void SetAttackToWall(GameObject wall) {
        if (!navAgent.enabled)
            navAgent.enabled = true;
        navAgent.SetDestination(wall.transform.position);
        targetObject = wall;
    }

    #endregion

    public void DestMinion() => photonView.RPC("StartRunningSync", RpcTarget.AllViaServer, ((currentDestination + 1) % destinations.Length) as object);
}
