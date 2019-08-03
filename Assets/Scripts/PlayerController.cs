using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    [Header("Attack Properties")]
    public float m_AttackDistance = 1.0f;

    [Header("Sprite Handling")]
    public SpriteHandler m_SpriteHandler;

    private InputStrings m_InputStrings;
    private Rigidbody m_Rigidbody;

    [Header("Actions")]
    public PlayerAction m_DashAction;
    public PlayerAction m_AttackAction;

    [Header("Testing")]
    public GameObject m_HitboxTestObject;

    private bool m_CanMove = true;
    private Vector3 m_AttackDirection; //need to store attack direction for dash attacking

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        SetupInputStrings(m_PlayerNum);

        m_DashAction.IsAvailable = true;
    }

    private void OnEnable()
    {
        m_DashAction.OnActionStart += OnDashStart;
        m_DashAction.OnActionEnd += OnDashEnd;
        m_DashAction.ActionHandler += Dash;

        m_AttackAction.ActionHandler += StartLungeAttack;
        m_AttackAction.OnActionEnd += Attack;
    }

    private void OnDisable()
    {
        m_DashAction.OnActionStart -= OnDashStart;
        m_DashAction.OnActionEnd -= OnDashEnd;
        m_DashAction.ActionHandler -= Dash;

        m_AttackAction.ActionHandler -= StartLungeAttack;
        m_AttackAction.OnActionEnd -= Attack;
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
            m_AttackAction.ExecuteAction();
        }

        //if dash has been started
        m_DashAction.CheckActionCompleteness(x, y);
        m_AttackAction.CheckActionCompleteness(x, y);

        m_SpriteHandler.SetSprite(new Vector2(x, y).normalized);

    }

    private void Move(float x, float y)
    {
        Vector3 inputVelocity = new Vector3(x, 0f, y) * m_MoveSpeed; //no delta time because physcis is doing that for us

        m_Rigidbody.velocity = inputVelocity;
    }

    private void Dash(Vector3 direction)
    {
        StartCoroutine(DisablePlayerMovement(m_DisabledMovementTime));

        m_Rigidbody.velocity = direction * m_DashSpeed;
    }

    private void StartLungeAttack(Vector3 direction)
    {
        //first dash
        //then attack
        Dash(direction);

        m_AttackDirection = direction;
    }

    private void Attack()
    {
        GameObject g = Instantiate(m_HitboxTestObject);
        Vector3 attackPosition = transform.position;

        g.transform.position = transform.position;

        Debug.Log("Our Position: " + transform.position);
        Debug.Log("Its Position: " + m_HitboxTestObject.transform.position);
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
