using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CannonBotScript : MonoBehaviour
{
    [SerializeField] private GameObject shoutParticle;
    [SerializeField] private Rigidbody projectile;
    [SerializeField] private GameObject cursor;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Timer timer;
    [SerializeField] private float flightTime = 1f;
    [SerializeField] private CanonSO towerUpgrades;

    private List<GameObject> Enemies;
    private Transform target = null;
    private bool canFire = false;
    private Vector3 vector;



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
    }

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
            if (Enemies[i] == null) // düşman chibisi mi kontrolü de yap
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
        Instantiate(shoutParticle, shootPoint.position, Quaternion.identity);
        Rigidbody obj = Instantiate(projectile, shootPoint.position, Quaternion.identity);
        obj.velocity = vector;
        cursor.SetActive(false);
    }

}
