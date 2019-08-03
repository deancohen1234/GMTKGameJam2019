using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaWeapon : MonoBehaviour
{
    public Transform m_ArenaCenter;
    public float m_ArenaWidth;
    public float m_ArenaHeight;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //deal damage
            other.gameObject.GetComponent<PlayerController>().EquipWeapon(this);
            gameObject.SetActive(false);
        }
    }

    public void Unequip()
    {
        gameObject.SetActive(true);

        Vector3 newPosition = m_ArenaCenter.position + new Vector3(m_ArenaWidth, 0, m_ArenaHeight);

        transform.position = newPosition;
    }
}
