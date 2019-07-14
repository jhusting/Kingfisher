using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonPowerUpgrade : Upgrade
{

    public float powerPerLevel = 1f;
    float currentBonus = 0;

    public override void SetUpgradeLevel(int i)
    {
        playerController.spearMaxDistance -= currentBonus;

        float newBonus = i * powerPerLevel;

        playerController.spearMaxDistance += newBonus;

        currentBonus = newBonus;

        base.SetUpgradeLevel(i);
    }

    public override string GetDescription()
    {
        return "Increases harpoon power.";
    }
}
