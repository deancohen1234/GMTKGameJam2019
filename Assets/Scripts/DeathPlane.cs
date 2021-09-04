using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //send in player that was hit and sphere origin
            other.gameObject.GetComponent<PlayerController>().GetHealthComponent().Kill();
        }
    }
}
