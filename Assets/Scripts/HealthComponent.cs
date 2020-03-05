using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO add iFrames
public class HealthComponent : MonoBehaviour
{
    public float m_StartingHealth;

    public Action<PlayerController, float> m_OnPlayerDamaged;
    public Action m_OnDeath;

    private float m_CurrentHealth;
    private float m_DamageMultiplier = 1.0f;
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
        m_CurrentHealth = Mathf.Clamp(m_CurrentHealth - (damage * m_DamageMultiplier), 0, m_StartingHealth);

        m_OnPlayerDamaged?.DynamicInvoke(gameObject.GetComponent<PlayerController>(), m_CurrentHealth);

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

    public void Kill()
    {
        DealDamage(10000000000);
    }

    public bool IsDead()
    {
        return m_IsDead;
    }

    public void SetDamageMultiplier(float multiplier)
    {
        m_DamageMultiplier = multiplier;
    }
}
