using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderKiller : MonoBehaviour
{

    public SpiderBoss spiderBoss;

    private Rigidbody myRigidbody;

    private void Awake() {
        myRigidbody = GetComponent<Rigidbody>();

    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject == spiderBoss.gameObject) {
            spiderBoss.BossDefeated();
        }
    }

   

    public void DropSpiderKiller() {
        myRigidbody.useGravity = true;
    }


}
