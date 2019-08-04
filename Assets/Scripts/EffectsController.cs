using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsController : MonoBehaviour
{
    public ParticleSystem m_DisarmSystem;

    public void ActivateDisarmSystem()
    {
        m_DisarmSystem.Simulate(3f);

        m_DisarmSystem.Play();
    }

    public void DeActiviateDisarmSystem()
    {
        m_DisarmSystem.Stop();
    }
}
