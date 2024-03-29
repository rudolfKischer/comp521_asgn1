using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerInAreaTrigger : MonoBehaviour

{

    [SerializeField] 
    public UnityEngine.Events.UnityEvent onPlayerEnterArea;
    public UnityEngine.Events.UnityEvent onPlayerExitArea;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit (Collider other) {
        if (other.gameObject.tag == "Player") {
            onPlayerExitArea.Invoke();
        }
    }

    private void OnTriggerEnter (Collider other) {
        if (other.gameObject.tag == "Player") {
            onPlayerEnterArea.Invoke();
        }
    }

    
}
