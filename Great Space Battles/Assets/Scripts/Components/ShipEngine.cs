using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipEngine : ShipComponent // INHERITANCE
{
    [SerializeField]
    private float speed;  // ENCAPSULATION

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOn())
        {
            transform.parent.Translate(Vector3.forward * speed * Time.deltaTime);
        }
         
    }
}
