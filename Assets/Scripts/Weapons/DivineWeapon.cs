﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivineWeapon : MonoBehaviour
{
    public Transform m_ArenaCenter;
    public float m_ArenaWidth;
    public float m_ArenaHeight;
    public Vector3 m_Offset;

    public float m_FallSpeed = 0.3f;
    [Header("Player Modifiers")]
    public float m_PlayerSpeedModifier = 1.0f; // will be multiplyed by player default speed

    [Header("Action Details")]
    public PlayerAction m_AttackAction;

    [Header("Sounds")]
    public AudioClip m_AttackSound;

    protected PlayerController m_PlayerRef;

    private AudioSource m_AudioSource;
    private SpriteRenderer m_SpriteRenderer;
    private Rigidbody m_Rigidbody;
    private Collider[] m_AllColliders;

    private float m_WeaponStartHeight;
    private bool m_IsLerping;
    private float m_StartFallTime;
    private Vector3 m_DestinationPos;
    private Vector3 m_StartPos;
    private float m_Timer;

    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_AllColliders = GetComponents<Collider>();
    }

    private void Start()
    {
        OverrideStart();
    }

    protected virtual void OverrideStart()
    {
        m_WeaponStartHeight = transform.position.y;

        //SetWeaponActive(false);

        //assign all actions for delegate when game starts
        m_AttackAction.OnActionStart += OnWeaponAttackStart;
        m_AttackAction.ActionHandler += WeaponAttack; //currently isn't set up
        m_AttackAction.OnActionEnd += OnWeaponAttackEnd;
    }

    //When closing remove all delegates
    private void OnDestroy()
    {
        m_AttackAction.OnActionStart -= OnWeaponAttackStart;
        m_AttackAction.ActionHandler -= WeaponAttack;
        m_AttackAction.OnActionEnd -= OnWeaponAttackEnd;
    }

    protected virtual void Update()
    {
        if (m_IsLerping)
        {
            //lerp downward
            Vector3 newLerpPos = Vector3.Lerp(m_StartPos, m_DestinationPos, m_Timer);
            transform.position = newLerpPos;

            m_Timer += Time.deltaTime * m_FallSpeed;

            if (m_Timer >= 1.0f)
            {
                m_Timer = 0;
                m_IsLerping = false;
                transform.position = m_DestinationPos;

                //helps get dagger off tip of stalagmite
                GetComponent<Rigidbody>().AddForce(transform.forward * 5);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Pickup weapon
            other.gameObject.GetComponent<PlayerController>().EquipWeapon(this);
            //OnWeaponPickup(other.gameObject.GetComponent<PlayerController>());
            SetWeaponActive(false);
        }
    }

    //on weapon pickup set player reference for this weapon's attack action
    public virtual void OnWeaponPickup(PlayerController player)
    {
        m_PlayerRef = player;
        m_AttackAction.SetPlayerReference(player);
    }

    protected virtual void OnWeaponDropped()
    {
        m_PlayerRef = null;
        m_AttackAction.SetPlayerReference(null); //set reference for actions to null
    }

    public virtual void OnWeaponAttackStart()
    {
        m_PlayerRef.PlaySound(m_AttackSound);
    }

    public virtual void WeaponAttack(PlayerController player, Vector3 direction)
    {

    }

    public virtual void OnWeaponAttackEnd()
    {

    }

    //player drops weapon
    //has parameter of the player who dropped the weapon
    public virtual void Drop(PlayerController lastControlledPlayer)
    {
        RandomizeLocationFromPlayer(lastControlledPlayer.transform.position);
        OnWeaponDropped();
    }

    //retur true if hit was successful
    public virtual bool OnHit(PlayerController hitPlayer, PlayerController attackingPlayer)
    {
        //if is disarming return false
        if (hitPlayer.IsDisarming())
        {
            hitPlayer.DisarmOppponent(attackingPlayer);
            return false;
        }
        else
        {
            return true;
        }
    }

    //when weapon is picked up set weapon inactive without disabling monobehavior
    public void SetWeaponActive(bool isActive)
    {
        //hide/show sprite
        m_SpriteRenderer.enabled = isActive;

        //freeze all rigidbody control on inactive
        m_Rigidbody.constraints = isActive ? RigidbodyConstraints.FreezeRotation : RigidbodyConstraints.FreezeAll;
        
        //set all colliders to enabled/disabled
        for (int i = 0; i < m_AllColliders.Length; i++)
        {
            m_AllColliders[i].enabled = isActive;
        }

        //set all children to active/inactive
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(isActive);
        }
    }

    public void RandomizeLocation()
    {
        SetWeaponActive(true);

        float randomX = Random.Range(-1.0f, 1.0f) * m_ArenaWidth;
        float randomY = Random.Range(-1.0f, 1.0f) * m_ArenaHeight;
        Vector3 newPosition = m_ArenaCenter.position + m_Offset + new Vector3(randomX, 0, randomY);
        newPosition.y = transform.position.y;

        m_DestinationPos = newPosition;
        m_StartPos = m_DestinationPos + new Vector3(0, 7.5f, 0);

        transform.position = m_StartPos;
        m_IsLerping = true;
    }

    public void RandomizeLocationFromPlayer(Vector3 playerPos)
    {
        SetWeaponActive(true);

        float randomX = Random.Range(-1.0f, 1.0f) * m_ArenaWidth;
        float randomY = Random.Range(-1.0f, 1.0f) * m_ArenaHeight;
        Vector3 newPosition = m_ArenaCenter.position + m_Offset + new Vector3(randomX, 0, randomY);
        newPosition.y = transform.position.y;

        m_DestinationPos = newPosition;
        m_StartPos = playerPos + new Vector3(0, 0.5f, 0);

        transform.position = m_StartPos;
        m_IsLerping = true;
    }

    private bool IsValidPosition(Vector3 position)
    {
        bool check = Physics.CheckSphere(position + new Vector3(0, 0.5f, 0), 0.1f);

        //if true then good position, if false then try a new position
        return !check;
    }
}
