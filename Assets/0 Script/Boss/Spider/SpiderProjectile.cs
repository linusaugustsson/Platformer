using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderProjectile : MonoBehaviour
{
    public int damage = 1;

    private float lifeTime = 10.0f;
    private float currentLifeTime = 0.0f;


    private void Update() {
        currentLifeTime += Time.deltaTime;
        
        if(currentLifeTime >= lifeTime) {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player") {

            if(damage == 1) {
                other.GetComponentInParent<Player>().PlayerSlow(0.5f, 1.0f);
            } else {
                other.GetComponentInParent<Player>().PlayerSlow(0.2f, 1.5f);
            }
            
            Destroy(this.gameObject);

        } else if(other.gameObject.tag == "table") {
            
            other.GetComponent<DestroyableObjectSpider>().Hurt(damage);
            Destroy(this.gameObject);

        } else if(other.gameObject.tag == "bookshelf") {
            other.GetComponent<DestroyableObjectSpider>().Hurt(damage);
            Destroy(this.gameObject);
        } else {
            Destroy(this.gameObject);
        }


    }



}
