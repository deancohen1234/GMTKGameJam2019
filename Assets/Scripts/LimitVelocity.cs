using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LimitVelocity : MonoBehaviour
{
    public float m_MaxMagnitude = 10f;

    private Rigidbody m_RB;

    private void Start()
    {
        m_RB = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 velocity = m_RB.velocity;
        float magnitude = m_RB.velocity.magnitude;

        float clampedMagnitude = Mathf.Clamp(magnitude, 0, m_MaxMagnitude);
        Vector3 newVelocity = velocity.normalized * clampedMagnitude;

        m_RB.velocity = newVelocity;
    }
}
