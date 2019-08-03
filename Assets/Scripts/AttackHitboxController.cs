using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitboxController : MonoBehaviour
{
    public Collider m_UpCollider;
    public Collider m_UpRightCollider;
    public Collider m_RightCollider;
    public Collider m_DownRightCollider;
    public Collider m_DownCollider;
    public Collider m_DownLeftCollider;
    public Collider m_LeftCollider;
    public Collider m_UpLeftCollider;

    private void Start()
    {
        DisableAllHitBoxes();
    }

    public void ActivateHitBox(PlayerOrientation orientation)
    {
        DisableAllHitBoxes();

        switch (orientation)
        {
            case PlayerOrientation.Up:
                m_UpCollider.enabled = true;
                m_UpCollider.gameObject.SetActive(true);
                break;
            case PlayerOrientation.UpRight:
                m_UpRightCollider.enabled = true;
                m_UpRightCollider.gameObject.SetActive(true);
                break;
            case PlayerOrientation.Right:
                m_RightCollider.enabled = true;
                m_RightCollider.gameObject.SetActive(true);
                break;
            case PlayerOrientation.DownRight:
                m_DownRightCollider.enabled = true;
                m_DownRightCollider.gameObject.SetActive(true);
                break;
            case PlayerOrientation.Down:
                m_DownCollider.enabled = true;
                m_DownCollider.gameObject.SetActive(true);
                break;
            case PlayerOrientation.DownLeft:
                m_DownLeftCollider.enabled = true;
                m_DownLeftCollider.gameObject.SetActive(true);
                break;
            case PlayerOrientation.Left:
                m_LeftCollider.enabled = true;
                m_LeftCollider.gameObject.SetActive(true);
                break;
            case PlayerOrientation.UpLeft:
                m_UpLeftCollider.enabled = true;
                m_UpLeftCollider.gameObject.SetActive(true);
                break;
        }
    }

    public void DisableAllHitBoxes()
    {
        m_UpCollider.enabled = false;
        m_UpRightCollider.enabled = false;
        m_RightCollider.enabled = false;
        m_DownRightCollider.enabled = false;
        m_DownCollider.enabled = false;
        m_DownLeftCollider.enabled = false;
        m_LeftCollider.enabled = false;
        m_UpLeftCollider.enabled = false;

        m_UpCollider.gameObject.SetActive(false);
        m_UpRightCollider.gameObject.SetActive(false);
        m_RightCollider.gameObject.SetActive(false);
        m_DownRightCollider.gameObject.SetActive(false);
        m_DownCollider.gameObject.SetActive(false);
        m_DownLeftCollider.gameObject.SetActive(false);
        m_LeftCollider.gameObject.SetActive(false);
        m_UpLeftCollider.gameObject.SetActive(false);
    }
}
