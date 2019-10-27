using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsController : MonoBehaviour
{
    public ParticleSystem m_DisarmSystem;
    public ParticleSystem m_OnDisarmedSystem;
    public ParticleSystem m_OnDamagedSystem;
    public ParticleSystem m_OnDeathSystem;
    public ParticleSystem m_PoofSystem;

    public SpriteRenderer m_CharacterSprite;
    public Color m_TintColor;

    public void ActivateDisarmSystem()
    {
        m_DisarmSystem.Simulate(3f);

        m_DisarmSystem.Play();
    }

    public void DeActiviateDisarmSystem()
    {
        m_DisarmSystem.Stop();
        m_DisarmSystem.Clear();
    }

    public void ActivateOnDisarmedSystem(Vector3 position)
    {
        m_OnDisarmedSystem.transform.position = position;

        m_OnDisarmedSystem.Emit(50);
    }

    public void ActivateOnDeathSystem()
    {
        int bloodAmount = Random.Range(150, 250);
        m_OnDeathSystem.Emit(bloodAmount);
    }

    public void ActivateDamagedSystem()
    {
        int bloodAmount = Random.Range(25, 50);
        m_OnDamagedSystem.Emit(bloodAmount);
    }

    public void ActivatePoofSystem()
    {
        m_PoofSystem.Emit(175);
    }

    public void StartVisualAttack()
    {
        m_CharacterSprite.color = m_TintColor;
    }

    public void EndVisualAttack()
    {
        m_CharacterSprite.color = Color.white;
    }
}
