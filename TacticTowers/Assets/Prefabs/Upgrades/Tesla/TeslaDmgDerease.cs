using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaDmgDerease : CommonUpgrade
{
    public override void Execute(Tower tower)
    {
        tower.transform.GetComponent<Tesla>().dmgDecreaseMultiplier += actualBonus;
    }
}