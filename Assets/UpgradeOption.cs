using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UpgradeOption : MonoBehaviour
{

    public Image upgradeIcon;
    public Text upgradeName;
    public Text upgradeDescription;
    public Text upgradeCost;

    private Upgrade targetUpgrade;

    public void InitializeWithUpgrade(Upgrade upgrade)
    {
        targetUpgrade = upgrade;

        //upgardeIcon.sprite = upgrade.GetIcon()
        upgradeName.text = upgrade.upgradeName + "\n (Lv. " + upgrade.currentLevel.ToString() + ")";
        upgradeDescription.text = upgrade.GetDescription();
        upgradeCost.text = "$" + upgrade.GetCostToUpgrade().ToString();

        Button button = GetComponent<Button>();

        if (!upgrade.CanAffordUpgrade())
        {
            button.interactable = false;
        }
        button.onClick.AddListener(OnUpgradeClicked);
       
    }

    void OnUpgradeClicked()
    {
        targetUpgrade.TriggerUpgrade();
    }
}
