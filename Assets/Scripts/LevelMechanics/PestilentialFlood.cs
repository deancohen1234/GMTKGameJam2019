using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PestilentialFlood : MonoBehaviour
{
    [Header("References")]
    public LevelMechanicsContainer m_Container;
    public Animator m_Slime;
    public Totem[] m_Totems;

    [Header("Properties")]
    public AudioClip m_Flood;
    public Sprite m_2XWeaponIcon;

    [ColorUsage(true, true)]
    public Color m_PlayerOneTotemColor;

    [ColorUsage(true, true)]
    public Color m_PlayerTwoTotemColor;

    private bool isFlooded;

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
        isFlooded = true;

        GetComponent<AudioSource>().clip = m_Flood;
        GetComponent<AudioSource>().Play();

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
        totem.SetActivity(true, player.m_PlayerNum);

        //set totem color based on player
        Color totemColor = player.m_PlayerNum == PlayerType.Player1 ? m_PlayerOneTotemColor : m_PlayerTwoTotemColor;
        totem.SetTotemColor(totemColor);

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
            m_Totems[i].SetActivity(false, PlayerType.Player1);
            m_Totems[i].SetTotemColor(Color.black);
        }

        if (isFlooded)
        {
            m_Slime.SetTrigger("ResetSlime");
            isFlooded = false;
        }
        
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

        /*
        //check all totems are the same color
        int numPlayerOneTotems = 0;
        for (int j = 0; j < m_Totems.Length; j++)
        {
            if (m_Totems[j].GetPlayerActivated() == PlayerType.Player1)
            {
                numPlayerOneTotems++;
            }
        }
        //if 0 p1 totems they are all p2
        //if Length p1 totems they are all p1
        if (numPlayerOneTotems > 0 && numPlayerOneTotems < m_Totems.Length)
        {
            //there are a mismatch of p1 and p2 totems
            return false;
        }

        return true;
        */
    }

    private void OnDestroy()
    {
        //set all totems back to default color
        for (int i = 0; i < m_Totems.Length; i++)
        {
            m_Totems[i].SetTotemColor(Color.black);
        }
    }
}
