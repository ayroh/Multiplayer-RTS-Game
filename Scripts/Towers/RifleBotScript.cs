using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RifleBotScript : MonoBehaviour {
    [SerializeField] private Rigidbody projectile;
    [SerializeField] private GameObject cursor;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Timer timer;
    [SerializeField] private float flightTime = 1f;
    [SerializeField] private int _defaultDuration = 40;
    [SerializeField] private CanonSO towerUpgrades;

    private Transform target = null;
    private List<GameObject> Enemies;
    private bool canFire = false;
    private Vector3 vector;

    // Upgrades
    //[SerializeField] private CanonSO _defaultCanonUpgrades;
    //private int[] _upgrades;
    //private bool grenade = false;
    //private bool grenadeSize = false;
    //[SerializeField] private float _durationUpgradePercentage = .95f;
    //[SerializeField] private float _flightTimeUpgrade = .95f;
    //private bool explode = false;

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
        //_upgrades = _defaultCanonUpgrades._unlockedUpgrades;
        //CheckforUpgrades();
        //InvokeRepeating("UpdateTarget", 0f, 0.5f);
        //lineVisual.positionCount = lineSegment + 1;
        //StartCoroutine(DelayForShoot());
    }

    //private void CheckforUpgrades() {
    //    for (int i = 0; i < _upgrades.Length; i++) {
    //        if (_upgrades[i] == 1) {
    //            switch (i) {
    //                case 0:
    //                    //Debug.Log("Rifle1");
    //                    flightTime = flightTime - _flightTimeUpgrade * flightTime;
    //                    break;
    //                case 1:
    //                    //Debug.Log("Grenade launch");
    //                    grenade = true;
    //                    break;
    //                case 2:
    //                    timer.SetDurationMultiplier(_defaultDuration - _durationUpgradePercentage * _defaultDuration);
    //                    //EventManager.SetTimerDuration(id, (_defaultDuration - _durationUpgradePercentage * _defaultDuration));
    //                    break;
    //                case 3:
    //                    //Debug.Log("Size increase");
    //                    grenadeSize = true;
    //                    break;
    //                case 4:
    //                    //Debug.Log("Rifle5");
    //                    explode = true;
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
        Vector3 dir = target.position - transform.position;
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

    Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float time) {
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

    void LaunchProjectile() {
        cursor.SetActive(true);
        cursor.transform.DOMove(target.position + Vector3.up * 0.1f, 0.2f);
        Vector3 vo = CalculateVelocity(target.position, shootPoint.position, flightTime);

        transform.rotation = Quaternion.LookRotation(vo);
        vector = vo;
    }

    private void Fire() {
        Rigidbody obj = Instantiate(projectile, shootPoint.position, Quaternion.identity);
        obj.velocity = vector;
        cursor.SetActive(false);
    }

}
