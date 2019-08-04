using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsController : MonoBehaviour
{
    public ParticleSystem m_DisarmSystem;
    public ParticleSystem m_OnDisarmedSystem;

    public void ActivateDisarmSystem()
    {
        m_DisarmSystem.Simulate(3f);

        m_DisarmSystem.Play();
    }

    public void DeActiviateDisarmSystem()
    {
        m_DisarmSystem.Stop();
    }

    public void ActivateOnDisarmedSystem(Vector3 position)
    {
        m_OnDisarmedSystem.transform.position = position;

        m_OnDisarmedSystem.Emit(50);
    }
}
