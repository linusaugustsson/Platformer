using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaxiObject : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.transform.parent.TryGetComponent(out Player player))
        {
            player.globalData.taxiManager.DamagePlayer();
        }
    }
}
