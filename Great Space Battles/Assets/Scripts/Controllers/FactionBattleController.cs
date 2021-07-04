using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionBattleController 
{
    
    string name;
    private List<Core> ships = new List<Core>();
    private Core[] detectedEnemys;
    
    public string status = "";

    public FactionBattleController(string name)
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

    public void BattleLoop()
    {
        Debug.Log("FACTION " + name + " BattleLoop");
        status = EvaluateStatus();
        foreach(Core ship in ships)
        {
            ShipCommandInfo command = GetCommandForShip(ship);
            Debug.Log("ship " + ship.name + " FACTION " + name + " command " + command.ToString());
            ship.SetCommand(command);
        }        
    }

    private static ShipCommandInfo GetCommandForShip(Core c)
    {
        return new ShipCommandInfo("Attack", "Nearest", "Core", null);
    }

    private string EvaluateStatus()
    {
        detectedEnemys = Core.DetectEnemys(this.name);

        return "battle";
    }

    public Core[] GetDetectedEnemies()
    {
        return detectedEnemys;
    }


}

public class ShipCommandInfo
{
    public string name, advice, component;
    public GameObject target;
    public ShipCommandInfo(string name, string advice, string component, GameObject target)
    {
        this.name = name;
        this.advice = advice;
        this.component = component;
        this.target = target;
    }
    
    public override string ToString()
    {
        return "[" + name + ", " + advice + ", " + component + "]";
    }
}
