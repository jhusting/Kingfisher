using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Upgrade : MonoBehaviour
{

    [SerializeField]
    public int currentLevel { get; private set; }

    [SerializeField]
    public int maxLevel { get; private set; }

    public Sprite myIcon;

    public string upgradeName;

    protected PlayerController playerController;

    void Awake()
    {
        currentLevel = 1;
        maxLevel = 5;
    }
    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    public bool CanAffordUpgrade()
    {
        return playerController.cash > GetCostToUpgrade() && currentLevel < maxLevel;
    }

    public bool TriggerUpgrade()
    {
        if (CanAffordUpgrade())
        {
            playerController.AddCash(-1 * GetCostToUpgrade());

            SetUpgradeLevel(currentLevel + 1);
            GetComponent<UpgradeManager>().upgradeEvent.Invoke(this);
            return true;
        }

        return false;
    }


    //Override this in the child class to handle adjusting the players stats
    virtual public void SetUpgradeLevel(int i)
    {
        currentLevel = i;
    }

    //Override this to change the cost if an upgrade is more valuable
    virtual public int GetCostToUpgrade()
    {
        return currentLevel * 50;
    }


    virtual public string GetDescription()
    {
        return "Please purchase Gems to see this upgrades description";
    }
}
