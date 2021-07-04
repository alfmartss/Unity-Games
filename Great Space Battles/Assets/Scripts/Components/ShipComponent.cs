using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipComponent : MonoBehaviour
{
    [SerializeField]
    private bool debug = false;
    protected bool isOn = true;
    [SerializeField]
    private int durability = 100;
    [SerializeField]
    private bool destroyOnBreak = true;
    [SerializeField]
    protected float energyUsagePS = 0.0f;
    public int energyPrority = 0;
    public string status = "";
    public string group;
    public static string[] energyModes = new string[] { "battle", "base", "idle", "moving", "working", "emergency" };
   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Damage(int damage, string type)  // ABSTRACTION
    {
        if (durability>=0)
        {
            Debug("ShipComponent.Damage");
            durability -= damage;
            if (durability < 0)
            {
                SetOff("broken");
                if (destroyOnBreak)
                {
                    NotifyCore();
                    Destroy(gameObject);
                }
            }
        }
    }

    private void NotifyCore()
    {
        Core c = transform.GetComponentInParent<Core>();
        if (c!=null && c!=this)
        {
            c.OnChildDestroyed(this);
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        Debug("ShipComponent.OnTriggerEnter_1");
        Projectile p = other.GetComponent<Projectile>();
        if (p != null)
        {
            Debug("ShipComponent.OnTriggerEnter_2");
            Damage(p.Damage, p.DamageType);
            p.AfterColisionWithComponent(this);
        }
    }
    public virtual void SetOff(string status)
    {
        Debug(gameObject.name + ".SetOff");
        this.status = status;
        isOn = false;
    }


    protected void Debug(object msg)
    {
        if (debug) UnityEngine.Debug.Log(msg);
    }

    public virtual float EnergyUsed(float seconds, string mode)
    {
        return isOn? energyUsagePS * seconds :0;
    }

    public virtual bool UsesEnergy()
    {
        return isOn?(energyUsagePS > 0.0f):false;
    }
}
