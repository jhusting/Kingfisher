using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonPowerUpgrade : Upgrade
{
    float currentBonus = 0;

    public override void SetUpgradeLevel(int i)
    {
        playerController.spearMaxDistance -= currentBonus;

        float newBonus = i * 1f;
        playerController.spearMaxDistance += newBonus;

        currentBonus = newBonus;

        base.SetUpgradeLevel(i);
    }

    public override string GetDescription()
    {
        return "Increases harpoon power by an additional 2";
    }
}
