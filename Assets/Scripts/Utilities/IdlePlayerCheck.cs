using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IdlePlayerCheck : MonoBehaviour
{
    public InputManager m_InputManager;

    public float m_CheckInterval = 1.0f; //in seconds
    public float m_TimeToIdle = 30f;

    public UnityEvent m_OnPlayerIdle;

    private void Start()
    {
        InvokeRepeating("CheckPlayerIdleTime", 0, m_CheckInterval);
    }

    void CheckPlayerIdleTime()
    {
        float lastInputTime = m_InputManager.GetLastInputTime();

        if (Time.time - lastInputTime >= m_TimeToIdle)
        {
            m_OnPlayerIdle.Invoke();
            Destroy(this); //kill this object so it can't trigger another idle invoke
        }
    }
}
