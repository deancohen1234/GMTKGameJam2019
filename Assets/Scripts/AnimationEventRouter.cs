using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventRouter : MonoBehaviour
{
    public Action m_OnAnimationComplete;

    public void CompleteAnimation()
    {
        if (m_OnAnimationComplete != null)
        {
            m_OnAnimationComplete.Invoke();
        }
    }
}
