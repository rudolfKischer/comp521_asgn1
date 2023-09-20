using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class target : MonoBehaviour
{
    [SerializeField]
    private BoxCollider doorBlockerCollider;
    [SerializeField]
    private MeshRenderer doorMeshRenderer; 

    [SerializeField]
    private Material mat1;
    [SerializeField]
    private Material mat2;
    

    private bool doorOpenToggle = false;
    private MeshRenderer meshRenderer;

    private bool hitCoolDown = false;
    private float hitCoolDownDuration = 0.1f;

    private int hitcounter =0;

    // Start is called before the first frame update
    void Start()
    {
      meshRenderer = GetComponent<MeshRenderer>();   
      meshRenderer.material = mat2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void toggleDoor() {
        if (doorBlockerCollider && 
            doorMeshRenderer
        ) {
            doorBlockerCollider.enabled = doorOpenToggle;
            doorMeshRenderer.enabled = doorOpenToggle;
        }
        hitcounter ++;
        if (doorOpenToggle == true) {
          meshRenderer.material = mat2;
          doorOpenToggle = false;
        }else if (doorOpenToggle == false){
          meshRenderer.material = mat1;
          doorOpenToggle = true;
        }


    }

    IEnumerator CooldownCoroutine() {
          hitCoolDown = true;
          yield return new WaitForSeconds(hitCoolDownDuration);
          hitCoolDown = false;
    }

    private void OnCollisionEnter(Collision collision) {

        if (collision.gameObject.tag != "Projectile") {
            return;
        }

        if (!hitCoolDown) {

            toggleDoor();
            StartCoroutine(CooldownCoroutine());
        }

        Destroy(gameObject);






    }


}
