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
        Debug("ShipComponent.Damage");
        durability -= damage;
        if (durability<0)
        {
            SetOff();
            if (destroyOnBreak)
            {
                Destroy(gameObject);
            }
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
    public virtual void SetOff()
    {
        Debug(gameObject.name + ".SetOff");
        isOn = false;
    }


    protected void Debug(object msg)
    {
        if (debug) UnityEngine.Debug.Log(msg);
    }


}
