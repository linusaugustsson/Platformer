using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObjectSpider : MonoBehaviour
{

    public GlobalData globalData;

    public int health = 6;

    public GameObject boxObject;

    private int[] randomArray = new int[] { -1, 1 };


    public bool shouldShake = false;
    private Vector3 startPosition = new Vector3();

    public float shakeTime = 0.3f;
    private float currentShakeTime = 0.0f;

    public Transform targetPlayerTransform;

    private void Start() {
        startPosition = gameObject.transform.position;
        //targetPlayerTransform = globalData.player.characterController.transform;
    }

    private void Update() {
        if (shouldShake == true) {
            currentShakeTime += Time.deltaTime;

            float x = gameObject.transform.position.x;
            float y = gameObject.transform.position.y;
            float z = (startPosition.z + ((Mathf.Sin(Time.time * 1.1f) * 0.15f) * /*Random.Range(-1, 1)*/ randomArray[Random.Range(0, 2)]));

            // Then assign a new vector3
            gameObject.transform.position = new Vector3(x, y, z);

            if (currentShakeTime >= shakeTime) {
                shouldShake = false;
                currentShakeTime = 0.0f;
                gameObject.transform.position = startPosition;

            }

        }
    }

    private void FixedUpdate() {
        
    }

    public void Hurt(int _damage) {
        startPosition = gameObject.transform.position;
        health -= _damage;

        //globalData.player.gameManager.soundManager.PlaySound()
        globalData.soundManager.PlaySound(transform.position, globalData.soundManager.breakableHit);

        if(health <= 0) {
            DestroyObject();
        } else {
            shouldShake = true;
        }

    }

    public void DestroyObject() {

        if (boxObject != null) {
            Vector3 moveVector = new Vector3();
            moveVector = globalData.player.characterController.transform.position - boxObject.transform.position;
            moveVector.y = 0.0f;
            moveVector = moveVector.normalized * 200.0f;

        
            boxObject.GetComponent<Rigidbody>().AddForce(moveVector);
        }
        

        Destroy(this.gameObject);
    }



}
