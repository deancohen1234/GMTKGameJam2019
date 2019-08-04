using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaWeapon : MonoBehaviour
{
    public Transform m_ArenaCenter;
    public float m_ArenaWidth;
    public float m_ArenaHeight;

    private float m_WeaponStartHeight;

    private void Start()
    {
        m_WeaponStartHeight = transform.position.y;
    }

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
        /*gameObject.SetActive(true);

        float randomX = Random.Range(0.0f, 1.0f) * m_ArenaWidth;
        float randomY = Random.Range(0.0f, 1.0f) * m_ArenaHeight;
        Vector3 newPosition = m_ArenaCenter.position + new Vector3(randomX, 0, randomY);

        transform.position = newPosition;*/
    }
}
