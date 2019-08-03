using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    private PlayerController m_ParentPlayerController;

    private void Start()
    {
        m_ParentPlayerController = transform.parent.parent.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != this.gameObject)
        {
            if (other.gameObject.tag == "Player")
            {
                //deal damage
                other.gameObject.GetComponent<PlayerController>().AttemptAttack(m_ParentPlayerController);
            }

        }
    }
}
