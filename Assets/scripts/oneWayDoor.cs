using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oneWayDoor : MonoBehaviour
{

    [SerializeField]
    private Transform doorFront;
    [SerializeField]
    private BoxCollider blockerCollider;
    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private bool startClosed = false;

    // Start is called before the first frame update
    void Start()
    {
        if (blockerCollider && !startClosed) {
            blockerCollider.enabled = false;
        }

        if (meshRenderer && !startClosed) {
            meshRenderer.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void doorCheck(Collider other) {
        if (other.CompareTag("Player")) {
            Vector3 toObject = other.transform.position - doorFront.position;
            if (Vector3.Dot(toObject, transform.forward) > 0)
            {
                blockerCollider.enabled = true;
                meshRenderer.enabled = true;
            } else {
                blockerCollider.enabled = false;
                meshRenderer.enabled = false;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        doorCheck(other);

    }

    private void OnTriggerExit(Collider other)
    {
        doorCheck(other);
    }
}
