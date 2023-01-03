using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;
using System;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour{
    [SerializeField] private UserSO userSO;
    [SerializeField] private ModifySO modifySO;
    [SerializeField] private LevelSO levelSO;
    [SerializeField] private CanonListSO canonListSO;
    [SerializeField] private GameObject MeteorPrefab;
    [SerializeField] private GameObject DestructibleWallPrefab;
    [SerializeField] private GameObject MeteorExplosionPrefab;
    [SerializeField] private GameObject SongOfValorPrefab;
    [SerializeField] private GameObject IceFieldPrefab;
    [SerializeField] private GameObject SpeedFieldPrefab;
    [SerializeField] private GameObject PortalPrefab;
    [SerializeField] private GameObject TurretPrefab;
    [SerializeField] private GameObject MGDPrefab;
    [SerializeField] private GameObject ChromancerPrefab;
    [SerializeField] private GameObject WallPrefab;
    [SerializeField] private GameObject VacuumPrefab;
    [SerializeField] private GameObject MonkPrefab;
    [SerializeField] private GameObject ColorBombPrefab;
    [SerializeField] private GameObject ColorBallPrefab;
    [SerializeField] private GameObject ScarecrowPrefab;
    [SerializeField] private GameObject SPACMinionPrefab;
    [SerializeField] private GameObject EarthquakePrefab;
    [SerializeField] private GameObject ShowerPrefab;
    [SerializeField] private Material Blue;
    [SerializeField] private Material Red;
    [SerializeField] private GameObject EndGamePanel;
    [SerializeField] private GameObject healthBarObject;
    [SerializeField] private TextMeshProUGUI EndTimeText;
    [SerializeField] private Transform blueCastleSpawnPoint;
    [SerializeField] private Transform redCastleSpawnPoint;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private ManaScript manaScript;
    [SerializeField] public CanonSO cannonTowerUpgrades;
    [SerializeField] public CanonSO rifleTowerUpgrades;
    [SerializeField] public CanonSO electricTowerUpgrades;
    [SerializeField] public BoxCollider roadMainCollider;
    [SerializeField] public ItemTypeSO redItemType;
    [SerializeField] public ItemTypeSO blueItemType;

    public Action<GameObject> ReleaseMinion;
        
    // Minions use this to reach transform
    public CastleScript redCastle;
    public CastleScript blueCastle;
    //static public bool isBlue = true; i dont know who uses this

    // Skill Variables
    private float meteorInterval = 0.2f;
    private int numberOfMeteors = 20;
    private int songOfValorTime = 5;

    // End Game Variables
    private int currentGold;
    private int currentExperience;

    // Singleton
    public static GameManager instance;


    private void OnEnable() {
        EventManager.OnCardCast += CastCard;
        //EventManager.OnSongOfValorEnable += enableSongOfValorButton;
        //EventManager.OnMeteorEnable += enableMeteorButton;
        EventManager.OnLevelEndTriggeredAction += OnLevelEndTriggered;
        EventManager.OnLevelStatueAction += EndGameAsync;
    }

    private void OnDisable() {
        EventManager.OnCardCast -= CastCard;
        //EventManager.OnSongOfValorEnable -= enableSongOfValorButton;
        //EventManager.OnMeteorEnable -= enableMeteorButton;
        EventManager.OnLevelEndTriggeredAction -= OnLevelEndTriggered;
        EventManager.OnLevelStatueAction -= EndGameAsync;
    }

    

    private void Awake() {
        if (instance == null)
            instance = this;
        currentGold = 0;
        currentExperience = 0;
        SpawnBlueCannon();
        SpawnRedCannon();
    }

    #region EndTimer

    private float remainingTime = 180f;
    private bool gameEnded = false;


    private void Update() {
        if (gameEnded)
            return;
        if ((remainingTime -= Time.deltaTime) < 0f) {
            gameEnded = true;
            if (blueCastle.currentHealth > redCastle.currentHealth)
                EndGameAsync(true);
            else
                EndGameAsync(false);
        }
        EndTimeText.text = ((int)remainingTime).ToString();
    }

    #endregion

    #region Spawn Cannons
    private void SpawnBlueCannon() {
        blueCastle = Instantiate(canonListSO.CannonPrefabs[userSO.CurrentCannon], blueCastleSpawnPoint.position, Quaternion.identity).GetComponent<CastleScript>();
        blueCastle.Init(true, false, healthBarObject.transform.Find("SliderBlue").GetComponent<HealthBarMultiplayerScript>());
        blueCastle.EnableCannon();
    }
    private void SpawnRedCannon() {
        redCastle = Instantiate(canonListSO.CannonPrefabs[userSO.CurrentCannon], redCastleSpawnPoint.position, Quaternion.identity).GetComponent<CastleScript>();
        redCastle.Init(false, true,healthBarObject.transform.Find("SliderRed").GetComponent<HealthBarMultiplayerScript>());
        redCastle.EnableCannon();
        redCastle.transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
    }
    #endregion

    #region End Game

    async void EndGameAsync(bool isPlayerWin) {
        Time.timeScale = 0f;
        redCastle.DisableCannon();
        blueCastle.DisableCannon();

        await FirebaseManager.instance.LoadSO(userSO);

        // Disable game
        //disableMeteorButton();
        //disableSongOfValorButton();

        // End game rewards
        Random.InitState(System.DateTime.Now.Millisecond);
        if (isPlayerWin) {
            currentGold = Random.Range(75, 101);
            currentExperience = 35;
            EndGamePanel.transform.Find("WinCondition").GetComponent<Text>().text = "YOU WON!";
        }
        else {
            currentGold = Random.Range(30, 61);
            currentExperience = 20;
            EndGamePanel.transform.Find("WinCondition").GetComponent<Text>().text = "YOU LOST!";
        }
        DailyQuestEndControl(isPlayerWin);
        
        userSO.Gold += currentGold;
        int extraExpPercentage = 0;
        for (int i = 0; i < userSO.XpBoosts.Count; ++i) {
            DateTime lastTime;
            try {
                lastTime = DateTime.Parse(userSO.XpBoosts[i].time);
            }
            catch (FormatException) {
                lastTime = DateTime.UtcNow;
            }
            if ((DateTime.UtcNow - lastTime).TotalMinutes >= userSO.XpBoosts[i].duration)
                userSO.XpBoosts.RemoveAt(i);
            else
                extraExpPercentage += userSO.XpBoosts[i].percentage;
        }
        userSO.Experience += currentExperience + (int)((float)currentExperience * (float)extraExpPercentage / 100);

        int maxSlider = Mathf.FloorToInt(100 * Mathf.Pow(1.15f, userSO.Level)); // formüller machinationstan
        while (userSO.Experience >= maxSlider) {
            LevelUpReward();
            userSO.Experience -= maxSlider;
            userSO.Level++;
            maxSlider = Mathf.FloorToInt(100 * Mathf.Pow(1.15f, userSO.Level));
        }
        GameObject levelSlider = EndGamePanel.transform.Find("LevelPanel").Find("LevelSlider").gameObject;

        // decrement to 4 decimal spaces
        levelSlider.GetComponent<Slider>().value = (float)Math.Round((float)userSO.Experience / (float)maxSlider, 4);
        levelSlider.transform.Find("CurrentLevelText").GetComponent<Text>().text = userSO.Level.ToString();
        levelSlider.transform.Find("NextLevelText").GetComponent<Text>().text = (userSO.Level + 1).ToString();
        if(userSO.XpBoosts.Count == 0)
            EndGamePanel.transform.Find("ExpValue").GetComponent<Text>().text = currentExperience.ToString();
        else {
            int extraExp = 0;
            extraExp += (int)((float)currentExperience * (float)extraExpPercentage / 100);
            EndGamePanel.transform.Find("ExpValue").GetComponent<Text>().text = currentExperience.ToString() + "+" + extraExp.ToString();
        }
        EndGamePanel.transform.Find("GoldValue").GetComponent<Text>().text = currentGold.ToString();

        currentGold = 0;
        currentExperience = 0;
        await FirebaseManager.instance.SaveSO(userSO);
        EndGamePanel.SetActive(true);
    }

    private void OnLevelEndTriggered() {
        modifySO.Modify = true;
        modifySO.ModifyClose = false;
        EventManager.SetHomePage();
    }

    #endregion

    #region Rewards
    private void DailyQuestEndControl(bool winCondition) {
        if (userSO.Quest1.QuestType == "MatchJoin" && userSO.Quest1.WinAmount != userSO.Quest1.currentAmount) {
            if (userSO.Quest1.WinAmount == ++userSO.Quest1.currentAmount)
                userSO.Gold += userSO.Quest1.RewardAmount;
        }
        else if (userSO.Quest1.QuestType == "MatchWin" && userSO.Quest1.WinAmount != userSO.Quest1.currentAmount && winCondition) //       winconditiona statu giriliyo input olarak, onu boola çevir
            if (userSO.Quest1.WinAmount == ++userSO.Quest1.currentAmount)
                userSO.Gold += userSO.Quest1.RewardAmount;

        if (userSO.Quest2.QuestType == "MatchJoin" && userSO.Quest2.WinAmount != userSO.Quest2.currentAmount) {
            if (userSO.Quest2.WinAmount == ++userSO.Quest2.currentAmount)
                userSO.Gold += userSO.Quest2.RewardAmount;
        }
        else if (userSO.Quest2.QuestType == "MatchWin" && userSO.Quest2.WinAmount != userSO.Quest2.currentAmount && winCondition) //       winconditiona statu giriliyo input olarak, onu boola çevir
            if (userSO.Quest2.WinAmount == ++userSO.Quest2.currentAmount)
                userSO.Gold += userSO.Quest2.RewardAmount;
    }

    private void LevelUpReward() { // usersodan aldıyo bilgileri fakat levelup usersoya savelenmeden bu fonksiyon çağrıldığı için userSO.Level -1 yerine userSO.level yapıyorum

        if (levelSO.LevelRewards.Count <= userSO.Level)
            return;
        string Type = levelSO.LevelRewards[userSO.Level].Type;
        if(Type == "SkillPoint") {
            userSO.SkillPoint += levelSO.LevelRewards[userSO.Level].Value;
        }
        else if(Type == "Unlock") {
            for (int i = 0; i < levelSO.LevelRewards[userSO.Level].Unlocks.Length; ++i)
                userSO.Unlockables.Add(levelSO.LevelRewards[userSO.Level].Unlocks[i]);
        }
        else if(Type == "Unlock+Gold") {
            for (int i = 0; i < levelSO.LevelRewards[userSO.Level].Unlocks.Length; ++i)
                userSO.Unlockables.Add(levelSO.LevelRewards[userSO.Level].Unlocks[i]);
            userSO.Gold += levelSO.LevelRewards[userSO.Level].Value;
        }
    }
    #endregion

    #region Skills
    private void CastCard(string CardName, int CardMana , Vector3 firstTap = default(Vector3), Vector3 secondTap = default(Vector3)) {
        if (!manaScript.RemoveMana(CardMana))
            return;
        switch (CardName) {
            case "Meteor":
                //blueCastle.OnCastleHit(blueCastle.currentHealth / 4);
                //StartCoroutine(dropMeteor(numberOfMeteors));
                break;
            case "SongOfValor":
                //Instantiate(SongOfValorPrefab, blueCastle.transform.position, Quaternion.identity);
                //Collider[] Chibis = Physics.OverlapBox(new Vector3(roadMainCollider.bounds.center.x, roadMainCollider.bounds.max.y, roadMainCollider.bounds.center.z - (roadMainCollider.bounds.center.z - roadMainCollider.bounds.min.z) / 2), new Vector3(roadMainCollider.bounds.max.x, 1, roadMainCollider.bounds.max.z / 4), Quaternion.identity, LayerMask.GetMask("Chibi"));
                //foreach (Collider chibi in Chibis) {
                //    PatrolController ChibiScript = chibi.GetComponent<PatrolController>();
                //    if (!ChibiScript.isBlue || !ChibiScript.isInitialized) {
                //        if (!ChibiScript.isInitialized)
                //            EventManager.DeleteChibiFromEnemies(chibi.gameObject);
                //        ChibiScript.TriggerChibi(blueItemType);
                //    }
                //}
                break;
            case "DestructibleWall":
                Instantiate(DestructibleWallPrefab, firstTap, Quaternion.identity).GetComponent<DestructibleWall>().isBlue = blueItemType.isBlue;
                break;
            case "MGD":
                Instantiate(MGDPrefab, firstTap, Quaternion.identity).GetComponent<MGDScript>().OfflineInit(blueItemType.isBlue, secondTap, roadMainCollider);
                break;
            case "IceField":
                Instantiate(IceFieldPrefab, firstTap, Quaternion.identity).GetComponent<IceFieldScript>().OfflineInit(blueItemType.isBlue);
                break;
            case "SpeedField":
                Instantiate(SpeedFieldPrefab, firstTap, Quaternion.identity);
                break;
            case "Shower":
                Instantiate(ShowerPrefab, firstTap, Quaternion.identity);
                break;
            case "Portal":
                Instantiate(PortalPrefab, firstTap, Quaternion.identity).GetComponent<PortalScript>().OfflineInit(blueItemType.isBlue);
                break;
            case "Earthquake":
                Instantiate(EarthquakePrefab, firstTap, Quaternion.identity);
                break;
            case "Wall":
                Instantiate(WallPrefab, firstTap, Quaternion.identity);
                break;
            case "ColorBomb":
                Rigidbody colorBomb = Instantiate(ColorBombPrefab, blueCastle.GetShootPoint(), Quaternion.identity).GetComponent<Rigidbody>();
                Vector3 colorBombResult = ColorBombInstantiate(firstTap, blueCastle.GetShootPoint(), 1f);
                colorBomb.velocity = colorBombResult;
                colorBomb.GetComponent<ColorBombScript>().isBlue = true;
                blueCastle.SetCannonRotation(colorBombResult);
                break;
            case "SPACMinions":
                for (int i = 0; i < 5; ++i)
                    Instantiate(SPACMinionPrefab, new Vector3(Random.Range(firstTap.x - 1f, firstTap.x + 1f), firstTap.y, Random.Range(firstTap.z - 1f, firstTap.z + 1f)), Quaternion.identity).GetComponent<SPACMinionScript>().Init(true);
                break;
            case "Monk":
                Instantiate(MonkPrefab, firstTap, Quaternion.identity).GetComponent<MonkScript>().Init(true);
                break;
            case "Chromancer":
                Instantiate(ChromancerPrefab, firstTap, Quaternion.identity).GetComponent<ChromancerScript>().Init(true);
                break;
            case "Vacuum":
                Instantiate(VacuumPrefab, firstTap, Quaternion.LookRotation(secondTap - firstTap)).GetComponent<VacuumScript>().Init(true);
                break;
            case "ColorBall":
                Instantiate(ColorBallPrefab, firstTap, Quaternion.LookRotation(secondTap - firstTap)).GetComponent<ColorBallScript>().Init(true, secondTap);
                break;
            case "Scarecrow":
                Instantiate(ScarecrowPrefab, firstTap, Quaternion.identity).GetComponent<ScarecrowScript>().Init(true);
                break;
            case "Turret":
                Instantiate(TurretPrefab, firstTap, Quaternion.LookRotation(secondTap - firstTap)).GetComponent<TurretScript>(); // no init since only player uses skills
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
            GameObject Meteor = Instantiate(MeteorPrefab, new Vector3(roadMainCollider.bounds.max.x - Random.Range(0, roadMainCollider.bounds.max.x * 2), roadMainCollider.bounds.max.y + 80, roadMainCollider.bounds.max.z - Random.Range(0, roadMainCollider.bounds.max.z)), Quaternion.identity);
            Meteor.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            //Meteor.transform.DOMoveY(roadMainCollider.bounds.max.y, 1f).OnComplete(() => Instantiate(MeteorExplosionPrefab, Meteor.transform.position, Quaternion.Euler(Vector3.zero)));
            Vector3 Target = new Vector3(roadMainCollider.bounds.max.x - Random.Range(0, roadMainCollider.bounds.max.x * 2), roadMainCollider.bounds.max.y, roadMainCollider.bounds.max.z - Random.Range(0, roadMainCollider.bounds.max.z * 2));
            Meteor.transform.DOMove(Target, 3f).OnComplete(() => ExplodeMeteor(Target));
            yield return new WaitForSeconds(meteorInterval);
            StartCoroutine(dropMeteor(count - 1));
        }
    }

    private void ExplodeMeteor(Vector3 target) { // düzeltilebilir
        GameObject Explosion = Instantiate(MeteorExplosionPrefab, target, Quaternion.Euler(Vector3.zero));       
        //Explosion.GetComponent<MeteorExplosionScript>().SetItemType(blueItemType);
    }

    #endregion

    #region UI

    public void OpenSettings() {
        if (settingsPanel.activeSelf) {
            settingsPanel.SetActive(false);
            Time.timeScale = 1f;
        }
        else {
            Time.timeScale = 0f;
            settingsPanel.SetActive(true);
        }
    }

    #endregion

}

