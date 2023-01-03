using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ElectricBotScript : MonoBehaviour {

    [SerializeField] private Rigidbody projectile;
    [SerializeField] private GameObject cursor;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Timer timer;
    [SerializeField] private float flightTime = 1f;
    [SerializeField] private int _defaultDuration = 1;
    [SerializeField] private CanonSO towerUpgrades;
    private Transform target = null;
    private List<GameObject> Enemies;
    private bool canFire = false;
    private Vector3 vector;

    // Upgrade
    private int[] _upgrades;
    //private bool SpeedActive = false;
    //private bool SecondBallActive = false;
    //private bool ElectricTowerActive = false;
    //public float _durationUpgradePercentage = .95f;
    //public float _flightTimeUpgrade = .95f;

    private void OnEnable() {
        EventManager.OnAddTarget += AddTarget;
        EventManager.OnDeleteChibi += DeleteChibi;
    }

    private void OnDisable() {
        EventManager.OnAddTarget -= AddTarget;
        EventManager.OnDeleteChibi -= DeleteChibi;
    }

    void Start() {
        Enemies = new List<GameObject>();
        timer.SetDurationMultiplier(towerUpgrades.cooldownSeconds);
        //CheckforUpgrades();
    }

    //private void CheckforUpgrades() {

    //    for (int i = 0; i < _upgrades.Length; i++) {
    //        if (_upgrades[i] == 1) {
    //            switch (i) {
    //                case 0:
    //                    EventManager.SetJumpRange();
    //                    break;
    //                case 1:
    //                    SecondBallActive = true;
    //                    break;
    //                case 2:
    //                    timer.SetDurationMultiplier(_defaultDuration - _durationUpgradePercentage * _defaultDuration);
    //                    //EventManager.SetTimerDuration(id, (_defaultDuration - _durationUpgradePercentage * _defaultDuration));
    //                    break;
    //                case 3:
    //                    ElectricTowerActive = true;
    //                    break;
    //                case 4:
    //                    SpeedActive = true;
    //                    break;
    //            }
    //        }
    //    }
    //}



    private void Update() {
        if (!canFire)
            return;
        GameObject temp = ClosestTarget();
        if (temp == null)
            return;
        target = temp.transform;
        Vector3 dir = target.position - shootPoint.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = lookRotation.eulerAngles;
        transform.DOKill(); // this seems like a expensive call
        transform.DORotate(new Vector3(0f, rotation.y, 0f), 1f);
        LaunchProjectile();
        CanFire();
        
    }

    private void AddTarget(GameObject Chibi) => Enemies.Add(Chibi);

    private void DeleteChibi(GameObject Chibi) => Enemies.Remove(Chibi);

    private GameObject ClosestTarget() {  // gameobject döndürüyo, indexe dönülebilir hatalara göre
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        for (int i = 0; i < Enemies.Count; ++i) {
            if (Enemies[i] == null) // düþman chibisi mi kontrolü de yap
                continue;
            float distanceToEnemy = Vector3.Distance(transform.position, Enemies[i].transform.position);
            if (distanceToEnemy < shortestDistance) {
                shortestDistance = distanceToEnemy;
                nearestEnemy = Enemies[i];
            }
        }
        return nearestEnemy;
    }
    public void EnableFire() => canFire = true;

    public void CanFire() {
        Fire();
        timer.Fire();
        canFire = false;
        target = null;
    }
    private void Fire() {
        Rigidbody obj = Instantiate(projectile, shootPoint.position, Quaternion.Euler(transform.rotation.eulerAngles - vector));
        obj.velocity = vector;
        cursor.SetActive(false);
    }

    Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float time) {
        Vector3 distance = target - origin;
        return (distance.magnitude / time) * distance.normalized;
    }

    void LaunchProjectile() {
        cursor.SetActive(true);
        cursor.transform.DOMove(target.position + Vector3.up * 0.1f, 0.2f);
        Vector3 vo = CalculateVelocity(target.position, shootPoint.position, flightTime);

        transform.rotation = Quaternion.LookRotation(vo);
        vector = vo;
    }

}
