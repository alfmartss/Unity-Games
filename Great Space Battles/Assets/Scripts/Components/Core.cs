using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : ShipComponent
{
    private static List<Core> instances = new List<Core>();
    public string faction;

    private Weapon[] weapons;
    private Core[] enemys;
    private float targetingRate = 2.0f;
    public string targetingStrategy = "";

    private void Start()
    {
        instances.Add(this);
        weapons = gameObject.GetComponentsInChildren<Weapon>();
        Debug("Weapons detected: " + weapons.Length);
        Invoke("TargetingSystemLoop", UnityEngine.Random.Range(targetingRate/2, targetingRate));
    }

    public override void SetOff() // if core breaks, all subcomponents are disabled and the core removed from the list of instances.
    {
        Debug("Core.Setoff");
        base.SetOff();
        //
        instances.Remove(this);
        // loop 
        ShipComponent[] cs = gameObject.GetComponentsInChildren<ShipComponent>();
        foreach (ShipComponent c in cs)
        {
            if (c!=this)
            {
                c.SetOff();
            }           
        }
    }

    private void TargetingSystemLoop()
    {
        Debug("TargetingSystemLoop");
        enemys = DetectEnemys();
        if (enemys!=null)
        {
            CalculateNewTargets(weapons, enemys);
        } else
        {
            ClearTargets(weapons);
        }

        Invoke("TargetingSystemLoop", targetingRate);
    }

    private void ClearTargets(Weapon[] weapons)
    {
        foreach (Weapon w in weapons)
        {
            w.target = null;
        }
    }

    private Core[] DetectEnemys()
    {
        List<Core> enemyCores = new List<Core>();
        foreach (Core core in instances)
        {
            if (core.faction!=this.faction)
            {
                Debug("DetectEnemys: enemy detected " + core.name);
                enemyCores.Add(core);
            }
        }
        return enemyCores.ToArray();        
    }

    private void CalculateNewTargets(Weapon[] weapons, Core[] enemys)
    {
        Debug("CalculateNewTargets");
        if (this.targetingStrategy=="" || this.targetingStrategy=="random")
        {
            RandomTargetingStrategy(weapons, enemys);
        }        
    }

    private void RandomTargetingStrategy(Weapon[] weapons, Core[] enemys)
    {
        Debug("RandomTargetingStrategy");
        foreach (Weapon w in weapons)
        {
            int enemyIndex = UnityEngine.Random.Range(0, enemys.Length);
            GameObject newTarget = enemys[enemyIndex].gameObject;
            Debug("RandomTargetingStrategy: " + w.name + " -> " + newTarget.name);
            w.target = newTarget;
        }
    }
}
