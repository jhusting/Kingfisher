using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UpgradeEvent : UnityEvent<Upgrade> { }


public class UpgradeManager : MonoBehaviour
{
    public UpgradeEvent upgradeEvent { get; private set; }

    void Awake()
    {
        upgradeEvent = new UpgradeEvent();
    }
}
