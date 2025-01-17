using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlimeMedium : Enemy
{
    [Header("Slime Medium")] 
    [SerializeField] private GameObject slimeSmall;
    [SerializeField] private int slimeAmount;
    
    private void OnDestroy()
    {
        if(!gameObject.scene.isLoaded) 
            return;
        
        var enemyParent = GameObject.FindGameObjectWithTag("EnemyParent").transform;
        for (var i = 0; i < slimeAmount; i++)
        {
            var spawnedEnemy = Instantiate(slimeSmall, transform.position, transform.rotation, enemyParent);
            spawnedEnemy.GetComponent<Enemy>().SetCreditsDropChance(0);
        }
        
        EnemySpawner.FindEnemies();
    }
}
