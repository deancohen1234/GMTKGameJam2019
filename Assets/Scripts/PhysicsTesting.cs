using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsTesting : MonoBehaviour
{
    public GameObject m_LandingPrefab;
    public Transform m_Destination;

    public float m_ForceMultiplier = 5.0f;

    public float m_Range = 3f;
    public float m_Angle = 45.0f;

    private Rigidbody m_RB;
    // Start is called before the first frame update
    void Start()
    {
        m_RB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Launch();
        }
    }

    private void Launch()
    {
        //Vector3 velocity = new Vector3(Mathf.Cos(45.0f * Mathf.Deg2Rad), Mathf.Sin(45.0f * Mathf.Deg2Rad), 0) * m_ForceMultiplier;
        //Vector3 direction = new Vector3(Mathf.Cos(m_Angle * Mathf.Deg2Rad), Mathf.Sin(m_Angle * Mathf.Deg2Rad), 0);
        Vector3 direction = m_Destination.position - transform.position;
        direction.y = 0;
        m_Range = direction.magnitude;

        direction = direction.normalized;

        direction.x *= Mathf.Cos(m_Angle * Mathf.Deg2Rad);
        direction.z *= Mathf.Cos(m_Angle * Mathf.Deg2Rad);

        direction.y = Mathf.Sin(m_Angle * Mathf.Deg2Rad);

        float velocityMagnitude = Mathf.Sqrt((m_Range * Mathf.Abs(Physics.gravity.y)) / Mathf.Sin(2 * Mathf.Deg2Rad * m_Angle));


        m_RB.velocity = direction.normalized * velocityMagnitude;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject g = Instantiate(m_LandingPrefab);
        g.transform.position = collision.contacts[0].point;
    }
}
