using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameBox : MonoBehaviour
{
    private List<Enemy> enemiesInside = new List<Enemy>();

    [NonSerialized] public float dmg;
    [NonSerialized] public float burnDmg;
    [NonSerialized] public float burnTime;
    [NonSerialized] public float attackSpeed;
    private float dmgDelayTimer;
    private Material myMaterial;
    
    private float appearTimer;
    private float appearDelay = 2f;

    private float destroyTimer;
    private float destroyDelay;
    
    private bool needToBeDestroyed;
    private DamageType damageType = DamageType.Fire;

    private static readonly int EndFlame = Shader.PropertyToID("_EndFlame");
    private static readonly int StartFlame = Shader.PropertyToID("_StartFlame");

    private void Start()
    {
        myMaterial = GetComponent<SpriteRenderer>().material;
        myMaterial.SetFloat(StartFlame, 0);
    }

    protected void Update()
    {
        if (dmgDelayTimer > 0) dmgDelayTimer -= Time.deltaTime;
        
        if (dmgDelayTimer <= 0) DealDamage();
        if (needToBeDestroyed)
        {
            destroyTimer += Time.deltaTime;
            myMaterial.SetFloat(EndFlame, destroyTimer / destroyDelay);
        }

        if (appearTimer < appearDelay)
        {
            appearTimer += Time.deltaTime;
            myMaterial.SetFloat(StartFlame, appearTimer / appearDelay);
        }
    }

    private void DealDamage()
    {
        for (var index = 0; index < enemiesInside.Count; index++)
        {
            var enemy = enemiesInside[index];
            enemy.TakeDamage(dmg, damageType, transform.position);
            //SetOnFire(enemy.gameObject);
            enemy.TakeFire(burnDmg, burnTime);
        }

        dmgDelayTimer = 1f / attackSpeed;
    }

    public void DestroySelf(float delay)
    {
        Destroy(gameObject, delay);
        destroyDelay = delay;
        needToBeDestroyed = true; 
    }

    private void SetOnFire(GameObject enemy)
    {
        //var fire = 
        //enemy.GetComponent<Enemy>().TakeFire()
        if (!enemy.GetComponent<Fire>())
        {
            enemy.transform.gameObject.AddComponent<Fire>();
            enemy.GetComponent<Fire>().burnDmg = burnDmg;
            enemy.GetComponent<Fire>().burnTime = burnTime;
            //enemy.GetComponent<Fire>().fire = fire;
        }
        else
        {
            enemy.GetComponent<Fire>().burnTime = burnTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<Enemy>();
            if (!enemiesInside.Contains(enemy))
            {
                enemiesInside.Add(enemy);
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<Enemy>();
            if (enemiesInside.Contains(enemy))
            {
                enemiesInside.Remove(enemy);
            }
            
        }
    }
}
