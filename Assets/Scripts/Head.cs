using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : MonoBehaviour
{
    public float m_RotationSpeed = 60f;

    private void Update()
    {
        transform.Rotate(transform.forward, m_RotationSpeed * Time.deltaTime);
    }

    public void SetSelfDestruct(float time)
    {
        Invoke("SelfDestruct", time);

        m_RotationSpeed *= Random.Range(-1.0f, 1.0f);
    }

    private void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
