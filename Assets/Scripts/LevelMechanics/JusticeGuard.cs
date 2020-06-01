using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//blocks player from passing through this collider if they haven't been whacked
[RequireComponent(typeof(Collider))]
public class JusticeGuard : MonoBehaviour
{
    public float m_ForceThreshold = 300f;
    public float m_BouncebackForce = 3.0f;
    public float m_BouncebackThreshold = 1.0f;

    public int m_Health = 3;

    private int m_CurrentHealth = 0;

    private void Start()
    {
        m_CurrentHealth = m_Health;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Player")
        {
            if (collision.collider.GetComponent<JusticeUser>().GetIsHit())
            {
                if (AttemptBreakThroughWall())
                {
                    Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
                }
            }
            else if (collision.impulse.magnitude > m_BouncebackThreshold)
            {
                Vector3 direction = collision.contacts[0].point - collision.collider.transform.position;
                direction = direction.normalized;

                collision.collider.gameObject.GetComponent<Rigidbody>().AddForce(-direction * m_BouncebackForce);
            }
        }
    }

    //returns true if successful
    private bool AttemptBreakThroughWall()
    {
        m_CurrentHealth--;

        if (m_CurrentHealth <= 0)
        {
            return true;
        }

        else
        {
            return false;
        }
    }
}
