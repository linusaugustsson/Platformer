using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderWeb : MonoBehaviour
{

    public SpiderBoss spiderBoss;
    public GlobalData globalData;

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "pushable") {
            globalData.soundManager.PlaySound(transform.position, globalData.soundManager.webBreak);

            spiderBoss.HurtBoss();

            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }

    }

}
