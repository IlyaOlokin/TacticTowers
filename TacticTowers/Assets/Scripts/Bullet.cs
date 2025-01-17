using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour
{
     [NonSerialized] public float Dmg;
     [NonSerialized] public float Speed;
     [NonSerialized] public List<GameObject> enemiesToIgnore;
     [NonSerialized] public Vector3 departurePos;
     [NonSerialized] public bool hasPenetrationUpgrade;
     [NonSerialized] public bool isCritical;
     [NonSerialized] public float penetrationDamageMultiplier;
     [NonSerialized] public int penetrationsCount;
     protected DamageType damageType = DamageType.Normal;
     protected int penetrationsLeft = 0;
    
     private Rigidbody2D rb;
     
     [SerializeField] private GameObject visualEffect;

     private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * Speed;
        foreach (var enemy in enemiesToIgnore)
        {
            if (enemy is null) continue;
            Physics2D.IgnoreCollision(enemy.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }

        if (hasPenetrationUpgrade)
            penetrationsLeft = penetrationsCount;
    }
     
     public void ActivateVisualEffect()
     {
         visualEffect.SetActive(true);
     }
     
     private void OnTriggerEnter2D(Collider2D other)
     {
         if (other.gameObject.CompareTag("Enemy"))
         {
             OnEnemyHit(other);
         }
         else if (other.gameObject.CompareTag("Wall"))
         {
             Destroy(gameObject);
         }
     }

     protected virtual void OnEnemyHit(Collider2D other)
     {
         other.gameObject.GetComponent<Enemy>().TakeDamage(Dmg, damageType, departurePos, isCritical);

         if (penetrationsLeft == 0)
         {
             Destroy(gameObject);
             return;
         }

         GetPenetrationEffect();
     }

     protected void GetPenetrationEffect()
     {
         penetrationsLeft--;
         Dmg *= penetrationDamageMultiplier;
     }
}