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
        //playerController.GetHealthComponent().Kill();
        //112 is only rotation bit mask
        playerController.enabled = false;

        Rigidbody r = playerController.GetComponent<Rigidbody>();
        r.constraints = (RigidbodyConstraints)112;
        r.useGravity = true;

        //r.velocity = new Vector3(0, 20.0f, 0);
    }
}
