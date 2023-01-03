using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;
using ExitGames.Client.Photon;
using Random = UnityEngine.Random;
using DG.Tweening;

public class NetworkManagerScript : MonoBehaviourPunCallbacks
{
    public static NetworkManagerScript instance { get; private set; }
    [SerializeField] private ModifySO modifySO;
    public bool isBlue;

    #region MultiplayerCardManager

    AllCardsSO allCardsSO;

    [SerializeField] private GameObject MeteorPrefab;
    [SerializeField] private GameObject MeteorExplosionPrefab;
    [SerializeField] private GameObject DestructibleWallPrefab;
    [SerializeField] private GameObject MGDPrefab;
    [SerializeField] private GameObject PortalPrefab;
    [SerializeField] private GameObject SongOfValorPrefab;
    [SerializeField] private GameObject SPACMinionPrefab;
    [SerializeField] private GameObject ShowerPrefab;
    [SerializeField] private GameObject ChromancerPrefab;
    [SerializeField] private GameObject IceFieldPrefab;
    [SerializeField] private GameObject ScarecrowPrefab;
    [SerializeField] private GameObject ColorBallPrefab;
    [SerializeField] private GameObject TurretPrefab;
    [SerializeField] private GameObject VacuumPrefab;
    [SerializeField] private GameObject MonkPrefab;
    [SerializeField] private GameObject SpeedFieldPrefab;
    [SerializeField] private GameObject EarthquakePrefab;
    [SerializeField] private GameObject WallPrefab;
    [SerializeField] private GameObject ColorBombPrefab;
    [SerializeField] public GameObject OnlinePanel;
    [SerializeField] public CanonSO cannonTowerUpgrades;
    [SerializeField] public CanonSO rifleTowerUpgrades;
    [SerializeField] public CanonSO electricTowerUpgrades;
    public ManaScript manaScript;
    public SpawnerMultiplayerScript spawnerScript;

    public Action<GameObject> ReleaseMinion;

    // Skill Variables
    private float meteorInterval = 0.2f;
    private int numberOfMeteors = 20;
    private int songOfValorTime = 5;
    object[] onlyIsBlue;

    private void CastCard(string CardName, int CardMana, Vector3 firstTap = default(Vector3), Vector3 secondTap = default(Vector3)) {

        if (!manaScript.RemoveMana(CardMana))
            return;
        switch (CardName) {
            case "Meteor":
                //blueCastle.OnCastleHit(blueCastle.currentHealth / 4);
                //StartCoroutine(dropMeteor(numberOfMeteors));
                break;
            case "SongOfValor":
                //PhotonNetwork.Instantiate(SongOfValorPrefab.name, MultiplayerMapDataScript.instance.blueCastle.transform.position, Quaternion.identity);
                //Collider[] minions = Physics.OverlapBox(new Vector3(MultiplayerMapDataScript.instance.roadMainCollider.bounds.center.x, MultiplayerMapDataScript.instance.roadMainCollider.bounds.max.y, MultiplayerMapDataScript.instance.roadMainCollider.bounds.center.z - (MultiplayerMapDataScript.instance.roadMainCollider.bounds.center.z - MultiplayerMapDataScript.instance.roadMainCollider.bounds.min.z) / 2), new Vector3(MultiplayerMapDataScript.instance.roadMainCollider.bounds.max.x, 1, MultiplayerMapDataScript.instance.roadMainCollider.bounds.max.z / 4), Quaternion.identity, LayerMask.GetMask("minion"));
                //foreach (Collider minion in minions) {
                //    MinionControllerScript MinionScript = minion.GetComponent<MinionControllerScript>();
                //    if (MinionScript.isBlue != NetworkManagerScript.instance.isBlue || !MinionScript.isInitialized)
                //        MinionScript.TriggerChibiMulti(NetworkManagerScript.instance.isBlue);
                //}
                break;
            case "DestructibleWall":
                PhotonNetwork.Instantiate(DestructibleWallPrefab.name, firstTap, Quaternion.identity, 0, onlyIsBlue);
                break;
            case "MGD":
                object[] mgdArg = new object[] { isBlue, secondTap };
                PhotonNetwork.Instantiate(MGDPrefab.name, firstTap, Quaternion.identity, 0, mgdArg);
                break;
            case "IceField":
                PhotonNetwork.Instantiate(IceFieldPrefab.name, firstTap, Quaternion.identity, 0, onlyIsBlue);
                break;
            case "SpeedField":
                PhotonNetwork.Instantiate(SpeedFieldPrefab.name, firstTap, Quaternion.identity, 0, onlyIsBlue);
                break;
            case "Shower":
                PhotonNetwork.Instantiate(ShowerPrefab.name, firstTap, Quaternion.identity, 0, onlyIsBlue);
                break;
            case "Portal":
                PhotonNetwork.Instantiate(PortalPrefab.name, firstTap, Quaternion.identity, 0, onlyIsBlue);
                break;
            case "Earthquake":
                PhotonNetwork.Instantiate(EarthquakePrefab.name, firstTap, Quaternion.identity, 0, onlyIsBlue);
                break;
            case "Wall":
                PhotonNetwork.Instantiate(WallPrefab.name, firstTap, Quaternion.identity);
                break;
            case "ColorBomb":
                if (isBlue) {
                    Vector3 colorBombResult = ColorBombInstantiate(firstTap, MultiplayerMapDataScript.instance.blueCastle.GetShootPoint(), 1f);
                    object[] colorBombArg = new object[] { isBlue, colorBombResult };
                    PhotonNetwork.Instantiate(ColorBombPrefab.name, MultiplayerMapDataScript.instance.blueCastle.GetShootPoint(), Quaternion.identity, 0, colorBombArg);
                    MultiplayerMapDataScript.instance.blueCastle.transform.rotation = Quaternion.LookRotation(colorBombResult);
                }
                else {
                    Vector3 colorBombResult = ColorBombInstantiate(firstTap, MultiplayerMapDataScript.instance.redCastle.GetShootPoint(), 1f);
                    object[] colorBombArg = new object[] { isBlue, colorBombResult };
                    PhotonNetwork.Instantiate(ColorBombPrefab.name, MultiplayerMapDataScript.instance.redCastle.GetShootPoint(), Quaternion.identity, 0, colorBombArg);
                    MultiplayerMapDataScript.instance.redCastle.transform.rotation = Quaternion.LookRotation(colorBombResult);
                }
                break;
            case "SPACMinions":
                for (int i = 0; i < 5; ++i)
                    PhotonNetwork.Instantiate(SPACMinionPrefab.name, new Vector3(Random.Range(firstTap.x - 1f, firstTap.x + 1f), firstTap.y, Random.Range(firstTap.z - 1f, firstTap.z + 1f)), Quaternion.identity).GetComponent<SPACMinionScript>().InitMulti(isBlue);
                break;
            case "Monk":
                PhotonNetwork.Instantiate(MonkPrefab.name, firstTap, Quaternion.identity, 0, onlyIsBlue);
                break;
            case "Chromancer":
                PhotonNetwork.Instantiate(ChromancerPrefab.name, firstTap, Quaternion.identity, 0, onlyIsBlue);
                break;
            case "Vacuum":
                secondTap.y = 0f;
                firstTap.y = 0f;
                PhotonNetwork.Instantiate(VacuumPrefab.name, firstTap, Quaternion.LookRotation(secondTap - firstTap), 0, onlyIsBlue);
                break;
            case "ColorBall":
                Vector3 destination = secondTap;
                Vector3 offset = (secondTap - firstTap).normalized;
                offset = new Vector3(offset.x, 0f, offset.z);
                while (MultiplayerMapDataScript.instance.roadMainCollider.bounds.Contains((destination += offset))) ;
                object[] colorBallArg = new object[] { isBlue, destination};
                PhotonNetwork.Instantiate(ColorBallPrefab.name, firstTap, Quaternion.LookRotation(secondTap - firstTap), 0, colorBallArg);
                break;
            case "Scarecrow":
                PhotonNetwork.Instantiate(ScarecrowPrefab.name, firstTap, Quaternion.identity, 0, onlyIsBlue);
                break;
            case "Turret":
                PhotonNetwork.Instantiate(TurretPrefab.name, firstTap, Quaternion.LookRotation(secondTap - firstTap), 0, onlyIsBlue);
                break;
        }
    }

    private Vector3 ColorBombInstantiate(Vector3 target, Vector3 origin, float time) {
        Vector3 distance = target - origin;
        Vector3 distanceXz = distance;
        distanceXz.y = 0f;

        float sY = distance.y;
        float sXz = distanceXz.magnitude;

        float Vxz = sXz / time;
        float Vy = (sY / time) + (0.5f * Mathf.Abs(Physics.gravity.y) * time);

        Vector3 result = distanceXz.normalized;
        result *= Vxz;
        result.y = Vy;

        return result;
    }

    private IEnumerator dropMeteor(int count) {
        if (count > 0) {
            GameObject Meteor = PhotonNetwork.Instantiate(MeteorPrefab.name, new Vector3(MultiplayerMapDataScript.instance.roadMainCollider.bounds.max.x - Random.Range(0, MultiplayerMapDataScript.instance.roadMainCollider.bounds.max.x * 2), MultiplayerMapDataScript.instance.roadMainCollider.bounds.max.y + 80, MultiplayerMapDataScript.instance.roadMainCollider.bounds.max.z - Random.Range(0, MultiplayerMapDataScript.instance.roadMainCollider.bounds.max.z)), Quaternion.identity);
            Vector3 Target = new Vector3(MultiplayerMapDataScript.instance.roadMainCollider.bounds.max.x - Random.Range(0, MultiplayerMapDataScript.instance.roadMainCollider.bounds.max.x * 2), MultiplayerMapDataScript.instance.roadMainCollider.bounds.max.y, MultiplayerMapDataScript.instance.roadMainCollider.bounds.max.z - Random.Range(0, MultiplayerMapDataScript.instance.roadMainCollider.bounds.max.z * 2));
            photonView.RPC("SetTargetForMeteor", RpcTarget.AllViaServer, Meteor.GetPhotonView().ViewID, Target);
            yield return new WaitForSeconds(meteorInterval);
            StartCoroutine(dropMeteor(count - 1));
        }
    }

    [PunRPC]
    private void SetTargetForMeteor(int viewID, Vector3 Target) {
        GameObject Meteor = PhotonView.Find(viewID).gameObject;
        Meteor.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        Meteor.transform.DOMove(Target, 3f).OnComplete(() => ExplodeMeteor(Target));
    }

    private void ExplodeMeteor(Vector3 target) { // düzeltilebilir
        GameObject Explosion = PhotonNetwork.Instantiate(MeteorExplosionPrefab.name, target, Quaternion.Euler(Vector3.zero));
        //Explosion.GetComponent<MeteorExplosionScript>().SetItemType(MultiplayerMapDataScript.instance.blueItemType);
    }

    #endregion



    private void Awake() {
        if (instance != null && instance != this) 
            Destroy(instance.gameObject);
        instance = this;
        DontDestroyOnLoad(this);
        PhotonNetwork.OfflineMode = false;
        Application.targetFrameRate = 120;
    }

    public override void OnEnable() {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public override void OnDisable() {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    

    //private void GameEndedEvent(EventData obj) {
    //    if (obj.Code == MultiplayerMapDataScript.END_GAME_EVENT) 
    //        photonView.RPC("GameEnded", RpcTarget.All, obj.CustomData);

    //}

    public void LeaveGame() => photonView.RPC("GameEnded", RpcTarget.All, !isBlue);

    // change this to "isBlue" as parameter, everyone should control other players health
    public void EndGame() => photonView.RPC("GameEnded", RpcTarget.All, isBlue);

    [PunRPC]
    private void GameEnded(bool isBlueWin) {
        gameEnded = true;
        remainingTime = 180f;
        Time.timeScale = 0f;
        if (isBlueWin == isBlue)
            MultiplayerMapDataScript.instance.EndGamePanel.transform.Find("WinCondition").GetComponent<UnityEngine.UI.Text>().text = "YOU WON!";
        else
            MultiplayerMapDataScript.instance.EndGamePanel.transform.Find("WinCondition").GetComponent<UnityEngine.UI.Text>().text = "YOU LOST!";
        MultiplayerMapDataScript.instance.EndGamePanel.SetActive(true);
        MultiplayerMapDataScript.instance.SettingsButton.interactable = false;
        modifySO.Modify = true;
        modifySO.ModifyClose = false;
    }

    void Start(){
        // PhotonNetwork.GameVersion() = "0.0.1"; For every new update for game, update this too so user from previous versions cant login
        PhotonNetwork.AutomaticallySyncScene = true; // close this, errors on hot join
    }



    #region Main Menu Networking

    public void UnrankedPVPStart() {
        OnlinePanel.SetActive(true);
        if (PhotonNetwork.IsConnectedAndReady)
            return;
        print("connecting ");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2) {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel("MultiPlayerScene");
        }
    }

    public override void OnConnectedToMaster() {
        if (!PhotonNetwork.IsConnectedAndReady)
            return;
        print("connected");
        PhotonNetwork.JoinRandomOrCreateRoom();
    }


    public override void OnJoinedRoom() {
        if (!PhotonNetwork.IsConnectedAndReady)
            return;
        print("Entered Room Name: " + PhotonNetwork.CurrentRoom.Name);
    }
    

    public override void OnCreateRoomFailed(short returnCode, string message) {
        print("Room create failed: " + message);
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer) {
        print("Exit");
        EndGame();
        //PhotonNetwork.Destroy(photonView);
    }

    public override void OnDisconnected(DisconnectCause cause) {
        print("Disconnected: ");
        PhotonNetwork.AutomaticallySyncScene = true;
        //if (!PhotonNetwork.IsConnected)
        //    
        //else
        //    print("baglihala");
        //PhotonNetwork.Destroy(photonView);
    }

    public override void OnMasterClientSwitched(Player newMasterClient) { // use this
        base.OnMasterClientSwitched(newMasterClient);
    }

    public void StopSearchingOpponent() {
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
    }


    public void SetOnlinePanel(GameObject onlinePanel) => OnlinePanel = onlinePanel;

    #endregion

    #region InGame Networking
    
    //private IEnumerator LeaveRoom() {
    //    PhotonNetwork.LeaveRoom();
    //    while (PhotonNetwork.InRoom)
    //        yield return null;
    //    StartCoroutine(Disconnect());
    //}

    private IEnumerator Disconnect() {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnectedAndReady)
            yield return null;
        //try {
            PhotonNetwork.LoadLevel("MenuScene");
        //}
        //catch(Photon)
    }

    private void BackToHomeScene() {
        StartCoroutine(Disconnect());
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name.Equals("SinglePlayerScene")) {
            EventManager.OnCardCast -= CastCard;
            //PhotonNetwork.NetworkingClient.EventReceived -= GameEndedEvent;
            EventManager.OnLevelEndTriggeredAction -= BackToHomeScene;
            //Destroy(gameObject);
        }
        else if (scene.name.Equals("MultiPlayerScene")) {
            EventManager.OnCardCast += CastCard;
            //PhotonNetwork.NetworkingClient.EventReceived += GameEndedEvent;
            EventManager.OnLevelEndTriggeredAction += BackToHomeScene;
            PhotonNetwork.AutomaticallySyncScene = false;
            if (PhotonNetwork.IsMasterClient)
                isBlue = true;
            else {
                Transform main = MultiplayerMapDataScript.instance.vCam.transform;
                main.rotation = Quaternion.Euler(new Vector3(main.rotation.eulerAngles.x, main.rotation.eulerAngles.y + 180f, main.rotation.eulerAngles.z));
                main.position = new Vector3(main.position.x, main.position.y, -main.position.z);
                isBlue = false;
                photonView.RPC("SendSceneSyncData", RpcTarget.OthersBuffered);
            }
            onlyIsBlue = new object[] { isBlue };
        }
        else if (scene.name.Equals("MenuScene")) {
            EventManager.OnCardCast -= CastCard;
            PhotonNetwork.OfflineMode = false;
        }
    }

    [PunRPC]
    private void SendSceneSyncData() {
        print("other baglandi, oyun basliyor");
        photonView.RPC("WaitForSync", RpcTarget.AllViaServer);
        //photonView.StartCoroutine(WaitForSync());
    }

    [PunRPC]
    private void WaitForSync() {
        if (PhotonNetwork.IsMasterClient) {
            MultiplayerMapDataScript.instance.healthBarObject = PhotonNetwork.Instantiate("Healthbar", Vector3.zero, Quaternion.identity);
            photonView.RPC("SetHealthBarObject", RpcTarget.Others, MultiplayerMapDataScript.instance.healthBarObject.GetPhotonView().ViewID);
        }
        if (isBlue)
            SpawnBlueCannon();
        else
            SpawnRedCannon();
        if (PhotonNetwork.IsMasterClient)
            spawnerScript.StartSpawning();
        remainingTime = 180f;
        gameEnded = false;
    }

    [PunRPC]
    private void SetHealthBarObject(int viewID) => MultiplayerMapDataScript.instance.healthBarObject = PhotonView.Find(viewID).gameObject;

    private void SpawnBlueCannon() {
        MultiplayerMapDataScript.instance.blueCastle = PhotonNetwork.Instantiate(MultiplayerMapDataScript.instance.canonListSO.multiplayerCannonPrefabs[MultiplayerMapDataScript.instance.userSO.CurrentCannon].name, MultiplayerMapDataScript.instance.blueCastleSpawnPoint.position, Quaternion.identity).GetComponent<CastleScript>();
        MultiplayerMapDataScript.instance.blueCastle.enabled = true;
        MultiplayerMapDataScript.instance.blueCastle.Init(true);
        MultiplayerMapDataScript.instance.blueCastle.EnableCannon();
        MultiplayerMapDataScript.instance.blueCastle.SetHealthBar(MultiplayerMapDataScript.instance.healthBarObject.transform.Find("SliderBlue").GetComponent<HealthBarMultiplayerScript>());
        MultiplayerMapDataScript.instance.healthBarObject.transform.SetParent(MultiplayerMapDataScript.instance.canvas.transform);
        MultiplayerMapDataScript.instance.healthBarObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -40f);
        object[] arg = { isBlue, MultiplayerMapDataScript.instance.blueCastle.GetComponent<PhotonView>().ViewID, MultiplayerMapDataScript.instance.healthBarObject.GetComponent<PhotonView>().ViewID};
        photonView.RPC("SetCannon", RpcTarget.Others, arg);
    }
    private void SpawnRedCannon() {
        MultiplayerMapDataScript.instance.redCastle = PhotonNetwork.Instantiate(MultiplayerMapDataScript.instance.canonListSO.multiplayerCannonPrefabs[MultiplayerMapDataScript.instance.userSO.CurrentCannon].name, MultiplayerMapDataScript.instance.redCastleSpawnPoint.position, Quaternion.Euler(new Vector3(0f, 180f, 0f))).GetComponent<CastleScript>();
        MultiplayerMapDataScript.instance.redCastle.enabled = true;
        MultiplayerMapDataScript.instance.redCastle.Init(false);
        StartCoroutine(WaitSetHealthBarObject());
        MultiplayerMapDataScript.instance.redCastle.EnableCannon();
        object[] arg = { isBlue, MultiplayerMapDataScript.instance.redCastle.GetComponent<PhotonView>().ViewID };
        photonView.RPC("SetCannon", RpcTarget.Others, arg);
    }

    private IEnumerator WaitSetHealthBarObject() {
        while (MultiplayerMapDataScript.instance.healthBarObject == null)
            yield return null;
        MultiplayerMapDataScript.instance.redCastle.SetHealthBar(MultiplayerMapDataScript.instance.healthBarObject.transform.Find("SliderRed").GetComponent<HealthBarMultiplayerScript>());
    }

    [PunRPC]
    private void SetCannon(object[] arg) {
        if ((bool)arg[0]) {
            MultiplayerMapDataScript.instance.blueCastle = PhotonView.Find((int)arg[1]).GetComponent<CastleScript>();
            MultiplayerMapDataScript.instance.healthBarObject = PhotonView.Find((int)arg[2]).gameObject;
            MultiplayerMapDataScript.instance.blueCastle.SetHealthBar(MultiplayerMapDataScript.instance.healthBarObject.transform.Find("SliderBlue").GetComponent<HealthBarMultiplayerScript>());
            MultiplayerMapDataScript.instance.healthBarObject.transform.SetParent(MultiplayerMapDataScript.instance.canvas.transform);
            MultiplayerMapDataScript.instance.healthBarObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -40f);
        }
        else {
            MultiplayerMapDataScript.instance.redCastle = PhotonView.Find((int)arg[1]).GetComponent<CastleScript>();
            MultiplayerMapDataScript.instance.redCastle.SetHealthBar(MultiplayerMapDataScript.instance.healthBarObject.transform.Find("SliderRed").GetComponent<HealthBarMultiplayerScript>());
        }
        
    }

    #region EndTimer

    private float remainingTime = 180f;
    private bool gameEnded = true;

    private void Update() {
        if (gameEnded)
            return;
        if ((remainingTime -= Time.deltaTime) < 0f) {
            gameEnded = true;
            if (!PhotonNetwork.IsMasterClient)
                photonView.RPC("SendOtherEnd", RpcTarget.Others, MultiplayerMapDataScript.instance.blueCastle.currentHealth);
        }
        MultiplayerMapDataScript.instance.EndTimeText.text = ((int)remainingTime).ToString();
    }

    private IEnumerator WaitForOtherTimerEnd(int BlueCastleHealth) { 
        yield return new WaitUntil(() => gameEnded);
        if (MultiplayerMapDataScript.instance.redCastle.currentHealth > BlueCastleHealth)
            LeaveGame(); // red won
        else
            EndGame(); // blue won
            
    }

    [PunRPC]
    private void SendOtherEnd(int BlueCastleHealth) => StartCoroutine(WaitForOtherTimerEnd(BlueCastleHealth));

    #endregion


    #endregion
}
