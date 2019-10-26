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

        m_PlayerOne.GetHealthComponent().m_OnPlayerDamaged += OnPlayerDamaged;
        m_PlayerTwo.GetHealthComponent().m_OnPlayerDamaged += OnPlayerDamaged;

        //set all health to visible on initialize
        m_P1_Health_1.gameObject.SetActive(true);
        m_P1_Health_2.gameObject.SetActive(true);
        m_P1_Health_3.gameObject.SetActive(true);

        Debug.Log("P1 Health: " + m_P1_Health_1.IsActive());

        m_P2_Health_1.gameObject.SetActive(true);
        m_P2_Health_2.gameObject.SetActive(true);
        m_P2_Health_3.gameObject.SetActive(true);
    }

    private void OnPlayerDamaged(PlayerController player, float currentHealth)
    {
        int divisor = Mathf.CeilToInt(currentHealth / 33.0f);

        if (divisor <= 0)
        {
            Image healthBar_1 = (int)player.m_PlayerNum == 0 ? m_P1_Health_1 : m_P2_Health_1;
            Image healthBar_2 = (int)player.m_PlayerNum == 0 ? m_P1_Health_2 : m_P2_Health_2;
            Image healthBar_3 = (int)player.m_PlayerNum == 0 ? m_P1_Health_3 : m_P2_Health_3;

            healthBar_1.gameObject.SetActive(false);
            healthBar_2.gameObject.SetActive(false);
            healthBar_3.gameObject.SetActive(false);
        }
        else if (divisor == 1)
        {
            Image healthBar_1 = (int)player.m_PlayerNum == 0 ? m_P1_Health_1 : m_P2_Health_1;
            Image healthBar_2 = (int)player.m_PlayerNum == 0 ? m_P1_Health_2 : m_P2_Health_2;
            Image healthBar_3 = (int)player.m_PlayerNum == 0 ? m_P1_Health_3 : m_P2_Health_3;

            healthBar_1.gameObject.SetActive(true);
            healthBar_2.gameObject.SetActive(false);
            healthBar_3.gameObject.SetActive(false);
        }
        else if (divisor == 2)
        {
            Image healthBar_1 = (int)player.m_PlayerNum == 0 ? m_P1_Health_1 : m_P2_Health_1;
            Image healthBar_2 = (int)player.m_PlayerNum == 0 ? m_P1_Health_2 : m_P2_Health_2;
            Image healthBar_3 = (int)player.m_PlayerNum == 0 ? m_P1_Health_3 : m_P2_Health_3;

            healthBar_1.gameObject.SetActive(true);
            healthBar_2.gameObject.SetActive(true);
            healthBar_3.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (m_PlayerOne)
        {
            m_PlayerOne.GetHealthComponent().m_OnPlayerDamaged -= OnPlayerDamaged;
        }

        if (m_PlayerTwo)
        {
            m_PlayerTwo.GetHealthComponent().m_OnPlayerDamaged -= OnPlayerDamaged;
        }
    }
}
