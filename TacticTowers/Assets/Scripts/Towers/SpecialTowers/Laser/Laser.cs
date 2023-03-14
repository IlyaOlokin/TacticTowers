using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Laser : Tower
{
    [SerializeField] private GameObject laserBim;
    [SerializeField] private GameObject shootPoint;

    private float heatCount = 0;
    private float coolTimer;
    private List<GameObject> currentEnemies = new List<GameObject>() {null, null};
    private List<GameObject> extraCurrentEnemies = new List<GameObject>() {null, null, null, null};
    private List<GameObject> activeLasers = new List<GameObject>() {null, null};
    private List<List<GameObject>> extraActiveLasers = new List<List<GameObject>>() {new List<GameObject>() {null, null}, new List<GameObject>() {null, null}};
    private List<bool> laserSound = new List<bool>() {false, false};

    [SerializeField] public int maxHeat;
    [SerializeField] public float maxHeatMultiplier;
    [SerializeField] public float multiplierPerHeatStack;
    [SerializeField] public float multiplierPerHeatStackMultiplier;
    [SerializeField] public float coolDelay;
    [SerializeField] public float coolDelayMultiplier;
    [SerializeField] private ContactFilter2D contactFilter;

    [NonSerialized] public bool shooting;
    private DamageType damageType = DamageType.Fire;

    [NonSerialized] public bool hasSecondBeamUpgrade;
    
    [NonSerialized] public bool hasBranchingBeamUpgrade;
    [SerializeField] private int extraLaserCount = 2;
    [SerializeField] private float extraLaserDamageMultiplier;

    private void Start() => audioSrc = GetComponent<AudioSource>();

    private new void Update()
    {
        base.Update();

        if (coolTimer < 0)
        {
            if (heatCount > 0) heatCount -= Time.deltaTime * 2;
            else heatCount = 0;
        }
        else coolTimer -= Time.deltaTime;
    }
    
    protected override void Shoot(GameObject enemy)
    {
        LaserShoot(enemy, 0);
        if (hasSecondBeamUpgrade && heatCount >= maxHeat * maxHeatMultiplier) 
            LaserShoot(FindTarget(new List<GameObject>(){enemy}.Union(extraCurrentEnemies)), 1);
        if (enemy == null) return;
        LootAtTarget(MiddleEnemyPoint());
        DealDamage();
    }

    private void LaserShoot(GameObject target, int i)
    {
        if (target == null)
        {
            Destroy(activeLasers[i]);
            DeactivateLaserSound(i);
            currentEnemies[i] = null;
            if (hasBranchingBeamUpgrade)
                for (int j = 0; j < extraLaserCount; j++)
                    DeactivateExtraLasers(i, j);
            return;
        }

        if (heatCount < maxHeat * maxHeatMultiplier) heatCount += Time.deltaTime;
        if (activeLasers[i] != null) activeLasers[i].GetComponent<LaserBeam>().IncreaseWidth(heatCount);
        
        if (target != currentEnemies[i])
        {
            if (hasBranchingBeamUpgrade)
                for (int j = 0; j < extraLaserCount; j++)
                    DeactivateExtraLasers(i, j);
            Destroy(activeLasers[i]);
            DeactivateLaserSound(i);
            activeLasers[i] = Instantiate(laserBim, transform.position, towerCanon.transform.rotation);
            activeLasers[i].GetComponent<LaserBeam>().target = target;
            activeLasers[i].GetComponent<LaserBeam>().origin = gameObject;
            currentEnemies[i] = target;
            ActivateLaserSound(i);
        }
        
        if (hasBranchingBeamUpgrade)
        {
            for (int j = 0; j < extraLaserCount; j++)
            {
                var enemyToIgnore = currentEnemies.Union(extraCurrentEnemies).ToList();
                enemyToIgnore.Remove(extraCurrentEnemies[i * 2 + j]);
                var closetEnemy = FindClosetEnemy(target.transform.position, enemyToIgnore, 1f);

                if (CheckWallCollision(transform.position, target.transform.position, GetShootDistance(), false) is
                    null)
                {
                    ExtraLaserShoot(target, closetEnemy, i, j);
                }
            }
        }
    }
    
    private void ExtraLaserShoot(GameObject origin, GameObject target, int parentLaserIndex, int extraLaserIndex)
    {
        if (target == null || origin == null)
        {
            DeactivateExtraLasers(parentLaserIndex, extraLaserIndex);
            return;
        }
        
        if (extraActiveLasers[parentLaserIndex][extraLaserIndex] != null) extraActiveLasers[parentLaserIndex][extraLaserIndex].GetComponent<LaserBeam>().IncreaseWidth(heatCount);
        
        if (target != extraCurrentEnemies[parentLaserIndex * 2 + extraLaserIndex])
        {
            Destroy(extraActiveLasers[parentLaserIndex][extraLaserIndex]);
            extraActiveLasers[parentLaserIndex][extraLaserIndex] = Instantiate(laserBim, transform.position, towerCanon.transform.rotation);
            extraActiveLasers[parentLaserIndex][extraLaserIndex].GetComponent<LaserBeam>().target = target;
            extraActiveLasers[parentLaserIndex][extraLaserIndex].GetComponent<LaserBeam>().origin = origin;
            extraCurrentEnemies[parentLaserIndex * 2 + extraLaserIndex] = target;
        }
    }

    private void DeactivateExtraLasers(int parentLaserIndex, int extraLaserIndex)
    {
        Destroy(extraActiveLasers[parentLaserIndex][extraLaserIndex]);
        extraCurrentEnemies[parentLaserIndex * 2 + extraLaserIndex] = null;
    }

    private void DealDamage()
    {
        bool needToDealDamage = false;
        foreach (var enemy in currentEnemies)
        {
            if (enemy != null)
            {
                needToDealDamage = true;
                break;
            }
        }
        if (!needToDealDamage) return;
        
        if (shootDelayTimer <= 0)
        {
            shootDelayTimer = 1f / GetAttackSpeed();
            coolTimer = coolDelay * coolDelayMultiplier;

            DealDamageToGroup(currentEnemies, activeLasers);
            var extraLasers = extraActiveLasers[0].Union(extraActiveLasers[1]).ToList();
            DealDamageToGroup(extraCurrentEnemies, extraLasers,  extraLaserDamageMultiplier);
        }
    }

    private void DealDamageToGroup(List<GameObject> enemies, List<GameObject> lasers, float damageMultiplier = 1f)
    {
        for (var i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] == null) continue;
            if (CheckWallCollision(transform.position, enemies[i].transform.position, GetShootDistance(), false) is
                null)
            {
                if (enemies[i].GetComponent<Enemy>().TakeDamage(
                    GetDmg() * (1 + Mathf.Floor(heatCount) * multiplierPerHeatStack *
                        multiplierPerHeatStackMultiplier) * damageMultiplier,
                    damageType, transform.position))
                {
                    Destroy(lasers[i]);
                    shooting = false;
                }
            }
        }
    }

    private void ActivateLaserSound(int i)
    {
        laserSound[i] = true;
        HandleLaserSound();
    }
    
    private void DeactivateLaserSound(int i)
    {
        laserSound[i] = false;
        HandleLaserSound();
    }

    private void HandleLaserSound()
    {
        if (laserSound.Contains(true))
        {
            if (!audioSrc.isPlaying) audioSrc.Play();
        }
        else audioSrc.Stop();
    }

    private Vector3 MiddleEnemyPoint()
    {
        Vector3 posSum = Vector3.zero;
        int enemiesCount = 0;
        foreach (var enemy in currentEnemies)
            if (enemy != null)
            {
                posSum += enemy.transform.position;
                enemiesCount++;
            }
        
        return posSum / enemiesCount;
    }
}