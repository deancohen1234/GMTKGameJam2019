using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RippleCaller : MonoBehaviour
{
    public RippleEffect m_RippleEffect;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (m_RippleEffect.dropInterval > 0)
        {
            m_RippleEffect.timer += Time.deltaTime;
            while (m_RippleEffect.timer > m_RippleEffect.dropInterval)
            {
                m_RippleEffect.ActivateRipple(transform.position);
                m_RippleEffect.timer -= m_RippleEffect.dropInterval;
            }
        }
    }
}
