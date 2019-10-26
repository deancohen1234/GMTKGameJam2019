using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadLauncher : MonoBehaviour
{
    public GameObject m_HeadPrefab;

    public float m_Force;
    public float m_LaunchVariance = 0.5f;

    public void LaunchHead(Vector3 startPos, Vector3 goalPos)
    {
        GameObject head = Instantiate(m_HeadPrefab, startPos, Quaternion.Euler(42.0f, 0, 0));
        head.transform.position = startPos;

        head.GetComponent<Head>().SetSelfDestruct(2.0f);

        Vector3 randomVariance = new Vector3(Random.Range(-m_LaunchVariance, m_LaunchVariance), Random.Range(m_LaunchVariance, m_LaunchVariance), 0);
        Vector3 dirToCamera = (goalPos + randomVariance) - head.transform.position;

        Rigidbody r = head.GetComponent<Rigidbody>();

        r.AddForce(dirToCamera * m_Force);
    }
}
