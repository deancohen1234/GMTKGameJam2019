using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public Image m_P1_Health_1;
    public Image m_P1_Health_2;
    public Image m_P1_Health_3;

    public Image m_P2_Health_1;
    public Image m_P2_Health_2;
    public Image m_P2_Health_3;

    private PlayerController m_PlayerOne;
    private PlayerController m_PlayerTwo;

    public void Initialize(PlayerController P1, PlayerController P2)
    {
        m_PlayerOne = P1;
        m_PlayerTwo = P2;

        m_PlayerOne.m_OnPlayerDamaged += OnPlayerDamaged;
        m_PlayerTwo.m_OnPlayerDamaged += OnPlayerDamaged;
    }

    private void OnPlayerDamaged(int playerIndex, float currentHealth)
    {
        Debug.Log("Player Damaged: " + playerIndex);

        Debug.Log("Math: " + (currentHealth / 33.0f));
        int divisor = Mathf.CeilToInt(currentHealth / 33.0f);

        if (divisor <= 0)
        {
            Image healthBar_1 = playerIndex == 1 ? m_P1_Health_1 : m_P2_Health_1;
            Image healthBar_2 = playerIndex == 1 ? m_P1_Health_2 : m_P2_Health_2;
            Image healthBar_3 = playerIndex == 1 ? m_P1_Health_3 : m_P2_Health_3;

            healthBar_1.gameObject.SetActive(false);
            healthBar_2.gameObject.SetActive(false);
            healthBar_3.gameObject.SetActive(false);
        }
        else if (divisor == 1)
        {
            Image healthBar_1 = playerIndex == 1 ? m_P1_Health_1 : m_P2_Health_1;
            Image healthBar_2 = playerIndex == 1 ? m_P1_Health_2 : m_P2_Health_2;
            Image healthBar_3 = playerIndex == 1 ? m_P1_Health_3 : m_P2_Health_3;

            healthBar_1.gameObject.SetActive(true);
            healthBar_2.gameObject.SetActive(false);
            healthBar_3.gameObject.SetActive(false);
        }
        else if (divisor == 2)
        {
            Image healthBar_1 = playerIndex == 1 ? m_P1_Health_1 : m_P2_Health_1;
            Image healthBar_2 = playerIndex == 1 ? m_P1_Health_2 : m_P2_Health_2;
            Image healthBar_3 = playerIndex == 1 ? m_P1_Health_3 : m_P2_Health_3;

            healthBar_1.gameObject.SetActive(true);
            healthBar_2.gameObject.SetActive(true);
            healthBar_3.gameObject.SetActive(false);
        }
    }
}
