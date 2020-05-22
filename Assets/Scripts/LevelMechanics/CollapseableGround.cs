using System;
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

    public float m_PerlinScale = 0.5f;
    public float m_MaxShake = 1.0f;
    public float m_Frequecy = 1.0f;

    public Action m_OnCollapse;

    private Animator m_Animator;

    private Vector3 m_StartingPos;
    private int m_CurrentNumHitsRemaining = 0;
    private bool m_IsCollapsed = false;
    private bool m_IsCrumbling = false;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Animator.enabled = false;
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
        if(m_IsCrumbling || Input.GetKey(KeyCode.Space))
        {
            VibrateGround();
        }
    }

    private void OnEnable()
    {
        if (m_ConnectedChunk != null)
        {
            m_ConnectedChunk.m_OnCollapse += OnConnectChunkCollapse;
        }
    }

    private void OnDisable()
    {
        if (m_ConnectedChunk != null)
        {
            m_ConnectedChunk.m_OnCollapse -= OnConnectChunkCollapse;
        }
    }

    //called from Hammer
    public void HitGround()
    {
        if (m_ConnectedChunk != null) { return; }

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
        m_IsCrumbling = true;
        Invoke("TriggerCollapse", timeToCollapse);
    }

    private void TriggerCollapse()
    {
        //stop crumbling to start collapse
        m_IsCrumbling = false;

        //play falling animation
        m_Animator.enabled = true;
        m_Animator.SetBool("IsCollapsed", true);

        m_ConnectedWall.SetActive(false);

        //if player is in area (maybe by doing a raycast from the player down to see if they hit the falling piece)
        //m_LevelMechanic.GetPlayerOne()

        m_OnCollapse?.Invoke();
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
        m_ConnectedChunk = null;
    }

    public bool IsCollapsed()
    {
        return m_IsCollapsed;
    }
}
