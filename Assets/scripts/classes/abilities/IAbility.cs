using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbility
{
    public string Name {get;}
    public string Description {get;}
    public float CallCooldown {get;}
    public bool CanCall {get; set;}

    public float LastCalled {get; set;}

    public void Run(params object[] args);
}
