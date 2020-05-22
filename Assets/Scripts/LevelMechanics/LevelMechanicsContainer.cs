using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//TODO maybe make all level mechanics inherit this in some fashion

//called from Round Manager that holds basic information for level mechanics to use
//Singleton
public class LevelMechanicsContainer : MonoBehaviour
{
    public Action m_OnRoundEnd;
    private RoundManager m_RoundManager;

    public void InitializeContainer(RoundManager roundManager)
    {
        m_RoundManager = roundManager;
    }

    public void EndRound()
    {
        Debug.Log("End Round...");
        m_OnRoundEnd?.Invoke();
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
