using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallTrap : MonoBehaviour
{

    public GameObject jointA;
    public GameObject jointB;

    public float targetRotation = -78.0f;
    private float startRotation = 0.0f;
    private float currentRotation = 0.0f;

    public float trapTime = 0.3f;
    private float currentTrapTime = 0.0f;

    private bool trapActive = false;

    public SpiderBoss spiderBoss;


    public float maxFallTime = 5.0f;
    public float currentFallTime = 0.0f;

    public Transform fallToTransform;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            ActivateTrap();
        }
    }


    private void Update() {
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            trapActive = true;
        }

        if(trapActive == true) {
            currentTrapTime += Time.deltaTime;

            currentRotation = Mathf.LerpAngle(startRotation, targetRotation, currentTrapTime / trapTime);

            
            Vector3 newRotation = new Vector3();
            newRotation = jointA.transform.rotation.eulerAngles;
            newRotation.z = currentRotation;
            

            Quaternion newQuaternion = new Quaternion();
            newQuaternion.z = currentRotation;


            jointA.transform.rotation = Quaternion.Euler(newRotation);
            jointB.transform.rotation = Quaternion.Euler(-newRotation);

            if (currentTrapTime >= trapTime) {
                trapActive = false;
            }
        }
    }

    public void ActivateTrap() {
        
        trapActive = true;
        spiderBoss.StartBoss();

    }


    public void ResetTrap() {
        currentRotation = 0.0f;
        currentTrapTime = 0.0f;
        trapActive = false;
        jointA.transform.rotation = Quaternion.Euler(Vector3.zero);
        jointB.transform.rotation = Quaternion.Euler(Vector3.zero);
    }

}
