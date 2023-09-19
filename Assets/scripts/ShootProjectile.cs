using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class ShootProjectile : MonoBehaviour
{
    [SerializeField]
    private GameObject prefabProjectile;
    [SerializeField]
    private float projectileSpeed = 5.0f;
    [SerializeField]
    private float projectileSpawnDistance = 0.5f;
    [SerializeField]
    private GameObject playerCamera;


    GameObject newProjectile = null;

    private bool inSafeZone;
    
    public void InstantiateProjectile() {

        if (prefabProjectile != null) {
            // Vector3 playerForward = transform.forward;
            Vector3 playerForward = playerCamera.transform.forward;
            // Debug.Log(pitch);
            Vector3 spawnPosition = transform.position + playerForward * projectileSpawnDistance;
            

            Destroy(newProjectile);
            newProjectile = Instantiate(prefabProjectile, spawnPosition, Quaternion.identity);
            Rigidbody rb = newProjectile.GetComponent<Rigidbody>();
            rb.velocity = playerForward * projectileSpeed;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
      if (!inSafeZone && Input.GetMouseButtonDown(0)) {
        InstantiateProjectile();
      }
    }

    void OnTriggerEnter(Collider other)
    {
      if (other.CompareTag("SafeZone")) {
        inSafeZone = true;
      }
    }

    void OnTriggerExit(Collider other)
    {
      if (other.CompareTag("SafeZone")) {
        inSafeZone = false;
      }
    }

}
