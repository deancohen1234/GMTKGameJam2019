using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    public Effect[] m_Effects;

    public void ActivateEffect(string effectName)
    {
        //find effect object
        Effect effect = null;
        for (int i = 0; i < m_Effects.Length; i++)
        {
            if (effectName == m_Effects[i].m_Name)
            {
                effect = m_Effects[i];
                break;
            }
        }

        if (effect == null)
        {
            Debug.Log("Activate Effect Failed because [" + effectName + "] wasn't found");
            return;
        }

        PlayEffect(effect, Vector3.zero);

    }

    public void ActivateEffect(string effectName, Vector3 position)
    {
        //find effect object
        Effect effect = null;
        for (int i = 0; i < m_Effects.Length; i++)
        {
            if (effectName == m_Effects[i].m_Name)
            {
                effect = m_Effects[i];
                break;
            }
        }

        if (effect == null)
        {
            Debug.LogError("Activate Effect Failed because [" + effectName + "] wasn't found");
            return;
        }

        PlayEffect(effect, position);

    }

    private void PlayEffect(Effect effect, Vector3 position)
    {
        //instatiate if necessary
        if (effect.m_NeedsInstantiation)
        {
            GameObject g = Instantiate(effect.m_SystemObject);

            if (g.GetComponent<DestroyAfterTime>())
            {
                g.GetComponent<DestroyAfterTime>().m_TimeToLive = effect.m_TimeToLive;
            }
            else
            {
                g.AddComponent<DestroyAfterTime>().m_TimeToLive = effect.m_TimeToLive;
            }

            effect.SetInstantiatedSystem(g);
        }

        //set effect position if necessary
        if (position != Vector3.zero)
        {
            effect.GetParticleSystem().gameObject.transform.position = position;
        }
        
        if (!effect.GetParticleSystem().isPlaying) { effect.GetParticleSystem().Play(); }

        if (effect.m_IsBurst)
        {
            effect.GetParticleSystem().Emit(effect.m_BurstAmount);
        }
        
    }
}

[System.Serializable]
public class Effect
{
    public string m_Name;
    public GameObject m_SystemObject;
    public bool m_IsBurst = false;
    public bool m_NeedsInstantiation;
    public float m_TimeToLive = 2;
    public int m_BurstAmount = 2;

    private GameObject m_SystemInstance;

    public ParticleSystem GetParticleSystem()
    {
        if (m_SystemInstance)
        {
            return m_SystemInstance.GetComponent<ParticleSystem>();
        }
        else
        {
            return m_SystemObject.GetComponent<ParticleSystem>();
        }
    }

    public void SetInstantiatedSystem(GameObject g)
    {
        m_SystemInstance = g;
    }
}
