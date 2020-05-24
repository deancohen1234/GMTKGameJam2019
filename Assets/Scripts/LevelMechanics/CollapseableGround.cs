﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//when ground is hit enough times, piece of ground falls
    //changes Justice Guards around stage
    //if player on collapsing ground, they die

//each ground has a connect chunk
//if there is no connected chunk then piece can fall

public class CollapseableGround : MonoBehaviour
{
    public LevelMechanicsContainer m_LevelMechanic;
    public CollapseableGround m_ConnectedChunk;
    public GameObject m_ConnectedWall;
    public int m_NumberOfHitsToBreak = 3;
    public bool m_IsUnbreakable = false;

    public float m_PerlinScale = 0.5f;
    public float m_MaxShake = 1.0f;
    public float m_Frequecy = 1.0f;

    public Action m_OnCollapse;

    private Animator m_Animator;

    private Vector3 m_StartingPos;
    private int m_CurrentNumHitsRemaining = 0;
    private bool m_IsCollapsed = false;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_StartingPos = transform.position;

        m_CurrentNumHitsRemaining = m_NumberOfHitsToBreak;

        if (m_ConnectedChunk != null)
        {
            m_ConnectedWall.SetActive(false);
        }
    }

    private void Update()
    {
        //VibrateGround();
    }

    private void OnEnable()
    {
        if (m_ConnectedChunk != null)
        {
            m_ConnectedChunk.m_OnCollapse += OnConnectChunkCollapse;
        }

        m_LevelMechanic.m_OnRoundEnd += OnRoundEnd;
    }

    private void OnDisable()
    {
        if (m_ConnectedChunk != null)
        {
            m_ConnectedChunk.m_OnCollapse -= OnConnectChunkCollapse;
        }

        if (m_LevelMechanic.m_OnRoundEnd != null)
        {
            m_LevelMechanic.m_OnRoundEnd -= OnRoundEnd;
        }
    }

    //called from Hammer
    public void HitGround()
    {
        //if connected chunk exists, and it's either collapsed or unbreakable then bail out of function
        if (m_ConnectedChunk != null && (m_ConnectedChunk.IsCollapsed() || m_IsUnbreakable)) { return; }

        //decrease total hit count by 1
        m_CurrentNumHitsRemaining--;

        Debug.Log("Chunk: " + transform.name + " hit. " + m_CurrentNumHitsRemaining + " Left");

        //if hit is less than 0 then trigger collapse
        if (m_CurrentNumHitsRemaining <= 0)
        {
            DelayToCollapse(3.0f);
        }
    }

    private void DelayToCollapse(float timeToCollapse)
    {
        Invoke("TriggerCollapse", timeToCollapse);
    }

    private void TriggerCollapse()
    {
        //play falling animation
        m_Animator.SetBool("IsCollapsed", true);

        m_ConnectedWall.SetActive(false);

        //if player is in area (maybe by doing a raycast from the player down to see if they hit the falling piece)
        PlayerController p1 = m_LevelMechanic.GetPlayerOne();
        PlayerController p2 = m_LevelMechanic.GetPlayerTwo();

        if (IsPlayerOnCollapsedGround(p1)) { p1.GetHealthComponent().Kill(); }
        if (IsPlayerOnCollapsedGround(p2)) { p2.GetHealthComponent().Kill(); }

        m_OnCollapse?.Invoke();
    }

    private bool IsPlayerOnCollapsedGround(PlayerController player)
    {
        Ray ray = new Ray(player.transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 5.0f, ~LayerMask.NameToLayer("IgnoreFloor"))) //ignore everything but "ignorefloor"
        {
            CollapseableGround ground = hit.transform.gameObject.GetComponent<CollapseableGround>();
            if (ground.Equals(this))
            {
                return true;
            }
        }

        return false;
    }
    //called from Update
    private void VibrateGround()
    {
        float perlinValueX = Mathf.PerlinNoise((1 / 128 * m_PerlinScale) + Time.time * m_Frequecy, (1 / 128 * m_PerlinScale) + Time.time);
        float perlinValueZ = Mathf.PerlinNoise((1 / 128 * m_PerlinScale) + Time.time * m_Frequecy, (1 / 128 * m_PerlinScale) + Time.time + 1);

        float x = m_MaxShake * ((perlinValueX * 2) - 1); //puts perlin between -1, 1 (x is not between -1 and 1)
        float z = m_MaxShake * ((perlinValueZ * 2) - 1); //puts perlin between -1, 1

        Vector3 newPosition = new Vector3(m_StartingPos.x + x, transform.position.y, m_StartingPos.z + z);
        Debug.Log("New Pos: " + newPosition);
        transform.position = newPosition;
    }

    private void OnConnectChunkCollapse()
    {
        m_ConnectedWall.SetActive(true);
        m_IsCollapsed = true;
    }

    private void OnRoundEnd()
    {
        m_Animator.SetBool("IsCollapsed", false);

        ResetGround();
    }

    public void ResetGround()
    {
        m_IsCollapsed = false;
        m_CurrentNumHitsRemaining = m_NumberOfHitsToBreak;
        m_ConnectedWall.SetActive(true);

        if (m_ConnectedChunk != null)
        {
            m_ConnectedWall.SetActive(false);
        }
    }

    public bool IsCollapsed()
    {
        return m_IsCollapsed;
    }
}