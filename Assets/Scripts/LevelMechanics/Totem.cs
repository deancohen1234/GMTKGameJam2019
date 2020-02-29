using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Totem : MonoBehaviour
{
    public Material m_TotemMat;
    public float m_CooldownTime = 3f; //in seconds

    private PlayerType m_PlayerType;
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

    public void SetActivity(bool isActive, PlayerType playerType)
    {
        m_IsActivated = isActive;
        m_LastChangeTime = Time.time;
        m_PlayerType = playerType;
    }

    public bool IsActivated()
    {
        return m_IsActivated;
    }

    public PlayerType GetPlayerActivated()
    {
        return m_PlayerType;
    }

    public void SetTotemColor(Color color)
    {
        m_TotemMat.SetColor("_Color", color);
        m_TotemMat.SetColor("_EmissionColor", color);
    }
}
