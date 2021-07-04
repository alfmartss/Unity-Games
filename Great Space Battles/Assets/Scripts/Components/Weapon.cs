using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : ShipComponent
{
    [SerializeField]
    GameObject projectilePrefab;
    [SerializeField]
    float fireLapse = 1.0f;
    [SerializeField]
    public float fireRange { get; private set; }

    public GameObject target;
    private AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponentInParent<AudioSource>();
        Projectile p = projectilePrefab.GetComponent<Projectile>();
        fireRange = p.maxDistance;
        Invoke("TryShootAgain", fireLapse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TryShootAgain()
    {
        Debug("Weapon.TryShootAgain");
        if (IsOn()){
            if (target!=null)
            {
                FireProjectile();
            }
            else
            {
                Debug("Weapon.TryShootAgain.TargetIsNull");
            }            
        }    
        else
        {
            Debug("Weapon.TryShootAgain.isOff");
        }
        Invoke("TryShootAgain", fireLapse);
    }

    private void FireProjectile()      // ABSTRACTION
    {        
        Vector3 direction = target.transform.position - transform.position;
        Projectile.Fire(gameObject, direction, projectilePrefab, audioSource);
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
