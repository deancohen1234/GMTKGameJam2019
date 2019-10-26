using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadLauncher : MonoBehaviour
{
    public GameObject m_HeadPrefab;
    public Transform m_Camera;

    public float m_Force;
    public float m_LaunchVariance = 0.5f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            //Testing
            LaunchHead(Vector3.zero + new Vector3(0, 0, 0));
        }
    }

    public void LaunchHead(Vector3 startPos)
    {
        GameObject head = Instantiate(m_HeadPrefab, startPos, Quaternion.identity);
        head.transform.position = startPos;

        head.GetComponent<Head>().SetSelfDestruct(2.0f);

        Vector3 randomVariance = new Vector3(Random.Range(-m_LaunchVariance, m_LaunchVariance), Random.Range(m_LaunchVariance, m_LaunchVariance), 0);
        Vector3 dirToCamera = (m_Camera.position + randomVariance) - head.transform.position;

        Rigidbody r = head.GetComponent<Rigidbody>();

        r.AddForce(dirToCamera * m_Force);
    }
}
