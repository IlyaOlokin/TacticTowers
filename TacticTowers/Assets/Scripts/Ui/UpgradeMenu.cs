using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenu : MonoBehaviour
{
    [SerializeField] private Text towerLevel;
    [SerializeField] private Text towerLevelConst;
    [SerializeField] private Text nextUpgradeCost;
    private Animation anim;
    private Collider2D coll2D;

    private void Start()
    {
        coll2D = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        anim = GetComponent<Animation>();
        anim.Stop("UpgradeMenuAnimation");
        anim.Play("UpgradeMenuAnimation");
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (!coll2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
                DeactivateMenu();
        }
    }

    public void UpdateTexts(int level, int cost)
    {
        towerLevel.text = level.ToString();
        towerLevelConst.text = level.ToString();
        if (cost == 0) 
            nextUpgradeCost.text = "MAX!";
        else
            nextUpgradeCost.text = cost + "$";
        
    }

    private void DeactivateMenu()
    {
        gameObject.SetActive(false);
    }
}