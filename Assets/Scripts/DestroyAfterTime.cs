using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float m_TimeToLive = 3f;

    public void OnEnable()
    {
        Invoke("DestroyMyself", m_TimeToLive);
    }

    private void DestroyMyself()
    {
        Destroy(gameObject);
    }
}
