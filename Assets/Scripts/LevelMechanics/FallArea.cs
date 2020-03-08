using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FallArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("In Fall Area");
            OnPlayerFall(other.GetComponent<PlayerController>());
        }
    }

    private void OnPlayerFall(PlayerController playerController)
    {
        playerController.GetHealthComponent().Kill();
    }
}
