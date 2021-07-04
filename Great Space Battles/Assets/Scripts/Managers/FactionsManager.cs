using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionsManager : MonoBehaviour
{
    private float BattleLoopLapse = 15.0f;
    private static Dictionary<string, FactionBattleController> factions = new Dictionary<string, FactionBattleController>();

    void Start()
    {
        Invoke("BattleLoop", 2.0f);
    }

    private void BattleLoop()
    {
        
        foreach (FactionBattleController fc in factions.Values)
        {
            fc.BattleLoop();
        }
        Invoke("BattleLoop", BattleLoopLapse);
    }

    public static FactionBattleController GetFactionController(string name)
    {
        if (factions.ContainsKey(name))
        {
            return factions[name];
        } else
        {
            FactionBattleController f = new FactionBattleController(name);
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
