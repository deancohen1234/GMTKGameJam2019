using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventRouter : MonoBehaviour
{
    public Action m_OnAnimationComplete;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CompleteAnimation()
    {
        if (m_OnAnimationComplete != null)
        {
            m_OnAnimationComplete.Invoke();
        }
    }
}
