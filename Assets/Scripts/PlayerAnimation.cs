using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator m_Animator;

    public string Up;
    public string UpRight;
    public string Right;
    public string DownRight;
    public string Down;
    public string DownLeft;
    public string Left;
    public string UpLeft;

    private PlayerOrientation m_PreviousPlayerOrientation;

    public void SetOrientation(PlayerOrientation orientation)
    {
        ResetAllAnimationBools();

        if (m_PreviousPlayerOrientation != orientation)
        {
            m_Animator.SetTrigger("ChangeState");
        }

        switch (orientation)
        {
            case PlayerOrientation.Up:
                m_Animator.SetBool(Up, true);
                break;
            case PlayerOrientation.UpRight:
                m_Animator.SetBool(UpRight, true);
                break;
            case PlayerOrientation.Right:
                m_Animator.SetBool(Right, true);
                break;
            case PlayerOrientation.DownRight:
                m_Animator.SetBool(DownRight, true);
                break;
            case PlayerOrientation.Down:
                m_Animator.SetBool(Down, true);
                break;
            case PlayerOrientation.DownLeft:
                m_Animator.SetBool(DownLeft, true);
                break;
            case PlayerOrientation.Left:
                m_Animator.SetBool(Left, true);
                break;
            case PlayerOrientation.UpLeft:
                m_Animator.SetBool(UpLeft, true);
                break;
        }

        m_PreviousPlayerOrientation = orientation;
    }

    public void FreezeAnimation()
    {
        m_Animator.StopPlayback();
    }

    private void ResetAllAnimationBools()
    {
        m_Animator.SetBool(Up, false);
        m_Animator.SetBool(UpRight, false);
        m_Animator.SetBool(Right, false);
        m_Animator.SetBool(DownRight, false);
        m_Animator.SetBool(Down, false);
        m_Animator.SetBool(DownLeft, false);
        m_Animator.SetBool(Left, false);
        m_Animator.SetBool(UpLeft, false);
    }
}
