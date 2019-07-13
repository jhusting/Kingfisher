using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum RunFailedStatus
{
    HeldBreath,
    NoOxygen,
    Hurt
}

public class RunFailedEvent : UnityEvent<RunFailedStatus> { }
