using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TriggerCollider : MonoBehaviour
{
    public Action<Collider, GameObject> m_OnTriggerEnter;
    public Action<Collider, GameObject> m_OnTriggerStay;
    public Action<Collider, GameObject> m_OnTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        m_OnTriggerEnter?.DynamicInvoke(other, gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        m_OnTriggerStay?.DynamicInvoke(other, gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        m_OnTriggerExit?.DynamicInvoke(other, gameObject);
    }
}
