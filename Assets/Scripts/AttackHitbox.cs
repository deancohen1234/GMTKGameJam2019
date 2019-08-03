using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != this.gameObject)
        {
            if (other.gameObject.tag == "Player")
            {
                //deal damage
                other.gameObject.GetComponent<HealthComponent>().DealDamage(200f);
            }

        }
    }
}
