using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameOver : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnPoint;

    
    public void GoalSuccess() {
        Debug.Log("Goal Succeeded!");
    }

    bool needsRespawn() {
        return transform.position.y < -10;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (needsRespawn()) {
            Debug.Log("Game Over!");
            transform.position = spawnPoint.transform.position;
        }
    }


}
