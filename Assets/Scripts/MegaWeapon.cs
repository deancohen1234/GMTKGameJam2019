using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaWeapon : MonoBehaviour
{
    public Transform m_ArenaCenter;
    public float m_ArenaWidth;
    public float m_ArenaHeight;
    public Vector3 m_Offset;

    public float m_FallSpeed = 0.3f;

    private float m_WeaponStartHeight;

    private bool m_IsFalling;
    private float m_StartFallTime;
    private Vector3 m_DestinationPos;
    private Vector3 m_StartPos;
    private float m_Timer;

    private void Start()
    {
        m_WeaponStartHeight = transform.position.y;
    }

    private void Update()
    {
        if (m_IsFalling)
        {
            //lerp downward
            Vector3 newLerpPos = Vector3.Lerp(m_StartPos, m_DestinationPos, m_Timer);
            transform.position = newLerpPos;

            m_Timer += Time.deltaTime * m_FallSpeed;

            if (m_Timer >= 1.0f)
            {
                m_Timer = 0;
                m_IsFalling = false;
                transform.position = m_DestinationPos;
            }
        }
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

    public void RandomizeLocation()
    {
        gameObject.SetActive(true);

        float randomX = Random.Range(0.0f, 1.0f) * m_ArenaWidth;
        float randomY = Random.Range(0.0f, 1.0f) * m_ArenaHeight;
        Vector3 newPosition = m_ArenaCenter.position + m_Offset + new Vector3(randomX, 0, randomY);
        newPosition.y = transform.position.y;

        m_DestinationPos = newPosition;
        m_StartPos = m_DestinationPos + new Vector3(0, 7.5f, 0);

        transform.position = m_StartPos;
        m_IsFalling = true;
    }
}
