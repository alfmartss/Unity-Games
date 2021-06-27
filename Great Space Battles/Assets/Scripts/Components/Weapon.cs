using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : ShipComponent
{
    [SerializeField]
    GameObject projectilePrefab;
    [SerializeField]
    float fireRate = 1.0f;
    [SerializeField]
    float fireRange = 50.0f;

    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("TryShootAgain", fireRate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TryShootAgain()
    {
        if (isOn)
        {
            FireProjectile();
        }        
        Invoke("TryShootAgain", fireRate);
    }

    private void FireProjectile()      // ABSTRACTION
    {
        Vector3 direction = target.transform.position - transform.position;
        Projectile.Fire(gameObject, direction, projectilePrefab);
    }

    public override void OnTriggerEnter(Collider other)  // POLYMORPHISM
    {
        Debug("Weapon.OnTriggerEnter");
        Projectile p = other.gameObject.GetComponent<Projectile>();
        if (p != null)
        { 
            if (!p.CheckSameWeapon(gameObject)) // projectiles dont damage the weapon that fired them
            {
                Debug("projectiles dont damage the weapon that fired them");
                Damage(p.Damage, p.DamageType);
                p.AfterColisionWithComponent(this);
            }            
        }
    }
}
