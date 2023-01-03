using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenu : MonoBehaviour
{

    /// BÝRAZ HARD CODED, 3TEN FAZLA SÝLAH OLURSA DEÐÝÞTÝRÝLMELÝ. SAÐ VE SOLA KAYDIRMA ZIPLAMAK YERÝNE BÝRBÝRÝNÝN POZÝSYONUNA GÖRE MOVE YAPIYO
    /// (CASTLE 0A GÝTMEK ÝÇÝN CASTLE2YE MOVEX YAPIYORUM ÇÜNKÜ KAMERA DEÐÝL OBJENÝN KENDÝSÝ HAREKET EDÝYO)
    
    [SerializeField] private CanonSO[] _cannonSO;
    [SerializeField] private UserSO userSO;
    [SerializeField] private GameObject Castle0;
    [SerializeField] private GameObject Castle1;
    [SerializeField] private GameObject Castle2;
    [SerializeField] private Transform Castle0StartingPoint;
    [SerializeField] private Text skillPointText;
    [SerializeField] private Transform Castle1StartingPoint;
    [SerializeField] private Transform Castle2StartingPoint;

    private bool isLoaded = false;
    private int _currentCannonID;

    public async void Init() {
        for(int i = 0; i < 3;++i)
           await FirebaseManager.instance.LoadSO(_cannonSO[i]);
        GetUnlockInfos();
        _currentCannonID = userSO.CurrentCannon;

        if (_currentCannonID == 0)
            transform.position = new Vector3(Castle2StartingPoint.transform.position.x, transform.position.y, transform.position.z);
        else if (_currentCannonID == 1)
            transform.position = Castle1StartingPoint.transform.position;
        else if (_currentCannonID == 2)
            transform.position = new Vector3(Castle0StartingPoint.transform.position.x, transform.position.y, transform.position.z);
    }
    /*
    private void ManipulateCanonID() {
        if (_currentCannonID > -1 || _currentCannonID < 3) {
            EventManager.CannonStatueAction(_currentCannonID);
        }
    }*/

    private async void OnDisable()
    {
        if (userSO.CurrentCannon == _currentCannonID)
            return;
        await FirebaseManager.instance.LoadSO(userSO);
        userSO.CurrentCannon = _currentCannonID;
        FirebaseManager.instance.SaveSO(userSO);
    }

    public void MoveLeft() {
        if (userSO.DoTalentTreeTutorial)
            return;
        switch (_currentCannonID) {
            case 2:
                _currentCannonID = 1;
                transform.DOKill();
                transform.DOMoveX(Castle1StartingPoint.transform.position.x, 1f);
                break;
            case 1:
                _currentCannonID = 0;
                transform.DOKill();
                transform.DOMoveX(Castle2StartingPoint.transform.position.x, 1f);
                break;
        }
        //userSO.CurrentCannon = _currentCannonID;
    }

    public void MoveRight() {
        if (userSO.DoTalentTreeTutorial)
            return;
        switch (_currentCannonID) {
            case 0:
                _currentCannonID = 1;
                transform.DOKill();
                transform.DOMoveX(Castle1StartingPoint.transform.position.x , 1f);
                break;
            case 1:
                _currentCannonID = 2;
                transform.DOKill();
                transform.DOMoveX(Castle0StartingPoint.transform.position.x , 1f);
                break;
        }
        //userSO.CurrentCannon = _currentCannonID;
    }
   
    private void GetUnlockInfos() {
        for(int i = 0;i < 3; ++i) {
            switch (i) {
                case 0:
                    if (_cannonSO[0]._unlockedUpgrades[0] == 1) {
                        
                        Castle0.transform.Find("Level1Button").Find("UpgradedShine").gameObject.SetActive(true);
                    }
                    if (_cannonSO[0]._unlockedUpgrades[1] == 1) {
                        Castle0.transform.Find("LeftUpArrow").gameObject.SetActive(true);
                        Castle0.transform.Find("Level3LeftButton").Find("UpgradedShine").gameObject.SetActive(true);
                    }
                    if (_cannonSO[0]._unlockedUpgrades[2] == 1) {
                        Castle0.transform.Find("RightUpArrow").gameObject.SetActive(true);
                        Castle0.transform.Find("Level3RightButton").Find("UpgradedShine").gameObject.SetActive(true);
                    }
                    if (_cannonSO[0]._unlockedUpgrades[3] == 1) {
                        Castle0.transform.Find("UpArrowLeftOne").gameObject.SetActive(true);
                        Castle0.transform.Find("Level5LeftButton").Find("UpgradedShine").gameObject.SetActive(true);
                    }
                    if (_cannonSO[0]._unlockedUpgrades[4] == 1) {
                        Castle0.transform.Find("UpArrowRightOne").gameObject.SetActive(true);
                        Castle0.transform.Find("Level5RightButton").Find("UpgradedShine").gameObject.SetActive(true);
                    }
                    break;
                case 1:
                    if (_cannonSO[1]._unlockedUpgrades[0] == 1) {
                        Castle1.transform.Find("Level1Button").Find("UpgradedShine").gameObject.SetActive(true);
                    }
                    if (_cannonSO[1]._unlockedUpgrades[1] == 1) {
                        Castle1.transform.Find("LeftUpArrow").gameObject.SetActive(true);
                        Castle1.transform.Find("Level3LeftButton").Find("UpgradedShine").gameObject.SetActive(true);
                    }
                    if (_cannonSO[1]._unlockedUpgrades[2] == 1) {
                        Castle1.transform.Find("RightUpArrow").gameObject.SetActive(true);
                        Castle1.transform.Find("Level3RightButton").Find("UpgradedShine").gameObject.SetActive(true);
                    }
                    if (_cannonSO[1]._unlockedUpgrades[3] == 1) {
                        Castle1.transform.Find("UpArrowLeftOne").gameObject.SetActive(true);
                        Castle1.transform.Find("Level5LeftButton").Find("UpgradedShine").gameObject.SetActive(true);
                    }
                    if (_cannonSO[1]._unlockedUpgrades[4] == 1) {
                        Castle1.transform.Find("UpArrowRightOne").gameObject.SetActive(true);
                        Castle1.transform.Find("Level5RightButton").Find("UpgradedShine").gameObject.SetActive(true);
                    }
                    break;
                case 2:
                    if (_cannonSO[2]._unlockedUpgrades[0] == 1) {
                        Castle2.transform.Find("Level1Button").Find("UpgradedShine").gameObject.SetActive(true);
                    }
                    if (_cannonSO[2]._unlockedUpgrades[1] == 1) {
                        Castle2.transform.Find("LeftUpArrow").gameObject.SetActive(true);
                        Castle2.transform.Find("Level3LeftButton").Find("UpgradedShine").gameObject.SetActive(true);
                    }
                    if (_cannonSO[2]._unlockedUpgrades[2] == 1) {
                        Castle2.transform.Find("RightUpArrow").gameObject.SetActive(true);
                        Castle2.transform.Find("Level3RightButton").Find("UpgradedShine").gameObject.SetActive(true);
                    }
                    if (_cannonSO[2]._unlockedUpgrades[3] == 1) {
                        Castle2.transform.Find("UpArrowLeftOne").gameObject.SetActive(true);
                        Castle2.transform.Find("Level5LeftButton").Find("UpgradedShine").gameObject.SetActive(true);
                    }
                    if (_cannonSO[2]._unlockedUpgrades[4] == 1) {
                        Castle2.transform.Find("UpArrowRightOne").gameObject.SetActive(true);
                        Castle2.transform.Find("Level5RightButton").Find("UpgradedShine").gameObject.SetActive(true);
                    }
                    break;
            }
        }
    }

    public void Open(int upgrade)
    {
        FirebaseManager.instance.LoadSO(userSO).ContinueWith(task => isLoaded = true);

        StartCoroutine(ResetAfterLoading(upgrade));
    }

    private IEnumerator ResetAfterLoading(int upgrade)
    {
        yield return new WaitUntil(() => isLoaded);

        if (userSO.SkillPoint <= 0)
            yield break;

        switch (_currentCannonID)
        {
            case 0:
                switch (upgrade)
                {
                    case 0:
                        if (Castle0.transform.Find("Level1Button").Find("UpgradedShine").gameObject.activeSelf)
                            yield break;
                        if (userSO.DoTalentTreeTutorial)
                            EventManager.EndSkillPointTutorial();
                        Castle0.transform.Find("Level1Button").Find("UpgradedShine").gameObject.SetActive(true);
                        --userSO.SkillPoint;
                        ReloadSkillPointText();
                        break;
                    case 1:
                        if (userSO.Level < 3 || !Castle0.transform.Find("Level1Button").Find("UpgradedShine").gameObject.activeSelf || Castle0.transform.Find("Level3LeftButton").Find("UpgradedShine").gameObject.activeSelf)
                            yield break;

                        Castle0.transform.Find("Level3LeftButton").Find("UpgradedShine").gameObject.SetActive(true);
                        Castle0.transform.Find("LeftUpArrow").gameObject.SetActive(true);
                        --userSO.SkillPoint;
                        ReloadSkillPointText();
                        break;
                    case 2:
                        if (userSO.Level < 3 || !Castle0.transform.Find("Level1Button").Find("UpgradedShine").gameObject.activeSelf || Castle0.transform.Find("Level3RightButton").Find("UpgradedShine").gameObject.activeSelf)
                            yield break;
                        Castle0.transform.Find("Level3RightButton").Find("UpgradedShine").gameObject.SetActive(true);
                        Castle0.transform.Find("RightUpArrow").gameObject.SetActive(true);
                        --userSO.SkillPoint;
                        ReloadSkillPointText();
                        break;
                    case 3:
                        if (userSO.Level < 5 || !Castle0.transform.Find("Level3LeftButton").Find("UpgradedShine").gameObject.activeSelf || Castle0.transform.Find("Level5LeftButton").Find("UpgradedShine").gameObject.activeSelf)
                            yield break;
                        Castle0.transform.Find("Level5LeftButton").Find("UpgradedShine").gameObject.SetActive(true);
                        Castle0.transform.Find("UpArrowLeftOne").gameObject.SetActive(true);
                        --userSO.SkillPoint;
                        ReloadSkillPointText();
                        break;
                    case 4:
                        if (userSO.Level < 5 || !Castle0.transform.Find("Level3RightButton").Find("UpgradedShine").gameObject.activeSelf || Castle0.transform.Find("Level5RightButton").Find("UpgradedShine").gameObject.activeSelf)
                            yield break;
                        Castle0.transform.Find("Level5RightButton").Find("UpgradedShine").gameObject.SetActive(true);
                        Castle0.transform.Find("UpArrowRightOne").gameObject.SetActive(true);
                        --userSO.SkillPoint;
                        ReloadSkillPointText();
                        break;
                }
                break;
            case 1:
                switch (upgrade)
                {
                    case 0:
                        if (Castle1.transform.Find("Level1Button").Find("UpgradedShine").gameObject.activeSelf)
                            yield break;
                        Castle1.transform.Find("Level1Button").Find("UpgradedShine").gameObject.SetActive(true);
                        --userSO.SkillPoint;
                        ReloadSkillPointText();
                        break;
                    case 1:
                        if (userSO.Level < 3 || !Castle1.transform.Find("Level1Button").Find("UpgradedShine").gameObject.activeSelf || Castle1.transform.Find("Level3LeftButton").Find("UpgradedShine").gameObject.activeSelf)
                            yield break;

                        Castle1.transform.Find("Level3LeftButton").Find("UpgradedShine").gameObject.SetActive(true);
                        Castle1.transform.Find("LeftUpArrow").gameObject.SetActive(true);
                        --userSO.SkillPoint;
                        ReloadSkillPointText();
                        break;
                    case 2:
                        if (userSO.Level < 3 || !Castle1.transform.Find("Level1Button").Find("UpgradedShine").gameObject.activeSelf || Castle1.transform.Find("Level3RightButton").Find("UpgradedShine").gameObject.activeSelf)
                            yield break;
                        Castle1.transform.Find("Level3RightButton").Find("UpgradedShine").gameObject.SetActive(true);
                        Castle1.transform.Find("RightUpArrow").gameObject.SetActive(true);
                        --userSO.SkillPoint;
                        ReloadSkillPointText();
                        break;
                    case 3:
                        if (userSO.Level < 5 || !Castle1.transform.Find("Level3LeftButton").Find("UpgradedShine").gameObject.activeSelf || Castle1.transform.Find("Level5LeftButton").Find("UpgradedShine").gameObject.activeSelf)
                            yield break;
                        Castle1.transform.Find("Level5LeftButton").Find("UpgradedShine").gameObject.SetActive(true);
                        Castle1.transform.Find("UpArrowLeftOne").gameObject.SetActive(true);
                        --userSO.SkillPoint;
                        ReloadSkillPointText();
                        break;
                    case 4:
                        if (userSO.Level < 5 || !Castle1.transform.Find("Level3RightButton").Find("UpgradedShine").gameObject.activeSelf || Castle1.transform.Find("Level5RightButton").Find("UpgradedShine").gameObject.activeSelf)
                            yield break;
                        Castle1.transform.Find("Level5RightButton").Find("UpgradedShine").gameObject.SetActive(true);
                        Castle1.transform.Find("UpArrowRightOne").gameObject.SetActive(true);
                        --userSO.SkillPoint;
                        ReloadSkillPointText();
                        break;
                }
                break;
            case 2:
                switch (upgrade)
                {
                    case 0:
                        if (Castle2.transform.Find("Level1Button").Find("UpgradedShine").gameObject.activeSelf)
                            yield break;
                        Castle2.transform.Find("Level1Button").Find("UpgradedShine").gameObject.SetActive(true);
                        --userSO.SkillPoint;
                        ReloadSkillPointText();
                        break;
                    case 1:
                        if (userSO.Level < 3 || !Castle2.transform.Find("Level1Button").Find("UpgradedShine").gameObject.activeSelf || Castle2.transform.Find("Level3LeftButton").Find("UpgradedShine").gameObject.activeSelf)
                            yield break;

                        Castle2.transform.Find("Level3LeftButton").Find("UpgradedShine").gameObject.SetActive(true);
                        Castle2.transform.Find("LeftUpArrow").gameObject.SetActive(true);
                        --userSO.SkillPoint;
                        ReloadSkillPointText();
                        break;
                    case 2:
                        if (userSO.Level < 3 || !Castle2.transform.Find("Level1Button").Find("UpgradedShine").gameObject.activeSelf || Castle2.transform.Find("Level3RightButton").Find("UpgradedShine").gameObject.activeSelf)
                            yield break;
                        Castle2.transform.Find("Level3RightButton").Find("UpgradedShine").gameObject.SetActive(true);
                        Castle2.transform.Find("RightUpArrow").gameObject.SetActive(true);
                        --userSO.SkillPoint;
                        ReloadSkillPointText();
                        break;
                    case 3:
                        if (userSO.Level < 5 || !Castle2.transform.Find("Level3LeftButton").Find("UpgradedShine").gameObject.activeSelf || Castle2.transform.Find("Level5LeftButton").Find("UpgradedShine").gameObject.activeSelf)
                            yield break;
                        Castle2.transform.Find("Level5LeftButton").Find("UpgradedShine").gameObject.SetActive(true);
                        Castle2.transform.Find("UpArrowLeftOne").gameObject.SetActive(true);
                        --userSO.SkillPoint;
                        ReloadSkillPointText();
                        break;
                    case 4:
                        if (userSO.Level < 5 || !Castle2.transform.Find("Level3RightButton").Find("UpgradedShine").gameObject.activeSelf || Castle2.transform.Find("Level5RightButton").Find("UpgradedShine").gameObject.activeSelf)
                            yield break;
                        Castle2.transform.Find("Level5RightButton").Find("UpgradedShine").gameObject.SetActive(true);
                        Castle2.transform.Find("UpArrowRightOne").gameObject.SetActive(true);
                        --userSO.SkillPoint;
                        ReloadSkillPointText();
                        break;
                }
                break;
        }
        _cannonSO[_currentCannonID]._unlockedUpgrades[upgrade] = 1;
        FirebaseManager.instance.SaveSO(userSO);
        FirebaseManager.instance.SaveSO(_cannonSO[_currentCannonID]);
        isLoaded = false;
    }

    public async void ResetAll(int ID) {

        await FirebaseManager.instance.LoadSO(userSO).ContinueWith(task => isLoaded = true);


        for (int i=0; i < _cannonSO[ID]._unlockedUpgrades.Length; i++)
            _cannonSO[ID]._unlockedUpgrades[i] = 0;
        switch (_currentCannonID) {
            case 0:
                if (Castle0.transform.Find("Level1Button").Find("UpgradedShine").gameObject.activeSelf) {
                    ++userSO.SkillPoint;
                    Castle0.transform.Find("Level1Button").Find("UpgradedShine").gameObject.SetActive(false);
                }
                if (Castle0.transform.Find("Level3LeftButton").Find("UpgradedShine").gameObject.activeSelf) {
                    ++userSO.SkillPoint;
                    Castle0.transform.Find("Level3LeftButton").Find("UpgradedShine").gameObject.SetActive(false);
                }
                if (Castle0.transform.Find("Level3RightButton").Find("UpgradedShine").gameObject.activeSelf) {
                    ++userSO.SkillPoint;
                    Castle0.transform.Find("Level3RightButton").Find("UpgradedShine").gameObject.SetActive(false);
                }
                if (Castle0.transform.Find("Level5LeftButton").Find("UpgradedShine").gameObject.activeSelf) {
                    ++userSO.SkillPoint;
                    Castle0.transform.Find("Level5LeftButton").Find("UpgradedShine").gameObject.SetActive(false);
                }
                if (Castle0.transform.Find("Level5RightButton").Find("UpgradedShine").gameObject.activeSelf) {
                    ++userSO.SkillPoint;
                    Castle0.transform.Find("Level5RightButton").Find("UpgradedShine").gameObject.SetActive(false);
                }
                Castle0.transform.Find("LeftUpArrow").gameObject.SetActive(false);
                Castle0.transform.Find("RightUpArrow").gameObject.SetActive(false);
                Castle0.transform.Find("UpArrowLeftOne").gameObject.SetActive(false);
                Castle0.transform.Find("UpArrowRightOne").gameObject.SetActive(false);
                break;
            case 1:
                if (Castle1.transform.Find("Level1Button").Find("UpgradedShine").gameObject.activeSelf) {
                    ++userSO.SkillPoint;
                    Castle1.transform.Find("Level1Button").Find("UpgradedShine").gameObject.SetActive(false);
                }
                if (Castle1.transform.Find("Level3LeftButton").Find("UpgradedShine").gameObject.activeSelf) {
                    ++userSO.SkillPoint;
                    Castle1.transform.Find("Level3LeftButton").Find("UpgradedShine").gameObject.SetActive(false);
                }
                if (Castle1.transform.Find("Level3RightButton").Find("UpgradedShine").gameObject.activeSelf) {
                    ++userSO.SkillPoint;
                    Castle1.transform.Find("Level3RightButton").Find("UpgradedShine").gameObject.SetActive(false);
                }
                if (Castle1.transform.Find("Level5LeftButton").Find("UpgradedShine").gameObject.activeSelf) {
                    ++userSO.SkillPoint;
                    Castle1.transform.Find("Level5LeftButton").Find("UpgradedShine").gameObject.SetActive(false);
                }
                if (Castle1.transform.Find("Level5RightButton").Find("UpgradedShine").gameObject.activeSelf) {
                    ++userSO.SkillPoint;
                    Castle1.transform.Find("Level5RightButton").Find("UpgradedShine").gameObject.SetActive(false);
                }
                Castle1.transform.Find("LeftUpArrow").gameObject.SetActive(false);
                Castle1.transform.Find("RightUpArrow").gameObject.SetActive(false);
                Castle1.transform.Find("UpArrowLeftOne").gameObject.SetActive(false);
                Castle1.transform.Find("UpArrowRightOne").gameObject.SetActive(false);
                break;
            case 2:
                if (Castle2.transform.Find("Level1Button").Find("UpgradedShine").gameObject.activeSelf) {
                    ++userSO.SkillPoint;
                    Castle2.transform.Find("Level1Button").Find("UpgradedShine").gameObject.SetActive(false);
                }
                if (Castle2.transform.Find("Level3LeftButton").Find("UpgradedShine").gameObject.activeSelf) {
                    ++userSO.SkillPoint;
                    Castle2.transform.Find("Level3LeftButton").Find("UpgradedShine").gameObject.SetActive(false);
                }
                if (Castle2.transform.Find("Level3RightButton").Find("UpgradedShine").gameObject.activeSelf) {
                    ++userSO.SkillPoint;
                    Castle2.transform.Find("Level3RightButton").Find("UpgradedShine").gameObject.SetActive(false);
                }
                if (Castle2.transform.Find("Level5LeftButton").Find("UpgradedShine").gameObject.activeSelf) {
                    ++userSO.SkillPoint;
                    Castle2.transform.Find("Level5LeftButton").Find("UpgradedShine").gameObject.SetActive(false);
                }
                if (Castle2.transform.Find("Level5RightButton").Find("UpgradedShine").gameObject.activeSelf) {
                    ++userSO.SkillPoint;
                    Castle2.transform.Find("Level5RightButton").Find("UpgradedShine").gameObject.SetActive(false);
                }
                Castle2.transform.Find("LeftUpArrow").gameObject.SetActive(false);
                Castle2.transform.Find("RightUpArrow").gameObject.SetActive(false);
                Castle2.transform.Find("UpArrowLeftOne").gameObject.SetActive(false);
                Castle2.transform.Find("UpArrowRightOne").gameObject.SetActive(false);
                break;
        }

        ReloadSkillPointText();
        FirebaseManager.instance.SaveSO(userSO);
        FirebaseManager.instance.SaveSO(_cannonSO[ID]);
    }

    private void ReloadSkillPointText() => skillPointText.text = userSO.SkillPoint.ToString() + " SKILL POINTS REMAINING";
}
