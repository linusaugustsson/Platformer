using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryScript : MonoBehaviour
{

    public GlobalData globalData;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            globalData.player.gameManager.ShowVictoryScreen();
        }
    }
}
