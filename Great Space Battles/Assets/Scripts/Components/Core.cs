using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : ShipComponent
{
    private static List<Core> allCores = new List<Core>();
    public string faction;
    private FactionBattleController factionController;

    private List<Weapon> weapons;
    private List<EnergyGenerator> generators;

    private List<ShipComponent> energyUsersComps;
    private List<ShipComponent> noEnergyComps;

    private Core[] enemys;
    private float targetingLoopLapse = 5.0f;

    private float energyLoopLapse = 10.0f;
    //private string energyStrategy = "";
    protected string energyMode = "battle";// base, idle, moving, working, emergency
    public string battleRole = "";

    private ShipCommandInfo command;


    private void Start()
    {
        factionController = FactionsManager.GetFactionController(faction);
        factionController.AddShip(this);
        allCores.Add(this);

        weapons = new List<Weapon>(gameObject.GetComponentsInChildren<Weapon>());
        Debug("Weapons detected: " + weapons.Count);
        Invoke("TargetingSystemLoop", UnityEngine.Random.Range(targetingLoopLapse / 2, targetingLoopLapse));

        ShipComponent[] subcomponents = gameObject.GetComponentsInChildren<ShipComponent>();
        energyUsersComps = new List<ShipComponent>();
        foreach (ShipComponent c in subcomponents)
        {
            if (c.UsesEnergy()) {
                Debug("Core: found consumer " + c.name);
                energyUsersComps.Add(c);
            }
        }
        SetEnergyPrioritys(energyUsersComps);
        energyUsersComps.Sort(delegate (ShipComponent a, ShipComponent b) {
            return (a.energyPrority).CompareTo(b.energyPrority);
        });

        generators = new List<EnergyGenerator>(gameObject.GetComponentsInChildren<EnergyGenerator>());
        SetGeneratorsPrioritys(generators);
        generators.Sort(delegate (EnergyGenerator a, EnergyGenerator b) {
            return (a.generatorsPriority).CompareTo(b.generatorsPriority);
        });

        EnergyManageLoop();
    }

    internal void SetCommand(ShipCommandInfo command)
    {
        this.command = command;
    }

    #region EnergyManagement
    internal void OnChildDestroyed(ShipComponent sc)
    {
        if (energyUsersComps.Contains(sc))
        {
            energyUsersComps.Remove(sc);
        }
        if (sc.group.Contains("weapon"))
        {
            Weapon w = sc.gameObject.GetComponent<Weapon>();
            if (w != null)
            {
                weapons.Remove(w);
            }
        }
        if (sc.group.Contains("energy"))
        {
            EnergyGenerator g = sc.GetComponent<EnergyGenerator>();
            if (g != null)
            {
                generators.Remove(g);
            }
        }
    }

    private void SetGeneratorsPrioritys(List<EnergyGenerator> generators)
    {
        // todo. first free generators , next reactors , last batteries.        
        string[] pGroups = new string[] { "solar", "reactor", "batteries" };
        foreach (EnergyGenerator g in generators)
        {
            string group = g.group;
            int index = findGroupIndex(pGroups, group);
            if (index > -1)
            {
                g.generatorsPriority = index;
                Debug("Core.SetGeneratorsPrioritys:" + g.name + "=" + index + " (" + group + ")");
            }
            else
            {
                g.generatorsPriority = 999;
                Debug("Core.SetGeneratorsPrioritys:" + g.name + " group not found " + group);
            }

        }
    }

    private void SetEnergyPrioritys(List<ShipComponent> energyUsersComps)
    {
        // todo. cores, thrusters, weapons? shields? bateries?
        string[] pGroups = new string[] { "core", "thruster", "shield", "weapon", "batteries" };
        foreach (ShipComponent c in energyUsersComps)
        {
            string group = c.group;
            int index = findGroupIndex(pGroups, group);
            if (index > -1)
            {
                c.energyPrority = index;
                Debug("Core.SetEnergyPrioritys:" + c.name + "=" + index + " (" + group + ")");
            }
            else
            {
                c.energyPrority = 999;
                Debug("Core.SetEnergyPrioritys:" + c.name + " group not found " + group);
            }

        }
    }

    private int findGroupIndex(string[] matches, string name)
    {
        for (int i = 0; i < matches.Length; i++)
        {
            if (name.Contains(matches[i]))
            {
                return i;
            }
        }
        return -1;
    }

    private void EnergyManageLoop()
    {
        if (IsOn())
        {
            Debug("Core.EnergyManageLoop");
            float energyNeeded = 0.0f;
            float energyProduced = 0.0f;
            Queue<ShipComponent> consumers = new Queue<ShipComponent>(energyUsersComps);
            ShipComponent c = null;

            foreach (EnergyGenerator g in generators)
            {
                if (g != null)
                {
                    Debug("Core.EnergyManageLoop:" + g.name);
                    energyProduced = g.EnergyProduced(energyLoopLapse, energyMode);
                    Debug("Core.EnergyManageLoop:" + g.name + " produces " + energyProduced);
                    while (consumers.Count > 0 && energyProduced > 0.0f)
                    {
                        c = consumers.Dequeue();
                        if (c != null)
                        {
                            energyNeeded = c.EnergyUsed(energyLoopLapse, energyMode);
                            Debug("Core.EnergyManageLoop:" + c.name + " needs " + energyNeeded);
                            if (energyProduced >= energyNeeded)
                            {
                                g.GetEnergy(energyNeeded, energyLoopLapse, energyMode);
                                energyProduced -= energyNeeded;
                                energyNeeded = 0.0f;
                            }
                            else
                            {
                                energyNeeded -= energyProduced;
                                energyProduced = 0.0f;
                            }
                        }


                    }
                }

            }

            if (energyNeeded > 0.0f && c != null)
            {
                Debug("Core.EnergyManageLoop:" + c.name + " must be setoff");
                c.SetOff("no energy");
            }
            if (consumers.Count > 0)
            {
                Debug("Core.EnergyManageLoop:" + " not enought energy some consumers must be setoff");
                // disable?

                foreach (ShipComponent notEnergizedComponent in consumers)
                {
                    Debug("Core.EnergyManageLoop:" + notEnergizedComponent.name + " must be setoff");
                    notEnergizedComponent.SetOff("no energy");
                }
            } else
            {
                //TODO: enable disabled comps
            }

            Invoke("EnergyManageLoop", energyLoopLapse);
        }
    }

    public override void SetOff(string status) // if core breaks, all subcomponents are disabled and the core removed from the list of instances.
    {
        Debug("Core.Setoff");
        base.SetOff(status);
        //
        allCores.Remove(this);
        factionController.RemoveShip(this);
        // loop 
        ShipComponent[] cs = gameObject.GetComponentsInChildren<ShipComponent>();
        foreach (ShipComponent c in cs)
        {
            if (c != this)
            {
                c.SetOff("core setoff");
            }
        }
    }
    #endregion

    #region BattleManagement


    private void TargetingSystemLoop()
    {
        Debug("TargetingSystemLoop");
        enemys = factionController.GetDetectedEnemies();
        if (enemys != null && enemys.Length > 0)
        {
            CalculateNewTargets(weapons, enemys);
        } else
        {
            // no enemys
            Debug("TargetingSystemLoop.no enemys");
            ClearTargets(weapons);
        }

        Invoke("TargetingSystemLoop", targetingLoopLapse);
    }

    private void ClearTargets(List<Weapon> weapons)
    {
        foreach (Weapon w in weapons)
        {
            w.target = null;
        }
    }

    public static Core[] DetectEnemys(string myFaction)
    {
        List<Core> enemyCores = new List<Core>();
        foreach (Core core in allCores)
        {
            if (core.faction != myFaction)
            {
                //      Debug("DetectEnemys: enemy detected " + core.name);
                enemyCores.Add(core);
            }
        }
        return enemyCores.ToArray();
    }

    private void CalculateNewTargets(List<Weapon> weapons, Core[] enemys)
    {
        Debug("TargetingSystemLoop.CalculateNewTargets.command" + this.command.ToString());
        if (this.command.name == "Attack")
        {
            if (this.command.advice == "Nearest")
            {
                NearestTargetingStrategy(weapons, enemys);
            }
            if (this.command.advice == "LongRange")
            {
                LongRangeTargetingStrategy(weapons, enemys);
            }

            if (this.command.advice == "Random")
            {
                RandomTargetingStrategy(weapons, enemys);
            }
        }

    }

    private void LongRangeTargetingStrategy(List<Weapon> weapons, Core[] enemys)
    {
        float longRange = -1;
        foreach (Weapon w in weapons)
        {
            if (longRange==-1 || w.fireRange>longRange)
            {
                longRange = w.fireRange;
            }
        }
        // long range weapons fire target
        float longRangeLimit = longRange * 0.8f;
        List<Weapon> restOfWeapons = new List<Weapon>();
        GameObject bestTarget = null;
        if (command.component != "")
        {
            bestTarget = FindTargetInRangeWithComponent(enemys, longRange, command.component);
        }

        foreach (Weapon w in weapons)
        {
            w.target = bestTarget;
        }
        if (restOfWeapons.Count>0)
        {
            NearestTargetingStrategy(restOfWeapons, enemys);
        }

    }

    private GameObject FindTargetInRangeWithComponent(Core[] enemys, float range, string component)
    {
        float sqrRange = range * range;
        foreach(Core enemy in enemys)
        {
            ShipComponent comp = FindShipComponent(enemy.gameObject, component);
            if (comp != null && comp.isActiveAndEnabled && comp.IsOn())
            {                
                if (InRange(sqrRange, comp.gameObject, this.gameObject))
                {
                    return comp.gameObject;
                }
            }
        }
        return null;
    }

    private bool InRange(float sqrRange, GameObject target, GameObject origin)
    {        
        Vector3 direction = target.transform.position - origin.transform.position;
        float sqrMagnitude = direction.sqrMagnitude;                
        return (sqrMagnitude <= sqrRange);
        
    }
    private void RandomTargetingStrategy(List<Weapon> weapons, Core[] enemys)
    {
        //Debug("RandomTargetingStrategy");
        foreach (Weapon w in weapons)
        {
            if (enemys.Length>1)
            {
                int enemyIndex = UnityEngine.Random.Range(0, enemys.Length);
                GameObject newTarget = enemys[enemyIndex].gameObject;
                //  Debug("RandomTargetingStrategy: " + w.name + " -> " + newTarget.name);
                w.target = newTarget;
            } else
            {
                GameObject newTarget = enemys[0].gameObject;
                w.target = newTarget;
            }

        }
    }

    private void NearestTargetingStrategy(List<Weapon> weapons, Core[] enemys)
    {
        Debug("TargetingSystemLoop.NearestTargetingStrategy" + this.command.ToString());
        GameObject newTarget = null;
        float bestDistance = -1;
        foreach(Core c in enemys)
        {
            Vector3 direction = c.transform.position - this.transform.position;
            float sqrMagnitude = direction.sqrMagnitude;
            if (bestDistance==-1 || sqrMagnitude<bestDistance)
            {
                bestDistance = sqrMagnitude;
                newTarget = c.gameObject;
                Debug("TargetingSystemLoop.NearestTargetingStrategy." + bestDistance + ":" + newTarget.name);
            }
        }
        Debug("TargetingSystemLoop.NearestTargetingStrategy.BEST" + bestDistance + ":" + newTarget.name);
        foreach (Weapon w in weapons)
        {
            w.target = newTarget;
        }
    }

    private ShipComponent FindShipComponent(GameObject target, string match)
    {        
        if (match!=null && match!="")
        {
            ShipComponent[] componects = target.GetComponentsInChildren<ShipComponent>();
            foreach(ShipComponent c in componects)
            {
                if (c.group.Contains(match))
                {
                    return c;
                }
            }
        }
        return null;
    }

    #endregion
}
