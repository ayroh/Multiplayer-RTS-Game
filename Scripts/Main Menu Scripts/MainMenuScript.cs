using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private UserSO userSO;
    [SerializeField] private LevelSO levelSO;
    [SerializeField] private GameObject questPanel1;
    [SerializeField] private DeckSO deckSO;
    [SerializeField] private GameObject questPanel2;
    [SerializeField] private Text goldValueText;
    [SerializeField] private Text energyText;
    [SerializeField] private GameObject NotEnoughMoneyObj;
    [SerializeField] private GameObject alreadyUnlockedObj;
    [SerializeField] private UpgradeMenu upgradeMenu;
    [SerializeField] private GameObject levelPanel;
    [SerializeField] private GameObject menuSelectionBar;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject skillPointTutorialPanel;
    [SerializeField] private GameObject mapModifyTutorialPanel;
    [SerializeField] private GameObject termsPanel;
    [SerializeField] private CardMenuScript cardMenuScript;
    [SerializeField] private Text skillPointText;
    private Color selectionBarSelectedColor = new Color((float)244 / 255,(float)211 / 255,(float)11 / 255);
    private List<UserSO.Quest> readyQuests;

    public List<GameObject> Objects;
    public ModifySO ModifyData;

    private bool isEnergyReloading = false;

    private bool readyForInitialize = false;

    private void FixedUpdate() => ReloadEnergy();


    private async void Start() {

        //// Start Screen changing. Play button clicking automaticly. Temporary.

        UIManager uiManager = FindObjectOfType<UIManager>();
        uiManager.EnterMainMenu(true);
        ///

        await FirebaseManager.instance.LoadSO(userSO);
        FirebaseManager.instance.LoadSO(deckSO).ContinueWith(task => readyForInitialize = true);
        upgradeMenu.Init();

        StartCoroutine(WaitForDeckLoad());

        Application.targetFrameRate = 120;
        

        if (ModifyData.ModifyClose)
            ModifyData.Modify = false;

        //bilal in  þimdilik random 2 tanesini daily queste yolluyorum, procedurale çek

        readyQuests = new List<UserSO.Quest>();
        readyQuests.Add(new UserSO.Quest("MatchWin", "Win 2 matches", "Defeat your oppenents 2 times", 2));
        readyQuests.Add(new UserSO.Quest("MatchWin", "Win 2 matches", "Defeat your oppenents 2 times", 2));
        readyQuests.Add(new UserSO.Quest("MatchJoin", "Join 3 matches", "Join a match 3 times", 3));
        readyQuests.Add(new UserSO.Quest("MatchJoin", "Join 3 matches", "Join a match 3 times", 3));
        //              UserSO.Quest lar boþsa dolduruyo. win 7 match ve win 6 match denk gelemiyo þu an
        Random.InitState(System.DateTime.Now.Millisecond);
        if (string.IsNullOrEmpty(userSO.Quest1.QuestType))
        {
            int random;
            while (userSO.Quest2.QuestType == readyQuests[random = Random.Range(0, 4)].QuestType);
            changeQuestInfo(1, readyQuests[random]);
            FirebaseManager.instance.SaveSO(userSO);
        }
        if (string.IsNullOrEmpty(userSO.Quest2.QuestType))
        {
            int random2;
            while (userSO.Quest1.QuestType == readyQuests[random2 = Random.Range(0, 4)].QuestType);
            changeQuestInfo(2, readyQuests[random2]);
            FirebaseManager.instance.SaveSO(userSO);
        }
        refreshQuestInfos();

        ReloadDailyQuestTime();

        if (string.IsNullOrEmpty(userSO.LastEntry) || userSO.LastEntry == "0")
            userSO.LastEntry = System.DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss");

        ////////////////////////////////////////////////////////////////////////////////
        
        Time.timeScale = 1f;
        if (ModifyData.Modify) {
            EventManager.StartMainMenu(true);
            ModifyData.Modify = false;
        }
        if (!userSO.AcceptedEulaTerms)
            termsPanel.SetActive(true);
        if (!userSO.DoTalentTreeTutorial)
            OpenSelected(0);

        int maxSlider = Mathf.FloorToInt(100 * Mathf.Pow(1.15f, userSO.Level)); // formüller machinationstan

        GameObject levelSlider = levelPanel.transform.Find("LevelSlider").gameObject;
        // decrement to 4 decimal spaces
        levelSlider.GetComponent<Slider>().value = (float)System.Math.Round((float)userSO.Experience / (float)maxSlider, 4);
        levelSlider.transform.Find("CurrentLevelText").GetComponent<Text>().text = userSO.Level.ToString();
        levelSlider.transform.Find("NextLevelText").GetComponent<Text>().text = (userSO.Level + 1).ToString();

        refreshQuestInfos();


    }


    private void OnEnable() {
        EventManager.OnGameEnter += SelfSetState;
        EventManager.OnMenuSelect += OpenSelected;
        EventManager.OnEndSkillPointTutorial += EndSkillPointTutorial;
    }

    private void OnDisable() {
        EventManager.OnGameEnter -= SelfSetState;
        EventManager.OnMenuSelect -= OpenSelected;
        EventManager.OnEndSkillPointTutorial -= EndSkillPointTutorial;
    }

    private void SelfSetState(bool value) {
        this.gameObject.SetActive(value);
        if (userSO.DoTutorial)
            tutorialPanel.SetActive(true);
        else if (!userSO.DoTalentTreeTutorial)
            Objects[0].SetActive(true);
        else if (userSO.DoTalentTreeTutorial)
            TalentTreeTutorial();
    }
    IEnumerator WaitForDeckLoad()
    {
        yield return new WaitUntil(() => readyForInitialize == true);
        cardMenuScript.Initialize();
    }

    #region Tutorials

    public void OpenTutorialPanel() => tutorialPanel.SetActive(true);

    public async void EndSkillPointTutorial() {

        await FirebaseManager.instance.LoadSO(userSO);
        userSO.DoTalentTreeTutorial = false;
        await FirebaseManager.instance.SaveSO(userSO);
        skillPointTutorialPanel.SetActive(false);
        menuSelectionBar.transform.Find("HomeButton").gameObject.SetActive(true);
        menuSelectionBar.transform.Find("DeckButton").gameObject.SetActive(true);
        menuSelectionBar.transform.Find("StoreButton").gameObject.SetActive(true);
    }

    public async void AskForTutorial(bool isYes) {

        if (isYes) {
            PhotonNetwork.OfflineMode = true;
            if (userSO.CurrentCannon != 0)
                userSO.CurrentCannon = 0;
            await FirebaseManager.instance.SaveSO(userSO);
            SceneManager.LoadScene(3);
        }
        else {
            userSO.DoTutorial = false;
            tutorialPanel.SetActive(false);
            FirebaseManager.instance.SaveSO(userSO);
        }
    }


    private async void TalentTreeTutorial() {

        await FirebaseManager.instance.LoadSO(userSO);
        if (userSO.SkillPoint < 1)
            userSO.SkillPoint = 1;
        await FirebaseManager.instance.SaveSO(userSO);
        OpenSelected(2);
        menuSelectionBar.transform.Find("HomeButton").gameObject.SetActive(false);
        menuSelectionBar.transform.Find("DeckButton").gameObject.SetActive(false);
        menuSelectionBar.transform.Find("StoreButton").gameObject.SetActive(false);
        skillPointTutorialPanel.SetActive(true);
    }

    #endregion

    #region Open/Close UI

    private void OpenAll() {
        for(int i = 0; i < Objects.Count; i++)
            Objects[i].SetActive(true);
    }

    private void CloseAll() {
        for (int i = 0; i < Objects.Count; i++) {
            Objects[i].SetActive(false);
        }
        menuSelectionBar.transform.Find("HomeButton").Find("ButtonImage").GetComponent<Image>().color = Color.white;
        menuSelectionBar.transform.Find("HomeButton").Find("ButtonText").GetComponent<Text>().color = Color.white;
        menuSelectionBar.transform.Find("HomeButton").Find("Flag").gameObject.SetActive(false);
        
        menuSelectionBar.transform.Find("DeckButton").Find("ButtonImage").GetComponent<Image>().color = Color.white;
        menuSelectionBar.transform.Find("DeckButton").Find("ButtonText").GetComponent<Text>().color = Color.white;
        menuSelectionBar.transform.Find("DeckButton").Find("Flag").gameObject.SetActive(false);
        
        menuSelectionBar.transform.Find("TowerButton").Find("ButtonImage").GetComponent<Image>().color = Color.white;
        menuSelectionBar.transform.Find("TowerButton").Find("ButtonText").GetComponent<Text>().color = Color.white;
        menuSelectionBar.transform.Find("TowerButton").Find("Flag").gameObject.SetActive(false);

        menuSelectionBar.transform.Find("StoreButton").Find("ButtonImage").GetComponent<Image>().color = Color.white;
        menuSelectionBar.transform.Find("StoreButton").Find("ButtonText").GetComponent<Text>().color = Color.white;
        menuSelectionBar.transform.Find("StoreButton").Find("Flag").gameObject.SetActive(false);

        NotEnoughMoneyObj.SetActive(false);
        alreadyUnlockedObj.SetActive(false);
    }

    private void OpenSelected(int i) {
        CloseAll();
        switch (i) {
            case 0:
                menuSelectionBar.transform.Find("HomeButton").Find("ButtonImage").GetComponent<Image>().color = selectionBarSelectedColor;
                menuSelectionBar.transform.Find("HomeButton").Find("ButtonText").GetComponent<Text>().color = selectionBarSelectedColor;
                menuSelectionBar.transform.Find("HomeButton").Find("Flag").gameObject.SetActive(true);
                break;
            case 1:
                menuSelectionBar.transform.Find("DeckButton").Find("ButtonImage").GetComponent<Image>().color = selectionBarSelectedColor;
                menuSelectionBar.transform.Find("DeckButton").Find("ButtonText").GetComponent<Text>().color = selectionBarSelectedColor;
                menuSelectionBar.transform.Find("DeckButton").Find("Flag").gameObject.SetActive(true);
                break;
            case 2:
                skillPointText.text = userSO.SkillPoint.ToString() + " SKILL POINTS REMAINING";
                menuSelectionBar.transform.Find("TowerButton").Find("ButtonImage").GetComponent<Image>().color = selectionBarSelectedColor;
                menuSelectionBar.transform.Find("TowerButton").Find("ButtonText").GetComponent<Text>().color = selectionBarSelectedColor;
                menuSelectionBar.transform.Find("TowerButton").Find("Flag").gameObject.SetActive(true);
                break;
            case 3:
                goldValueText.text = getGoldUI().ToString();
                menuSelectionBar.transform.Find("StoreButton").Find("ButtonImage").GetComponent<Image>().color = selectionBarSelectedColor;
                menuSelectionBar.transform.Find("StoreButton").Find("ButtonText").GetComponent<Text>().color = selectionBarSelectedColor;
                menuSelectionBar.transform.Find("StoreButton").Find("Flag").gameObject.SetActive(true);
                break;
        }
        Objects[i].SetActive(true);

    }

    #endregion

    #region Energy
    private bool WaitForLoadEnergy = false;
    public void PlayEnergy(int energy) {
        
        FirebaseManager.instance.LoadSO(userSO).ContinueWith(task => WaitForLoadEnergy = true);
        StartCoroutine(PlayEnergyCoroutine(energy));

    }

    private IEnumerator PlayEnergyCoroutine(int energy)
    {
        yield return new WaitUntil(() => WaitForLoadEnergy);

        if (userSO.Energy < energy || PhotonNetwork.IsConnectedAndReady)
            yield break;
        PhotonNetwork.OfflineMode = true;
        userSO.Energy -= energy;
        var saveTask = FirebaseManager.instance.SaveSO(userSO);

        yield return new WaitUntil(() => saveTask.IsCompleted);

        SceneManager.LoadScene("SinglePlayerScene");
    }

    private async void ReloadEnergy() {

        if (userSO.Energy == 50) {
            energyText.text = 50.ToString();
            return;
        }
        System.DateTime lastTime;
        try {
            lastTime = System.DateTime.Parse(userSO.LastEntry);
        }
        catch (System.FormatException) {
            lastTime = System.DateTime.UtcNow;
        }
        System.TimeSpan timeInterval = System.DateTime.UtcNow - lastTime;
        if ((int)(timeInterval.TotalSeconds / 300) > 0 && !isEnergyReloading)
        {
            isEnergyReloading = true;
            await FirebaseManager.instance.LoadSO(userSO);
            userSO.LastEntry = System.DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss");
            userSO.Energy += 15 * (int)(timeInterval.TotalSeconds / 300);
            if (userSO.Energy > 50)
                userSO.Energy = 50;
            await FirebaseManager.instance.SaveSO(userSO);
            isEnergyReloading = false;
        }
        else userSO.LastEntry = lastTime.ToString();
        energyText.text = userSO.Energy.ToString();
    }

    #endregion

    #region InGame Store Buttons

    private UserSO.XpBoost xpBoost;
    private bool XPBoostLoaded;

    private bool CostDone;
    public void XPBoostCostButton(int Cost) {   // gelen deðer pozitif, fonksiyona yollarken negatife çevir, gelen boost þimdilik %20 sadece input almýyorum bunun için.
        
        FirebaseManager.instance.LoadSO(userSO).ContinueWith(task => XPBoostLoaded = true);

        StartCoroutine(XPBoostCostButtonCoroutine(Cost));
    }

    private IEnumerator XPBoostCostButtonCoroutine(int Cost)
    {
        yield return new WaitUntil(() => XPBoostLoaded == true);
        if (!changeGold(-Cost)) {
            openOrCloseNotEnoughMoneyButton(true);
            xpBoost = new UserSO.XpBoost(-1);
        }
        else {
            changeGoldUI(getGoldUI());
            xpBoost = new UserSO.XpBoost(Cost);
        }
        CostDone = true;
    }

    private bool DurationDone;
    public void XPBoostDurationButton(int Duration) { // Minutes
        StartCoroutine(XPBoostDurationButtonCoroutine(Duration));
    }

    private IEnumerator XPBoostDurationButtonCoroutine(int Duration)
    {
        yield return new WaitUntil(() => CostDone == true);
        if (xpBoost.cost == -1)
            yield break;
        xpBoost.duration = Duration;
        DurationDone = true;
    }

    public void XPBoostPercentageButton(int Percentage) {

        StartCoroutine(XPBooxtPercentageCoroutine(Percentage));
    }

    private IEnumerator XPBooxtPercentageCoroutine(int percentage)
    {
        yield return new WaitUntil(() => DurationDone == true);

        if (xpBoost.cost == -1 || xpBoost.duration == -1)
            yield break;
        xpBoost.percentage = percentage;
        userSO.XpBoosts.Add(xpBoost);
        FirebaseManager.instance.SaveSO(userSO);
    }

    public void GoldCostButton(int Cost) {
        //if (!changeGold(-Cost)) {
        //    openOrCloseNotEnoughMoneyButton(true);
        //    xpBoost = new UserSO.XpBoost(-1);
        //}
        //else {
        //    changeGoldUI(getGoldUI());
        //    xpBoost = new UserSO.XpBoost(Cost);
        //}
    }

    public void GoldBuyButton(int Percentage) {
        //if (xpBoost.cost == -1 || xpBoost.duration == -1)
        //    return;
        //xpBoost.percentage = Percentage;
        //userSO.XpBoosts.Add(xpBoost);
        //FirebaseManager.instance.SaveSO(userSO);
    }

    private string cardName;
    private int cardAmount = -1;
   

    public void CardNameButton(string CardName) => cardName = CardName;

    public void CardAmountButton(int CardAmount) => cardAmount = CardAmount;

    public void CardBuyButton(int CardCost) {
        if (string.IsNullOrEmpty(cardName) || cardAmount == -1)
            return;
        if (!changeGold(-CardCost)) {
            openOrCloseNotEnoughMoneyButton(true);
            return;
        }
        else 
            changeGoldUI(getGoldUI());
        // find cardname, add cardamount
    }

    public void BuyEnvSkillButton(int value) {  // gelen deðer pozitif, fonksiyona yollarken negatife çevir
        if (userSO.Unlockables.Contains("IceBlock")) {
            openOrCloseAlreadyUnlockedButton(true);
            return;
        }
        else if (!changeGold(-value)) {
            openOrCloseNotEnoughMoneyButton(true);
            return;
        }
        else 
            changeGoldUI(getGoldUI());
        userSO.Unlockables.Add("IceBlock");
        FirebaseManager.instance.SaveSO(userSO);
    }

    #endregion

    #region UI Buttons / Gold

    public void DisconnectFromServerButton() => PhotonNetwork.Disconnect();
    private bool changeGold(int changeValue) { // para arttýr azalt, 0ýn altýna düþerse hiçbir iþlem yapmaz ve false döndürür
        if (userSO.Gold + changeValue < 0)
            return false;
        userSO.Gold += changeValue;
        return true;
    }

    private void changeGoldUI(int value) {  // ingame storedaki gold yazýsýný günceller
        goldValueText.text = userSO.Gold.ToString();
    }

    private int getGoldUI() {
        return userSO.Gold;
    }

    public void openOrCloseNotEnoughMoneyButton(bool Open) {
        if (Open)
            NotEnoughMoneyObj.SetActive(true);
        else
            NotEnoughMoneyObj.SetActive(false);
    }

    public void openOrCloseAlreadyUnlockedButton(bool Open) {
        if (Open)
            alreadyUnlockedObj.SetActive(true);
        else
            alreadyUnlockedObj.SetActive(false);
    }

    #endregion

    #region Quest

    private void ReloadDailyQuestTime() {
        if (string.IsNullOrEmpty(userSO.NextDailyQuestTime) || userSO.NextDailyQuestTime == "0") // oyunu ilk yükleyiþte saati hazýrlýyorum, tryparsela daha güzel yapýlabilir
            userSO.NextDailyQuestTime = System.DateTime.Today.ToString("dd/MM/yyyy HH:mm:ss");

        // her gün saat gece 12de deðiþicek, system saatini UTCden alýyo, her sistemde ayný tahminen. Oyun ilk yüklendiðinde minvalueye eþit, onu bugüne (00:00) eþitleyip her gün addday yapýyorum.
        System.DateTime lastTime;
        try {
            lastTime = System.DateTime.Parse(userSO.NextDailyQuestTime);
        }
        catch (System.FormatException) {
            lastTime = System.DateTime.Today;
        }
        if (System.DateTime.UtcNow > lastTime) {
            userSO.NextDailyQuestTime = System.DateTime.Today.AddDays(1).ToString("dd/MM/yyyy HH:mm:ss");
            int random;
            do {
                random = Random.Range(0, 4);
            }
            while (userSO.Quest2.QuestType == readyQuests[random].QuestType);
            changeQuestInfo(1, readyQuests[random]);
            do {
                random = Random.Range(0, 4);
            }
            while (userSO.Quest1.QuestType == readyQuests[random].QuestType);
            changeQuestInfo(2, readyQuests[random]);
            FirebaseManager.instance.SaveSO(userSO);
        }
    }

    public void changeQuestInfo(int whichOne, UserSO.Quest CopyQuest) {
        if(whichOne == 1) {
            userSO.Quest1.QuestText = CopyQuest.QuestText;
            userSO.Quest1.QuestExplanation = CopyQuest.QuestExplanation;
            userSO.Quest1.currentAmount = CopyQuest.currentAmount;
            userSO.Quest1.QuestType = CopyQuest.QuestType;
            userSO.Quest1.WinAmount = CopyQuest.WinAmount;
            userSO.Quest1.RewardAmount = CopyQuest.RewardAmount;
            return;
        }
        else if(whichOne == 2) {
            userSO.Quest2.QuestText = CopyQuest.QuestText;
            userSO.Quest2.QuestExplanation = CopyQuest.QuestExplanation;
            userSO.Quest2.currentAmount = CopyQuest.currentAmount;
            userSO.Quest2.WinAmount = CopyQuest.WinAmount;
            userSO.Quest2.QuestType = CopyQuest.QuestType;
            userSO.Quest2.RewardAmount = CopyQuest.RewardAmount;
            return;
        }
        print("changeQuestInfo: WRONG INFO");
    }

    public void refreshQuestInfos() {
        questPanel1.transform.Find("QuestText1").GetComponent<Text>().text = userSO.Quest1.QuestText;
        questPanel1.transform.Find("QuestExplanation1").GetComponent<Text>().text = userSO.Quest1.QuestExplanation;
        questPanel1.transform.Find("QuestSlider1").GetComponent<Slider>().value = (float)userSO.Quest1.currentAmount / userSO.Quest1.WinAmount;
        questPanel1.transform.Find("QuestRewardText1").GetComponent<Text>().text = userSO.Quest1.RewardAmount.ToString();

        questPanel2.transform.Find("QuestText2").GetComponent<Text>().text = userSO.Quest2.QuestText;
        questPanel2.transform.Find("QuestExplanation2").GetComponent<Text>().text = userSO.Quest2.QuestExplanation;
        questPanel2.transform.Find("QuestSlider2").GetComponent<Slider>().value = (float)userSO.Quest2.currentAmount / userSO.Quest2.WinAmount;
        questPanel2.transform.Find("QuestRewardText2").GetComponent<Text>().text = userSO.Quest2.RewardAmount.ToString();
    }

    #endregion

    #region EULA/Terms

    public void acceptEulaTerms() {
        termsPanel.SetActive(false);
        userSO.AcceptedEulaTerms = true;
        FirebaseManager.instance.SaveSO(userSO);
    }

    public void EULAButton() {
        Application.OpenURL("http://www.google.com");
    }

    public void TermsButton() {
        Application.OpenURL("http://www.google.com");
    }

    #endregion

    #region Reset Game

    public void ResetGame() {
        userSO.AcceptedEulaTerms = false;
        userSO.DoTalentTreeTutorial = false;
        userSO.DoTutorial= false;
        userSO.Energy = 50;
        userSO.Experience = 0;
        userSO.Gold = 1500;
        userSO.LastEntry = null;
        userSO.Level = 0;
        userSO.SkillPoint = 15;
        userSO.Unlockables = null;
        userSO.XpBoosts = null;
        FirebaseManager.instance.SaveSO(userSO);
    }

    #endregion
}
