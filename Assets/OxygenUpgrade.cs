using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenUpgrade : Upgrade
{
    public float bonusPerLevel = 30f;

    float currentBonus = 0;

    public override void SetUpgradeLevel(int i)
    {

        playerController.AddMaxOxygen(-1 * currentBonus);

        float newBonus = bonusPerLevel * i;
        playerController.AddMaxOxygen(newBonus);

        currentBonus = newBonus;
        

        base.SetUpgradeLevel(i);
    }

    public override string GetDescription()
    {
        return "Increases the amount of oxygen available";
    }
}
