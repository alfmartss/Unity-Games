using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionsManager 
{
    private static Dictionary<string, FactionController> factions = new Dictionary<string, FactionController>();


    public static FactionController GetFactionController(string name)
    {
        if (factions.ContainsKey(name))
        {
            return factions[name];
        } else
        {
            FactionController f = new FactionController(name);
            factions.Add(name, f);
            return f;
        }
        
    }

    internal static void FactionLost(string name)
    {
        factions.Remove(name);
        CheckVictory();

    }

    private static void CheckVictory()
    {
        if (factions.Count==1)
        {
            string winner = "";
            foreach(string fname in factions.Keys)
            {
                winner = fname;
            }
            UnityEngine.Debug.Log("END OF BATTLE: " + winner);
        }
    }
}
