using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsManager : MonoBehaviour
{
    public static CreditsManager m_Singleton;

    private int m_NumCredits = 0;

    private void Awake()
    {
        if (m_Singleton != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            m_Singleton = this;
            DontDestroyOnLoad(gameObject);
        }

    }

    public int ModifyCredits(int numCredits)
    {
        m_NumCredits += numCredits;
        return m_NumCredits;
    }

    public int GetNumCredits()
    {
        return m_NumCredits;
    }
}
