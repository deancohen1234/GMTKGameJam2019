using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaWeapon : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //deal damage
            other.gameObject.GetComponent<PlayerController>().EquipWeapon();
            gameObject.SetActive(false);
        }
    }
}
