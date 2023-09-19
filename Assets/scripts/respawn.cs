using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class respawn : MonoBehaviour
{

    [SerializeField]
    private GameObject spawnPoint;

    bool needsRespawn() {
        return transform.position.y < -10;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (needsRespawn()) {
            transform.position = spawnPoint.transform.position;
        }
    }
}
