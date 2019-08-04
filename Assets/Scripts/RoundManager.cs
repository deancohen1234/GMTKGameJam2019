using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public PlayerController PlayerOne;
    public PlayerController PlayerTwo;

    private void Start()
    {
        PlayerOne.GetHealthComponent().m_OnDeath += OnRoundComplete;
        PlayerTwo.GetHealthComponent().m_OnDeath += OnRoundComplete;
    }

    private void OnDisable()
    {

    }

    public void OnRoundComplete()
    {
        if (PlayerOne.GetHealthComponent().IsDead())
        {
            //PlayerTwo wins
            Debug.Log("Player 2 wins");
        }
        else
        {
            //PlayerOne wins
            Debug.Log("Player 1 wins");
        }
    }

    private void OnDestroy()
    {
        PlayerOne.GetHealthComponent().m_OnDeath -= OnRoundComplete;
        PlayerTwo.GetHealthComponent().m_OnDeath -= OnRoundComplete;
    }
}
