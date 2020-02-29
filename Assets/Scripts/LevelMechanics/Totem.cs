using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Totem : MonoBehaviour
{
    public float m_CooldownTime = 3f; //in seconds

    private float m_LastChangeTime = 0.0f;
    private bool m_IsActivated;

    public bool IsChangeable()
    {
        if (Time.time - m_LastChangeTime >= m_CooldownTime)
        {
            return true;
        }

        return false;
    }

    public void SetActivity(bool isActive)
    {
        m_IsActivated = isActive;
        m_LastChangeTime = Time.time;
    }

    public bool IsActivated()
    {
        return m_IsActivated;
    }
}
