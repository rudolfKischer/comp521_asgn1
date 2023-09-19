using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class projectile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Math.Abs(gameObject.transform.position.y) >= 15.0) {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision) {
          Destroy(gameObject);
    }
}
