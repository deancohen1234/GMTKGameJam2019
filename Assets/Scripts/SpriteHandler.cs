using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteHandler : MonoBehaviour
{
    public Sprite m_12oClock;
    public Sprite m_130oClock;
    public Sprite m_3oClock;
    public Sprite m_430oClock;
    public Sprite m_6oClock;
    public Sprite m_730oClock;
    public Sprite m_9oClock;
    public Sprite m_1030oClock;

    private Sprite m_PlayerSprite;
    private SpriteRenderer m_SpriteRenderer;

    private Transform m_Camera;

    private void Awake()
    {
        m_Camera = Camera.main.transform;
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSprite(PlayerOrientation orientation)
    {
        switch (orientation)
        {
            case PlayerOrientation.Up:
                m_PlayerSprite = m_12oClock;
                break;
            case PlayerOrientation.UpRight:
                m_PlayerSprite = m_130oClock;
                break;
            case PlayerOrientation.Right:
                m_PlayerSprite = m_3oClock;
                break;
            case PlayerOrientation.DownRight:
                m_PlayerSprite = m_430oClock;
                break;
            case PlayerOrientation.Down:
                m_PlayerSprite = m_6oClock;
                break;
            case PlayerOrientation.DownLeft:
                m_PlayerSprite = m_730oClock;
                break;
            case PlayerOrientation.Left:
                m_PlayerSprite = m_9oClock;
                break;
            case PlayerOrientation.UpLeft:
                m_PlayerSprite = m_1030oClock;
                break;
        }

        m_SpriteRenderer.sprite = m_PlayerSprite;
    }

    private void LateUpdate()
    {
        //face sprites to camera
        //transform.LookAt(m_Camera);
    }



}
