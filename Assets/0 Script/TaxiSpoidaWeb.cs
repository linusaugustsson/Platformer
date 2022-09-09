using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaxiSpoidaWeb : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if(other.transform.parent.TryGetComponent(out Player player))
        {
            player.PlayerSlow(0.2f, 2f);
        }
    }
}
