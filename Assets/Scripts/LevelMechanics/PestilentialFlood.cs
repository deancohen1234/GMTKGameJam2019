using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PestilentialFlood : MonoBehaviour
{
    public LevelMechanicsContainer m_Container;
    public Animator m_Slime;
    public Totem[] m_Totems;
    public Sprite m_2XWeaponIcon;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_Totems.Length; i++)
        {
            m_Totems[i].GetComponent<TriggerCollider>().m_OnTriggerStay += OnTotemStay;
        }

        m_Container.m_OnRoundEnd += ResetFlood;
    }

    // Update is called once per frame
    void Update()
    {
        //temporary
        if (Input.GetKeyDown(KeyCode.K))
        {
            ActivateFlood();
        }
    }

    void ActivateFlood()
    {
        Debug.Log("Flood is coming...");
        m_Slime.SetTrigger("RaiseSlime");
        m_Container.GetPlayerOne().GetHealthComponent().SetDamageMultiplier(2f);
        m_Container.GetPlayerTwo().GetHealthComponent().SetDamageMultiplier(2f);

        m_Container.GetPlayerOne().SetWeaponIcon(m_2XWeaponIcon);
        m_Container.GetPlayerTwo().SetWeaponIcon(m_2XWeaponIcon);
    }

    private void OnTotemStay(Collider other, GameObject owner)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        Totem totem = owner.GetComponent<Totem>();

        if (player != null && totem != null)
        {
            if (totem.IsChangeable())
            {
                if (player.IsDisarming())
                {
                    OnPlayerActivation(player, totem);
                }
            }

        }
    }

    private void OnPlayerActivation(PlayerController player, Totem totem)
    {
        totem.SetActivity(true);
        Debug.Log("Totem Activated");

        if (AreAllTotemsActivated())
        {
            ActivateFlood();
        }
    }

    //player health doesn't need to be reset because players are destroyed and recreated
    private void ResetFlood()
    {
        //set all totems back to inactive
        for (int i = 0; i < m_Totems.Length; i++)
        {
            m_Totems[i].SetActivity(false);
        }

        m_Slime.SetTrigger("ResetSlime");
    }

    private bool AreAllTotemsActivated()
    {
        //if any totem is not activated, then return false
        for (int i = 0; i < m_Totems.Length; i++)
        {
            if (m_Totems[i].IsActivated() == false)
            {
                return false;
            }
        }

        return true;
    }
}
