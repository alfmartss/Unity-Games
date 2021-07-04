using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionController
{
    string name;
    private List<Core> ships = new List<Core>();

    public FactionController(string name)
    {
        this.name = name;
        Debug.Log("FACTION " + name + " created");
    }

    internal void AddShip(Core core)
    {
        ships.Add(core);
        Debug.Log("New ship " + core.name + " FACTION " + name + " has " + ships.Count);
    }

    internal void RemoveShip(Core core)
    {
        ships.Remove(core);
        Debug.Log("CORE destroyed "+core.name+", FACTION " + name + ":" + ships.Count + " ships left");
        if (ships.Count==0)
        {
            FactionsManager.FactionLost(name);
        }
    }
}
