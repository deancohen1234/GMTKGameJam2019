using System.Collections;
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
    public int m_DashStartDelay = 15; //in frames
    public int m_DashLength = 21; //in frames
    public float m_DashCooldown = 0.5f;

    [Header("Sprite Handling")]
    public SpriteHandler m_SpriteHandler;

    private InputStrings m_InputStrings;
    private Rigidbody m_Rigidbody;

    private bool m_CanMove = true;
    private bool m_HasDash = true;

    private int m_FrameStarted = -1;
    private bool m_IsDashing;

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
            m_FrameStarted = Time.frameCount;
            m_IsDashing = true;
            m_HasDash = false;
            OnDashStart();
        }

        //if dash has been started
        if (m_IsDashing)
        {
            if (Time.frameCount - m_FrameStarted >= m_DashLength + m_DashStartDelay)
            {
                //End dash
                m_FrameStarted = 0; //reset counter
                OnDashEnd();
            }
            else if (Time.frameCount - m_FrameStarted >= m_DashStartDelay)
            {
                //Do dash
                Vector3 direction = new Vector3(x, 0, y);
                Dash(direction.normalized);
            }
            
        }

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

    private void OnDashStart()
    {
        //GetComponent<Renderer>().material.color = Random.ColorHSV();
        m_SpriteHandler.GetComponent<SpriteRenderer>().color = Random.ColorHSV();
    }

    private void OnDashEnd()
    {
        //GetComponent<Renderer>().material.color = Color.white;
        m_SpriteHandler.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
