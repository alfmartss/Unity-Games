using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense : ShipComponent // INHERITANCE
{
    [SerializeField]
    string[] types;
    [SerializeField]
    float[] factors;

    private Dictionary<string, float> damageReduction = new Dictionary<string, float>();

    private void Awake()
    {
        for (int i=0;i<types.Length;i++)
        {
            damageReduction.Add(types[i], factors[i]);
        }
    }
 


    // reduce some types of damage
    public override void Damage(int damage, string type)  // POLYMORPHISM
    {
        Debug("Defense.Damage");
        float factor = 1;
        int newDamage = damage;
        if (damageReduction.TryGetValue(type,out factor))
        {
            float damageFactor = factor;
            newDamage = (int)(newDamage * damageFactor); 
        }
        Debug("newDamage:" + newDamage);
        base.Damage(newDamage, type);
    }

}
