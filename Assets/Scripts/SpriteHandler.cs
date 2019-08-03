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

    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSprite(Vector2 vector)
    {
        float atan2 = Mathf.Atan2(vector.y, vector.x);

        atan2 = atan2 * Mathf.Rad2Deg;
        Debug.Log("Angle: " + atan2);

        if (IsInRange(atan2, 67.5f, 112.5f))
        {
            //12 o clock
            m_PlayerSprite = m_12oClock;
        }
        else if (IsInRange(atan2, 22.5f, 67.5f))
        {
            //1:30
            m_PlayerSprite = m_130oClock;
        }
        else if (IsInRange(atan2, -22.5f, 22.5f))
        {
            //3:00
            m_PlayerSprite = m_3oClock;
        }
        else if (IsInRange(atan2, -67.5f, -22.5f))
        {
            //4:30
            m_PlayerSprite = m_430oClock;
        }
        else if (IsInRange(atan2, -112.5f, -67.5f))
        {
            //6:00
            Debug.Log("6");
            m_PlayerSprite = m_6oClock;
        }
        else if (IsInRange(atan2, -157.5f, -112.5f))
        {
            //10:30
            m_PlayerSprite = m_1030oClock;

        }
        else
        {
            //9
            m_PlayerSprite = m_9oClock;
        }

        m_SpriteRenderer.sprite = m_PlayerSprite;
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

}
