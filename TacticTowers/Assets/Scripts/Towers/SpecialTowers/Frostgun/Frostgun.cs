using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frostgun : Tower
{
    public float freezeTime;
    [SerializeField] private float freezeStacksPerHit;
    public float freezeStacksPerHitMultiplier;
    [SerializeField] private GameObject frostBox;

    [SerializeField] private int freezeStacksNeeded;
    private GameObject currentEnemy;
    private GameObject activeFrostBox;
    [SerializeField] private GameObject frostEffect;
    
    [NonSerialized] public bool shooting;
    [SerializeField] private Transform frostStartPos;



    private void Start()
    {
        AudioManager.Instance.frostguns.Add(this);

    }

    void Update()
    {
        base.Update();
    }
    
    protected override void Shoot(GameObject enemy)
    {
        if (enemy == null)
        {
            DestroyFrostBox();
            currentEnemy = null;
            frostEffect.SetActive(false);
            shooting = false;
            
            return;
        }
        LootAtTarget(enemy);

        if (enemy != currentEnemy)
        {
            DestroyFrostBox();
            shooting = false;

        }
        
        if (shootDelayTimer <= 0)
        {
            if (enemy != currentEnemy)
            {
                activeFrostBox = Instantiate(frostBox, transform.position, towerCanon.transform.rotation);
                
                activeFrostBox.GetComponent<FrostBox>().dmg = GetDmg();
                activeFrostBox.GetComponent<FrostBox>().attackSpeed = GetAttackSpeed();
                activeFrostBox.GetComponent<FrostBox>().freezeTime = freezeTime;
                activeFrostBox.GetComponent<FrostBox>().freezeStacksPerHit = GetFreezeStacksPerHit();
                activeFrostBox.GetComponent<FrostBox>().freezeStacksNeeded = freezeStacksNeeded;
                
                //activeFrostBox.GetComponent<FrostBox>().frostStartPos = transform.position;
                //activeFrostBox.transform.localScale = new Vector3(activeFrostBox.transform.localScale.x, GetShootDistance());
                var frostDistance = GetFrostDistance(enemy);
                activeFrostBox.transform.localScale = new Vector3(activeFrostBox.transform.localScale.x, activeFrostBox.transform.localScale.x * 2.5f * frostDistance / 3f);
                activeFrostBox.transform.position = ((transform.up * frostDistance + transform.position) + frostStartPos.position) / 2f;
                currentEnemy = enemy;
                
                shooting = true;
                
                frostEffect.SetActive(true);
            }
            
            shootDelayTimer = 1f / GetAttackSpeed();
        }

        if (activeFrostBox != null)
        {
            var frostDistance = GetFrostDistance(enemy);
            activeFrostBox.transform.position = (towerCanon.transform.up * frostDistance + frostStartPos.position + transform.position) / 2f;
            activeFrostBox.transform.localScale = new Vector3(activeFrostBox.transform.localScale.x, activeFrostBox.transform.localScale.x * 2.5f * frostDistance / 3f);

            activeFrostBox.transform.rotation = towerCanon.transform.rotation;
        }
    }
    
    private float GetFrostDistance(GameObject enemy)
    {
        if(CheckWallCollision(transform.position, enemy.transform.position, true) != null)
        {
            var fireDistance = Vector2.Distance(transform.position,
                GetRayImpactPoint(transform.position, enemy.transform.position, true));
            return fireDistance;
        }
        else
        {
            return GetShootDistance();
        }
    }

    private void DestroyFrostBox()
    {
        if (activeFrostBox != null) activeFrostBox.GetComponent<FrostBox>().DestroySelf(1f);
        activeFrostBox = null;
    }

    private float GetFreezeStacksPerHit()
    {
        return freezeStacksPerHit * freezeStacksPerHitMultiplier;
    }
}
