﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSpeedUpgrade : Upgrade
{

    float currentBonus = 0;

    
    public override void SetUpgradeLevel(int level)
    {
        //Undo whatever the last upgrade had done
        playerController.AddBaseMoveSpeed(-1 * currentBonus);

        //Calculate the new upgrade value
        float newBonus = level * 0.1f * 3/5f;
        playerController.AddBaseMoveSpeed(newBonus);

        //Keep track of this number so we can deduct it when the player upgrades again
        currentBonus = newBonus;


        base.SetUpgradeLevel(level);

    }

    public override string GetDescription()
    {
        return "Increase move speed by an additional 0.5";
    }
}
