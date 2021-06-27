using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : ShipComponent
{
    public string faction;


    public override void SetOff() // if core breaks, all subcomponents are disabled.
    {
        Debug("Core.Setoff");
        base.SetOff();
        
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

}
