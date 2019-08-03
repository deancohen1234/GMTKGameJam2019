﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType {Player1, Player2}

public struct InputStrings
{
    public string Horizontal;
    public string Vertical;
    public string RTrigger;
    public string LTrigger;
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public PlayerType m_PlayerNum;

    [Header("Move Properties")]
    public float m_MoveSpeed = 5.0f;

    [Header("Dash Properties")]
    public float m_DashSpeed = 15.0f;
    public float m_DisabledMovementTime = 0.10f; //in seconds
    public float m_DashCooldown = 0.5f;

    private InputStrings m_InputStrings;
    private Rigidbody m_Rigidbody;

    private bool m_CanMove = true;
    private bool m_HasDash = true;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        SetupInputStrings(m_PlayerNum);
    }

    private void SetupInputStrings(PlayerType playerNum)
    {
        switch (playerNum)
        {
            case PlayerType.Player1:
                m_InputStrings.Horizontal = "P1_Horizontal";
                m_InputStrings.Vertical = "P1_Vertical";
                m_InputStrings.RTrigger = "P1_RTrigger";
                m_InputStrings.LTrigger = "P1_LTrigger";
                break;
            case PlayerType.Player2:
                m_InputStrings.Horizontal = "P2_Horizontal";
                m_InputStrings.Vertical = "P2_Vertical";
                m_InputStrings.RTrigger = "P2_RTrigger";
                m_InputStrings.LTrigger = "P2_LTrigger";
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis(m_InputStrings.Horizontal);
        float y = Input.GetAxis(m_InputStrings.Vertical);

        if (m_CanMove)
        {
            Move(x, y);
        }

        if (Input.GetAxis(m_InputStrings.LTrigger) >= 0.8f && m_HasDash)
        {
            Vector3 direction = new Vector3(x, 0, y);
            Dash(direction.normalized);
        }

    }

    private void Move(float x, float y)
    {
        Vector3 inputVelocity = new Vector3(x, 0f, y) * m_MoveSpeed; //no delta time because physcis is doing that for us

        m_Rigidbody.velocity = inputVelocity;
    }

    private void Dash(Vector3 direction)
    {
        StartCoroutine(DisablePlayerMovement(m_DisabledMovementTime));
        StartCoroutine(CooldownDash(m_DashCooldown));

        m_Rigidbody.velocity = direction * m_DashSpeed;
    }

    private IEnumerator DisablePlayerMovement(float time)
    {
        m_CanMove = false;

        yield return new WaitForSeconds(time);

        m_CanMove = true;
    }

    private IEnumerator CooldownDash(float time)
    {
        m_HasDash = false;

        yield return new WaitForSeconds(time);

        m_HasDash = true;
    }
}