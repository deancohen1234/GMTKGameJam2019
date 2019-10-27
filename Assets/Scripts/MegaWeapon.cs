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

    private bool m_IsLerping;
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
        if (m_IsLerping)
        {
            //lerp downward
            Vector3 newLerpPos = Vector3.Lerp(m_StartPos, m_DestinationPos, m_Timer);
            transform.position = newLerpPos;

            m_Timer += Time.deltaTime * m_FallSpeed;

            if (m_Timer >= 1.0f)
            {
                m_Timer = 0;
                m_IsLerping = false;
                transform.position = m_DestinationPos;

                //helps get dagger off tip of stalagmite
                GetComponent<Rigidbody>().AddForce(transform.forward * 5);
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

        float randomX = Random.Range(-1.0f, 1.0f) * m_ArenaWidth;
        float randomY = Random.Range(-1.0f, 1.0f) * m_ArenaHeight;
        Vector3 newPosition = m_ArenaCenter.position + m_Offset + new Vector3(randomX, 0, randomY);
        newPosition.y = transform.position.y;

        /*int numTries = 0;
        while (true)
        {
            numTries++;

            if (numTries >= 20)
            {
                Debug.LogError("Num Tries Exceeded for location");
                break;
            }

            if (IsValidPosition(newPosition))
            {
                break;
            }
            else
            {
                RandomizeLocation();
            }
        }*/

        m_DestinationPos = newPosition;
        m_StartPos = m_DestinationPos + new Vector3(0, 7.5f, 0);

        transform.position = m_StartPos;
        m_IsLerping = true;
    }

    public void RandomizeLocationFromPlayer(Vector3 playerPos)
    {
        
        gameObject.SetActive(true);

        float randomX = Random.Range(-1.0f, 1.0f) * m_ArenaWidth;
        float randomY = Random.Range(-1.0f, 1.0f) * m_ArenaHeight;
        Vector3 newPosition = m_ArenaCenter.position + m_Offset + new Vector3(randomX, 0, randomY);
        newPosition.y = transform.position.y;
        /*
        int numTries = 0;
        while (true)
        {
            numTries++;

            if (numTries >= 20)
            {
                Debug.LogError("Num Tries Exceeded for location");
                break;
            }

            if (IsValidPosition(newPosition))
            {
                break;   
            }
            else
            {
                RandomizeLocationFromPlayer(playerPos);
                break;
            }
        }*/

        m_DestinationPos = newPosition;
        m_StartPos = playerPos + new Vector3(0, 0.5f, 0);

        transform.position = m_StartPos;
        m_IsLerping = true;
    }

    private bool IsValidPosition(Vector3 position)
    {
        bool check = Physics.CheckSphere(position + new Vector3(0, 0.5f, 0), 0.1f);

        //if true then good position, if false then try a new position
        return !check;
    }
}
