using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float m_speed;

    [SerializeField]
    private float maxDistance;

    private Vector2 v_speed;
    
    [SerializeField]
    private string m_damageType = "kinetic";
    [SerializeField]
    private int damage = 10; 

    [SerializeField]
    public string DamageType
    {  // ENCAPSULATION 
        get { return m_damageType; }
    } 
    [SerializeField]
    public int Damage
    { // ENCAPSULATION 
        get { return damage; }
    }  // ENCAPSULATION 

    private Vector3 initialPosition;

    private GameObject weapon;

    private void Start() 
    {
        initialPosition = transform.position;
    }

    public static GameObject Fire(GameObject weapon, Vector3 direction, GameObject projectilePrefab)  // ABSTRACTION
    {
        //  Quaternion lookRotation = Quaternion.LookRotation(direction);
        GameObject shell = Instantiate<GameObject>(projectilePrefab);

        shell.transform.position = weapon.transform.position;
        Projectile p = shell.GetComponentInChildren<Projectile>();
        // set velocity
        Rigidbody rb = shell.GetComponent<Rigidbody>();
        rb.velocity = p.m_speed * direction;
        // face in direction of velocity
        shell.transform.forward = rb.velocity;
        p.weapon = weapon;
        // set initial speed 
        //p.v_speed = direction * p.m_speed;
        return shell;
    }


    // Update is called once per frame
    void Update()
    {
        if (checkMaxDistance())        
        {
            //transform.Translate(v_speed * Time.deltaTime);
        }       
    }

    private bool checkMaxDistance()  // ABSTRACTION
    {
        float distanceTravelled = (transform.position - initialPosition).magnitude;
        if (distanceTravelled > maxDistance)
        {
            Destroy(gameObject);
            return false;
        }
        return true;
    }

    public void AfterColisionWithComponent(ShipComponent c)
    {
        Destroy(gameObject);
    }

    public bool CheckSameWeapon(GameObject o)
    {
        if (o!=null && o==this.weapon)
        {
            return true;
        }
        return false;
    }
}
