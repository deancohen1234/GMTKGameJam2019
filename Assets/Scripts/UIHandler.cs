using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    public PlayerController m_PlayerOne;
    public PlayerController m_PlayerTwo;

    private void OnEnable()
    {
        m_PlayerOne.m_OnPlayerDamaged += OnPlayerDamaged;
        m_PlayerTwo.m_OnPlayerDamaged += OnPlayerDamaged;
    }

    private void OnDisable()
    {
        m_PlayerOne.m_OnPlayerDamaged -= OnPlayerDamaged;
        m_PlayerTwo.m_OnPlayerDamaged -= OnPlayerDamaged;
    }

    private void OnPlayerDamaged(int playerIndex)
    {
        Debug.Log("Player Damaged: " + playerIndex);
    }
}
