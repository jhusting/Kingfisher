using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{

    public RectTransform upgradePanel;
    public UpgradeOption upgradeOptionPrefab;

    void Start()
    {
        //Heres a neat way to get around listeners needing to have the exact same signature
        //This is basically a lambda that takes in the upgrade parameter, but doesnt do anything with it. it just calls PopulateWithUpgrades
        //even tho PopulateWithUpgrades doesnt match the signature of the upgradEvent event.
        PlayerController.playerController.GetComponent<UpgradeManager>().upgradeEvent.AddListener((Upgrade u) => { PopulateWithUpgrades(); });
    }

    public void PopulateWithUpgrades()
    {
        var upgrades = PlayerController.playerController.GetComponents<Upgrade>();


        //Might not actually destroy the children tho. Will have to test and see.
        for(int i = upgradePanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(upgradePanel.transform.GetChild(i).gameObject);
        }
        

        foreach (Upgrade u in upgrades)
        {
            var upgradeOption = Instantiate(upgradeOptionPrefab, upgradePanel.transform);
            upgradeOption.transform.SetParent(upgradePanel.transform);
            upgradeOption.InitializeWithUpgrade(u);
        }
    }
}
