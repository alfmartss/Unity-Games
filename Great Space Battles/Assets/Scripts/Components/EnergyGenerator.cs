using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyGenerator : ShipComponent
{
    //public static string[] energyModes = new string[] { "battle", "base", "idle", "moving", "working", "emergency" };
    public string[] outputs;
    public string[] inputs;
    //public Dictionary<string, float> input;
    //public Dictionary<string, float> output;

    public float maxCapacity=0;
    public float storedEnergy = 0;
    public float maxIO = 0;
    public float generatedEnergy = 0;

    private string currentMode="";
    private float currentInput = 0;
    private float currentOutput = 0;


    public int generatorsPriority = 0;

    public void CheckMode(string mode)
    {       
        if (mode!=null && mode!=currentMode)
        {
            Debug("CheckMode" + mode);
            currentOutput = 0.0f;
            foreach (string outputmode in outputs)
            {
                if (outputmode.Equals(mode))
                {
                    currentOutput = generatedEnergy>0 ? generatedEnergy : maxIO;
                    Debug("CheckMode" + mode + ", output:" + currentOutput);
                    break;
                }
            }
            currentInput = 0;
            foreach (string inputmode in outputs)
            {
                if (inputmode.Equals(mode))
                {
                    currentInput = maxIO;
                    Debug("CheckMode" + mode + ", input:" + currentInput);
                    break;
                }
            }
            currentMode = mode;
        }
    }

    public override float EnergyUsed(float seconds, string mode)
    {
        CheckMode(mode);
        return isOn ? currentInput * seconds : 0.0f ;
    }

    public float EnergyProduced(float seconds, string mode)
    {
        if (isOn)
        {
            CheckMode(mode);
            if (generatedEnergy > 0)
            {
                return currentOutput * seconds;
            } else
            {
                return Mathf.Min(maxIO * seconds, storedEnergy);
            }
        } else
        {
            return 0.0f;
        }
    }

    public bool GetEnergy(float energy, float seconds, string mode)
    {
        float needed = energy;
        if (isOn)
        {
            CheckMode(mode);
            if (currentOutput <= needed)
            {
                float generated = generatedEnergy * seconds;
                needed -= generated;
                if (needed > 0)
                {
                    if (storedEnergy > needed)
                    {
                        storedEnergy -= needed;
                        Debug("GetEnergy(" + energy + "): stored:" + storedEnergy);
                    } else
                    {
                        return false;
                    }                    
                }
                return true;
            } 
        }
        return false;
        
    }

    public bool PutEnergy(float energy)
    {
        
        storedEnergy += energy;
        if (storedEnergy>maxCapacity)
        {
            storedEnergy = maxCapacity;
        }
        return true;
    }
}
