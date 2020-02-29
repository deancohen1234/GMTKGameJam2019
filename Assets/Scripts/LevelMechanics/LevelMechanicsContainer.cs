using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//called from Round Manager that holds basic information for level mechanics to use
//Singleton
public class LevelMechanicsContainer : MonoBehaviour
{
    private RoundManager m_RoundManager;

    public void InitializeContainer(RoundManager roundManager)
    {
        m_RoundManager = roundManager;
    }

    public PlayerController GetPlayerOne()
    {
        return m_RoundManager.GetPlayer(0);
    }

    public PlayerController GetPlayerTwo()
    {
        return m_RoundManager.GetPlayer(1);
    }
}
