using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Web : MonoBehaviour
{
    public static int WebsReachedTower;
    [SerializeField] private float speed;
    [SerializeField] private float disarmDuration;
    [NonSerialized] public Vector3 endPos;
    private bool reachedEndPos = false;
    private CircleCollider2D circleCollider2D;
    [SerializeField] private Sprite webSprite;
    public event Action onTowerReached;
    
    private void Start()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, endPos, speed * Time.deltaTime);
        
        Vector3 dir = transform.position - endPos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle + 90);
        
        if (transform.position.Equals(endPos) && !reachedEndPos)
        {
            OnTargetReached();
        }
    }

    private void OnTargetReached()
    {
        reachedEndPos = true;
        circleCollider2D.enabled = true;
        GetComponent<SpriteRenderer>().sprite = webSprite;
        var towers = GameObject.FindGameObjectsWithTag("TowerInstance");
        bool isTowerReached = false;
        
        foreach (var tower in towers)
        {
            var towerComp = tower.GetComponent<Tower>();
            
            if (Vector3.Distance(tower.transform.position, transform.position) <
                (transform.localScale.x + tower.transform.localScale.x) / 2f
                && !towerComp.isDragging)
            {
                towerComp.Disarm(disarmDuration);
                if (!isTowerReached) WebsReachedTower++;
                isTowerReached = true;
                onTowerReached?.Invoke();
            }
        }

        if (!isTowerReached) WebsReachedTower--;

        StartCoroutine(Destroy(5));
    }

    private IEnumerator Destroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        circleCollider2D.radius = 0f;
        Destroy(gameObject);
    }
}
