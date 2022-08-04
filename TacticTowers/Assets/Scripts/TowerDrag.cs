using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TowerDrag : MonoBehaviour
{
    public Tower tower;
    private Collider2D collider2D;
    private NavMeshObstacle navMeshObstacle;
    private Vector2 mouseOffset;

    private Vector3 pressStartPos;
    [NonSerialized] public bool dragging;
    private bool triedToDrag;
    [NonSerialized] public bool needToDrop;

    private int conflicts;
    [SerializeField] private GameObject smokeEffect;
    private int edgeSize = 20;

    private void Start()
    {
        collider2D = GetComponent<CircleCollider2D>();
        navMeshObstacle = GetComponent<NavMeshObstacle>();
    }
    
    private void Update()
    {
        if (Time.timeScale == 0)
        {
            var a = 0;
        }
        
        if (needToDrop || Time.deltaTime == 0)
        {
            TryToDrop();
        }
        
        if (dragging)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(ControlledMousePosition());
            transform.position = new Vector3(mousePos.x + mouseOffset.x, mousePos.y + mouseOffset.y);
        }
        
        if (!triedToDrag) return;
        if (!dragging)
        {
            if (Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), pressStartPos) >= 0.1f)
            {
                StartDragging();
            }
        }
    }

    private Vector3 ControlledMousePosition()
    {
        var mousePos = Input.mousePosition;

        if (mousePos.x > Screen.width - edgeSize)
        {
            mousePos.x = Screen.width - edgeSize;
        }
        else if (mousePos.x < edgeSize)
        {
            mousePos.x = edgeSize;
        }

        if (mousePos.y > Screen.height - edgeSize)
        {
            mousePos.y = Screen.height - edgeSize;
        }
        else if (mousePos.y < edgeSize)
        {
            mousePos.y = edgeSize;
        }

        return mousePos;
    }

    private void OnMouseDown()
    {
        pressStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        triedToDrag = true;
        
        mouseOffset =  transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        TryToDrop();
    }

    private void TryToDrop()
    {
        if (conflicts <= 0)
        {
            PlaceTower();
            needToDrop = false;
        }
        else
        {
            needToDrop = true;
        }
    }

    private void PlaceTower()
    {
        if (dragging)
        {
            Instantiate(smokeEffect, transform.position, Quaternion.identity);
            FindObjectOfType<AudioManager>().Play("Landing");
        }
        dragging = false;
        tower.canShoot = true;
        triedToDrag = false;
        navMeshObstacle.enabled = true;
        collider2D.isTrigger = false;
        conflicts = 0;
        
    }

    private void StartDragging()
    {
        if (IsAnyOtherTowerDragging()) return;
        dragging = true;
        tower.canShoot = false;
        triedToDrag = false;
        navMeshObstacle.enabled = false;
        collider2D.isTrigger = true;
    }

    private bool IsAnyOtherTowerDragging()
    {
        var towers = FindObjectsOfType<TowerDrag>();
        foreach (var tower in towers)
        {
            if (tower.dragging) return true;
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!dragging) return;
        var otherGameObject = other.gameObject;
        if (otherGameObject.CompareTag("Enemy") || otherGameObject.CompareTag("Base") || otherGameObject.CompareTag("Tower"))
        {
            conflicts += 1;

            for (int i = 0; i < otherGameObject.transform.childCount; i++)
            {
                if (otherGameObject.transform.GetChild(i).gameObject.CompareTag("ConflictIndicator"))
                {
                    otherGameObject.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
            

        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!dragging) return;
        var otherGameObject = other.gameObject;
        if (otherGameObject.CompareTag("Enemy") || otherGameObject.CompareTag("Base") || otherGameObject.CompareTag("Tower"))
        {
            conflicts -= 1;
            
            for (int i = 0; i < otherGameObject.transform.childCount; i++)
            {
                if (otherGameObject.transform.GetChild(i).gameObject.CompareTag("ConflictIndicator"))
                {
                    otherGameObject.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }

    protected void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Web"))
        {
            StartCoroutine(Disarm());
            Destroy(other.gameObject);
        }
    }

    IEnumerator Disarm()
    {
        tower.canShoot = false;
        yield return new WaitForSeconds(3.5f);
        tower.canShoot = true;
    }
}
