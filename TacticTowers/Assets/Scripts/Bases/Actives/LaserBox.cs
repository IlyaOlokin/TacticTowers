using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBox : MonoBehaviour
{
    [NonSerialized] private List<Enemy> enemies = new List<Enemy>();
    [SerializeField] private float damage;
    [SerializeField] private float periodBetweenDmg;
    [SerializeField] private float speed;
    [SerializeField] public float duration;
    private float period;
    private AudioSource audioSrc;
    
    private void Start()
    {
        period = periodBetweenDmg;
        audioSrc = GetComponent<AudioSource>();
        audioSrc.Play();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<Enemy>();
            if (!enemies.Contains(enemy))
            {
                enemies.Add(enemy);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<Enemy>();
            if (enemies.Contains(enemy))
            {
                enemies.Remove(enemy);
            }
        }
    }


    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, GetMousePosition(), speed * Time.deltaTime);

        if (periodBetweenDmg > 0) periodBetweenDmg -= Time.deltaTime;

        if (periodBetweenDmg <= 0)
        {
            PeriodDamage();
            periodBetweenDmg = period;
        }
        duration -= Time.deltaTime;
        if (duration < 0)
        {
            Destroy(gameObject);
        }
    }

    private void PeriodDamage()
    {
        for (var i = 0; i < enemies.Count; i++)
        {
            var enemy = enemies[i];
            if (enemy != null)
            {
                enemy.TakeDamage(damage, DamageType.Fire, transform.position);
            }
        }
    }

    private Vector3 GetMousePosition()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        return mousePosition;
    }
}
