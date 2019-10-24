using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO add iFrames
public class HealthComponent : MonoBehaviour
{
    public float m_StartingHealth;

    public Action m_OnDeath;

    private float m_CurrentHealth;
    private bool m_IsDead;

    // Start is called before the first frame update
    void Start()
    {
        m_CurrentHealth = m_StartingHealth;
    }

    public float GetCurrentHealth()
    {
        return m_CurrentHealth;
    }

    public void DealDamage(float damage)
    {
        m_CurrentHealth = Mathf.Clamp(m_CurrentHealth - damage, 0, m_StartingHealth);

        if (m_CurrentHealth == 0)
        {
            //player is dead
            if (m_OnDeath != null)
            {
                m_IsDead = true;
                m_OnDeath.Invoke();
            }
        }
    }

    public bool IsDead()
    {
        return m_IsDead;
    }
}
