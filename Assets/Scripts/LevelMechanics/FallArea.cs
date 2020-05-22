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
            OnPlayerFall(other.GetComponent<PlayerController>());
        }
    }

    private void OnPlayerFall(PlayerController playerController)
    {
        //playerController.GetHealthComponent().Kill();
        //112 is only rotation bit mask
        playerController.DisableController(false);

        Rigidbody r = playerController.GetComponent<Rigidbody>();
        r.constraints = (RigidbodyConstraints)112;
        r.useGravity = true;

        playerController.gameObject.layer = LayerMask.NameToLayer("IgnoreFloor");
    }
}
