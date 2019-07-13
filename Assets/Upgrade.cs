using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{

    [SerializeField]
    public int currentLevel { get; private set; }

    [SerializeField]
    public int maxLevel { get; private set; }


    public string upgradeName;

    protected PlayerController playerController;

    void Awake()
    {
        currentLevel = 1;
    }
    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    public bool CanAffordUpgrade()
    {
        return playerController.cash > GetCostToUpgrade();
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
        return currentLevel * 100;
    }


    virtual public string GetDescription()
    {
        return "Please purchase Gems to see this upgrades description";
    }
}
