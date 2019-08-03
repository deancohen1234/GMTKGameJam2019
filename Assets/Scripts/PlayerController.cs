﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum PlayerType {Player1, Player2}
public enum PlayerOrientation { Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft}

public struct InputStrings
{
    public string Horizontal;
    public string Vertical;
    public string RTrigger;
    public string LTrigger;
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HealthComponent))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public PlayerType m_PlayerNum;

    [Header("Move Properties")]
    public float m_MoveSpeed = 5.0f;

    [Header("Dash Properties")]
    public float m_DashSpeed = 15.0f;
    public float m_DisabledMovementTime = 0.10f; //in seconds

    [Header("Attack Properties")]
    public float m_AttackDistance = 1.0f;
    public float m_AttackSphereRadius = 0.35f;
    public float m_KnockbackForce = 2500.0f;

    public SpriteHandler m_SpriteHandler;
    public AttackHitboxController m_AttackHitboxController;

    private InputStrings m_InputStrings;
    private Rigidbody m_Rigidbody;
    private HealthComponent m_HealthComponent;

    [Header("Actions")]
    public PlayerAction m_DashAction;
    public PlayerAction m_AttackAction;
    public PlayerAction m_DisarmAction;

    [Header("Testing")]
    public GameObject m_HitboxTestObject;

    private PlayerAction m_RTriggerAction; //abstract class so we can swap in disarm or attack

    private MegaWeapon m_EquippedWeapon;
    private bool m_HasWeapon;
    private bool m_IsDisarming;

    private PlayerOrientation m_PlayerOrientation;
    private bool m_CanMove = true;
    private Vector3 m_AttackDirection; //need to store attack direction for dash attacking

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_HealthComponent = GetComponent<HealthComponent>();
    }
    // Start is called before the first frame update
    void Start()
    {
        SetupInputStrings(m_PlayerNum);

        m_DashAction.IsAvailable = true;

        //default to disarmAction
        m_RTriggerAction = m_DisarmAction;
    }

    private void OnEnable()
    {
        m_DashAction.OnActionStart += OnDashStart;
        m_DashAction.OnActionEnd += OnDashEnd;
        m_DashAction.ActionHandler += Dash;

        m_AttackAction.ActionHandler += Attack;
        m_AttackAction.OnActionEnd += OnAttackEnd;

        m_DisarmAction.ActionHandler += Disarm;
        m_DisarmAction.OnActionStart += OnDisarmStart;
        m_DisarmAction.OnActionEnd += OnDisarmEnd;

        m_HealthComponent.m_OnDeath += OnPlayerDeath;
    }

    private void OnDisable()
    {
        m_DashAction.OnActionStart -= OnDashStart;
        m_DashAction.OnActionEnd -= OnDashEnd;
        m_DashAction.ActionHandler -= Dash;

        m_AttackAction.ActionHandler -= Attack;
        m_AttackAction.OnActionEnd -= OnAttackEnd;

        m_DisarmAction.ActionHandler -= Disarm;
        m_DisarmAction.OnActionStart -= OnDisarmStart;
        m_DisarmAction.OnActionEnd -= OnDisarmEnd;

        m_HealthComponent.m_OnDeath -= OnPlayerDeath;
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

        if (Input.GetAxis(m_InputStrings.LTrigger) >= 0.8f && m_DashAction.IsAvailable)
        {
            m_DashAction.ExecuteAction();
        }

        if (Input.GetAxis(m_InputStrings.RTrigger) >= 0.8f && m_DashAction.IsAvailable)
        {
            m_RTriggerAction.ExecuteAction();
        }

        //if dash has been started
        m_DashAction.CheckActionCompleteness(x, y);
        m_AttackAction.CheckActionCompleteness(x, y);
        m_DisarmAction.CheckActionCompleteness(x, y);


        m_PlayerOrientation = CalculateOrientation(new Vector2(x, y).normalized);

        if (new Vector2(x, y).sqrMagnitude >= .2f)
        {
            m_SpriteHandler.SetSprite(m_PlayerOrientation);
        }

    }

    private PlayerOrientation CalculateOrientation(Vector2 vector)
    {
        PlayerOrientation orientation = PlayerOrientation.Up;
        float atan2 = Mathf.Atan2(vector.y, vector.x);

        atan2 = atan2 * Mathf.Rad2Deg;

        if (IsInRange(atan2, 67.5f, 112.5f))
        {
            //12 o clock
            orientation = PlayerOrientation.Up;
        }
        else if (IsInRange(atan2, 22.5f, 67.5f))
        {
            //1:30
            orientation = PlayerOrientation.UpRight;
        }
        else if (IsInRange(atan2, -22.5f, 22.5f))
        {
            //3:00
            orientation = PlayerOrientation.Right;
        }
        else if (IsInRange(atan2, -67.5f, -22.5f))
        {
            //4:30
            orientation = PlayerOrientation.DownRight;
        }
        else if (IsInRange(atan2, -112.5f, -67.5f))
        {
            //6:00
            orientation = PlayerOrientation.Down;
        }
        else if (IsInRange(atan2, -157.5f, -112.5f))
        {
            orientation = PlayerOrientation.DownLeft;
        }
        else if (IsInRange(atan2, 112.5f, 157.5f))
        {
            //10:30
            orientation = PlayerOrientation.UpLeft;
        }
        else
        {
            //9
            orientation = PlayerOrientation.Left;
        }

        return orientation;
    }

    private void Move(float x, float y)
    {
        Vector3 inputVelocity = new Vector3(x, 0f, y) * m_MoveSpeed; //no delta time because physcis is doing that for us

        m_Rigidbody.velocity = inputVelocity;
    }

    public void EquipWeapon(MegaWeapon weapon)
    {
        if (m_HasWeapon == false)
        {
            m_EquippedWeapon = weapon;

            m_HasWeapon = true;
            m_RTriggerAction = m_AttackAction;
        }
    }

    public void LoseWeapon()
    {
        m_EquippedWeapon.Unequip();
        m_EquippedWeapon = null;

        m_HasWeapon = false;

        m_RTriggerAction.ForceStopAction();

        m_RTriggerAction = m_DisarmAction;
    }

    #region Action Methods
    private void Dash(Vector3 direction)
    {
        StartCoroutine(DisablePlayerMovement(m_DisabledMovementTime));

        m_Rigidbody.velocity = direction * m_DashSpeed;
    }

    public void AttemptAttack(PlayerController attackingPlayer)
    {
        if (m_IsDisarming)
        {
            //attacking player loses weapon, no damage
            ApplyBounceBackForce(attackingPlayer);

            attackingPlayer.LoseWeapon();

            m_RTriggerAction.ForceStopAction();
        }
        else
        {
            m_HealthComponent.DealDamage(200f);
        }
    }

    private void Attack(Vector3  direction)
    {
        Dash(direction);

        m_AttackHitboxController.ActivateHitBox(m_PlayerOrientation);
    }

    private void OnAttackEnd()
    {
        m_AttackHitboxController.DisableAllHitBoxes();
    }

    private IEnumerator DisablePlayerMovement(float time)
    {
        m_CanMove = false;

        yield return new WaitForSeconds(time);

        m_CanMove = true;
    }

    private void OnDashStart()
    {
        //GetComponent<Renderer>().material.color = Random.ColorHSV();
        m_SpriteHandler.GetComponent<SpriteRenderer>().color = UnityEngine.Random.ColorHSV();
    }

    private void OnDashEnd()
    {
        //GetComponent<Renderer>().material.color = Color.white;
        m_SpriteHandler.GetComponent<SpriteRenderer>().color = Color.white;
    }

    private void OnDisarmStart()
    {
        m_SpriteHandler.GetComponent<SpriteRenderer>().color = Color.black;
    }

    private void Disarm(Vector3 direction)
    {
        m_IsDisarming = true;
    }

    private void OnDisarmEnd()
    {
        Debug.Log("Ending disarm");
        m_IsDisarming = false;
        m_SpriteHandler.GetComponent<SpriteRenderer>().color = Color.white;
    }
    #endregion

    //player dies
    private void OnPlayerDeath()
    {
        gameObject.SetActive(false);
    }

    //inclusive min and exclusive max
    private bool IsInRange(float value, float min, float max)
    {
        if (value >= min && value < max)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ApplyBounceBackForce(PlayerController otherPlayer)
    {
        DisablePlayerMovement(0.2f);

        Vector3 direction = otherPlayer.gameObject.transform.position - transform.position;

        m_Rigidbody.AddForce(-direction.normalized * m_KnockbackForce);
        otherPlayer.m_Rigidbody.AddForce(direction.normalized * m_KnockbackForce);

        /*m_Rigidbody.velocity = -direction.normalized * m_KnockbackForce;
        otherPlayer.m_Rigidbody.velocity = direction.normalized * m_KnockbackForce;*/

    }
}

[System.Serializable]
public class PlayerAction
{
    [HideInInspector]
    public bool IsAvailable = true;
    [HideInInspector]
    public bool IsExecuting = false;

    public int CooldownTime; //in frames
    public int StartDelay; //in frames
    public int ActionLength; //in frames

    public Action OnActionStart;
    public Action OnActionEnd;
    public Action<Vector3> ActionHandler; //holds references to other scripts that actually do action lik dash and attack

    private int StartingFrame;

    public void SetStartingFrame(int frameNum)
    {
        StartingFrame = frameNum;
    }

    public int GetStartingFrame()
    {
        return StartingFrame;
    }

    public void ExecuteAction()
    {
        StartingFrame = Time.frameCount;
        IsAvailable = false;
        IsExecuting = true;

        if (OnActionStart != null)
        {
            OnActionStart.Invoke();
        }
    }

    public void ForceStopAction()
    {
        IsExecuting = false;

        OnActionEnd.Invoke();
    }

    //called every frame once action starts
    public void CheckActionCompleteness(float xInput, float yInput)
    {
        //if dash has been started
        if (IsExecuting)
        {
            if (Time.frameCount - StartingFrame >= ActionLength + StartDelay)
            {
                //End dash
                StartingFrame = Time.frameCount; //star timer for cooldown
                IsExecuting = false;

                if (OnActionEnd != null)
                {
                    OnActionEnd.Invoke();
                }
            }
            //start delay finished
            else if (Time.frameCount - StartingFrame >= StartDelay)
            {
                //Do dash
                Vector3 direction = new Vector3(xInput, 0, yInput);

                ActionHandler.DynamicInvoke(direction);
            }

        }

        //if action is complete and cooldown still is going on
        else if (!IsAvailable)
        {
            //check cooldown
            if (Time.frameCount - StartingFrame >= CooldownTime)
            {
                StartingFrame = 0;
                IsAvailable = true;
            }
        }
    }
}
