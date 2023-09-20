using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class crumblyStone : MonoBehaviour
{
    
    public Material crumblyStoneMaterial;
    public Material crumblingStoneMaterial;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision) {
          if (collision.gameObject.CompareTag("Player")) {
                StartCoroutine(Crumble());
          }
    }

    private IEnumerator Crumble() {
          GetComponent<MeshRenderer>().material = crumblingStoneMaterial;
          yield return new WaitForSeconds(0.5f);
          gameObject.SetActive(false);
    }

    
}
