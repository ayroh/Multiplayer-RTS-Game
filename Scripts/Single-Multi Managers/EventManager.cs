using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    public delegate void UIPlayyyzz();
    public static event UIPlayyyzz OnLevelEndTriggeredAction;
    public static void LevelEndTriggered()
    {
        OnLevelEndTriggeredAction?.Invoke();
    }

    public delegate void Fire(int id);
    public static event Fire OnFireTriggeredAction;
    public static void FireTriggered(int id)
    {
        OnFireTriggeredAction?.Invoke(id);
    }

    
    public delegate void LevelStatue(bool isPlayerWin);
    public static event LevelStatue OnLevelStatueAction;
    public static void LevelStatueAction(bool isPlayerWin)
    {
        OnLevelStatueAction?.Invoke(isPlayerWin);
    }
    
    public delegate void TimerDurationSet(int id, float duration);
    public static event TimerDurationSet OnMultiplierSet;
    public static void SetTimerDuration(int id, float duration) {
        OnMultiplierSet?.Invoke(id,duration);
    }

    #region grenade_rifle
    public delegate void GrenadeUpgrade(bool unlocked);
    public static event GrenadeUpgrade OnGrenadeUpgrade;

    public static void SetGrenadeUpgrade(bool unlocked) {
        OnGrenadeUpgrade?.Invoke(unlocked);
    }


    public delegate void GrenadeSizeUpgrade(bool unlocked);
    public static event GrenadeSizeUpgrade OnGrenadeSizeUpgrade;

    public static void SetGrenadeSizeUpgrade(bool unlocked) {
        OnGrenadeSizeUpgrade?.Invoke(unlocked);
    }
    #endregion

    #region explode
    public delegate void ExplodeUpgrade(bool unlocked);
    public static event ExplodeUpgrade OnExplodeUpgrade;

    public static void SetExplodeUpgrade(bool unlocked) {
        OnExplodeUpgrade?.Invoke(unlocked);
    }
    #endregion

    #region jump_range
    public delegate void JumpRangeUpgrade();
    public static event JumpRangeUpgrade OnJumpRangeUpgrade;

    public static void SetJumpRange() {
        OnJumpRangeUpgrade?.Invoke();
    }

    public delegate void CardDrop();
    public static event CardDrop OnDroppedCard;
    public static void DroppedCard() {
        OnDroppedCard?.Invoke();
    }

    public delegate void CardRemove(string CardName);
    public static event CardRemove OnRemoveCard;
    public static void RemoveCard(string CardName) {
        OnRemoveCard?.Invoke(CardName);
    }


    public delegate void ClickCard(DragDropScript CardScript);
    public static event ClickCard OnCardClicked;
    public static void CardClicked(DragDropScript CardScript) {
        OnCardClicked?.Invoke(CardScript);
    }
    
    
    public delegate void CardHold();
    public static event CardHold OnHoldingCard;
    public static void HoldingCard() {
        OnHoldingCard?.Invoke();
    }
    #endregion

    #region speed_upgrade
    public delegate void SpeedUpgrade(bool unlocked);
    public static event SpeedUpgrade OnSpeedUpgrade;

    public static void SetSpeedUpgrade(bool unlocked) {
        OnSpeedUpgrade?.Invoke(unlocked);
    }
    #endregion

    #region ElectricTower
    public delegate void ElectricTowerUpgrade(bool unlocked);
    public static event ElectricTowerUpgrade OnElectricTowerUpgrade;

    public static void SetElectricTowerUpgrade(bool unlocked) {
        OnElectricTowerUpgrade?.Invoke(unlocked);
    }
    #endregion

    #region UIEvents

    public delegate void MainMenu(bool unlocked);
    public static event MainMenu OnGameEnter;

    public static void StartMainMenu(bool unlocked) {
        OnGameEnter?.Invoke(unlocked);
    }

    public delegate void SelectedMenu(int  selecetd);
    public static event SelectedMenu OnMenuSelect;

    public static void SelectMenu(int selecetd) {
        OnMenuSelect?.Invoke(selecetd);
    }

    public delegate void GoBackToHomePage();
    public static event GoBackToHomePage OnHomePage;

    public static void SetHomePage() {
        OnHomePage?.Invoke();
    }

    #endregion

    #region obstacle_placement
    public delegate void SetSpawnPosition(int ID);
    public static event SetSpawnPosition OnSetSpawnPosition;

    public static void PositionSet(int ID) {
        OnSetSpawnPosition?.Invoke(ID);
    }

    public delegate void SpawnObject(int id, int obsid);
    public static event SpawnObject OnSpawnObject;

    public static void Spawn(int id, int obsid) {
        OnSpawnObject?.Invoke(id, obsid);
    }

    #endregion

    #region OpenModifyBar
    public delegate void OpenModifyBar(bool mode);
    public static event OpenModifyBar OnOpenModifyBar;

    internal static void LevelStatueAction(object isBot) {
        throw new NotImplementedException();
    }

    public static void OpenBar(bool mode) {
        OnOpenModifyBar?.Invoke(mode);
    }
    #endregion

    #region DestroyPlacedObstacles

    public delegate void DestroyObstacleChild();
    public static event DestroyObstacleChild OnDestroyObstacle;

    public static void DestroyObstacle() {
        OnDestroyObstacle?.Invoke();
    }

    #endregion

    #region WallDestroyed

    public delegate void WallDestroyed(bool value);
    public static event WallDestroyed OnWallDestroyed;

    public static void DestroyWall(bool value) {
        OnWallDestroyed?.Invoke(value);
    }
    #endregion


    public delegate void CannonStatue(int id);
    public static event CannonStatue OnCannonStatueAction;
    public static void CannonStatueAction(int id) {
        OnCannonStatueAction?.Invoke(id);
    }


    //public delegate void SongOfValorEnable();
    //public static event SongOfValorEnable OnSongOfValorEnable;
    //public static void EnableSongOfValor() {
    //    OnSongOfValorEnable?.Invoke();
    //}

    public delegate void CardCast(string CardName, int cardMana, Vector3 firstTap = default(Vector3), Vector3 secondTap = default(Vector3));
    public static event CardCast OnCardCast;
    public static void CastCard(string CardName, int cardMana, Vector3 firstTap = default(Vector3), Vector3 secondTap = default(Vector3)) {
        OnCardCast?.Invoke(CardName, cardMana, firstTap, secondTap);
    }

    //public delegate void MeteorEnable();
    //public static event MeteorEnable OnMeteorEnable;
    //public static void EnableMeteor() {
    //    OnMeteorEnable?.Invoke();
    //}

    public delegate void ChibiDelete(GameObject Chibi);
    public static event ChibiDelete OnDeleteChibi;
    public static void DeleteChibiFromEnemies(GameObject Chibi)
    {
        OnDeleteChibi?.Invoke(Chibi);
    }

    public delegate void TargetAdd(GameObject Chibi);
    public static event TargetAdd OnAddTarget;
    public static void AddTarget(GameObject Chibi) {
        OnAddTarget?.Invoke(Chibi);
    }

    public delegate void ChibisCame();
    public static event ChibisCame OnChibisCameToBridge;
    public static void ChibisCameToBridge() {
        OnChibisCameToBridge?.Invoke();
    }


    public delegate void SkillpointEnd();
    public static event SkillpointEnd OnEndSkillPointTutorial;
    public static void EndSkillPointTutorial() {
        OnEndSkillPointTutorial?.Invoke();
    }

}
