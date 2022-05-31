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
    [NonSerialized] public Vector3 flameStartPos;
    private float dmgDelayTimer;
    [NonSerialized] public ParticleSystem ps;

    [SerializeField] private GameObject fire;

    private void Start()
    {
        ps = transform.GetChild(0).GetComponent<ParticleSystem>();
        ps.transform.position = flameStartPos;
        var mainModule = ps.main;
        mainModule.startLifetime =transform.localScale.y / 3 * 0.85f;
    }

    protected void Update()
    {
        if (dmgDelayTimer > 0) dmgDelayTimer -= Time.deltaTime;
        
        if (dmgDelayTimer <= 0) DealDamage();
    }

    private void DealDamage()
    {
        for (var index = 0; index < enemiesInside.Count; index++)
        {
            var enemy = enemiesInside[index];
            enemy.TakeDamage(dmg);
            SetOnFire(enemy.gameObject);
            
        }

        dmgDelayTimer = 1f / attackSpeed;
    }

    private void SetOnFire(GameObject enemy)
    {
        if (!enemy.GetComponent<Fire>())
        {
            enemy.transform.gameObject.AddComponent<Fire>();
            enemy.GetComponent<Fire>().burnDmg = burnDmg;
            enemy.GetComponent<Fire>().burnTime = burnTime;
            enemy.GetComponent<Fire>().fire = fire;
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