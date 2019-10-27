using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stalagmite : MonoBehaviour
{
    public GameObject m_RockObject;
    public ParticleSystem m_Pebbles;

    public AudioClip m_EarthPuncture;

    [HideInInspector]
    public int m_Index; //set from Evil Man

    public void SetTimeToSelfDestruct(float time)
    {
        Invoke("SelfDestruct", time);
    }

    public void Create(int index)
    {
        m_Index = index;
        m_Pebbles.Emit(50);

        AudioSource source = GetComponent<AudioSource>();
        source.clip = m_EarthPuncture;
        source.pitch = Random.Range(0.8f, 1.2f);

        source.Play();
    }

    public void Hide()
    {
        m_RockObject.GetComponent<Animator>().SetTrigger("HideStalagmite");
        SetTimeToSelfDestruct(2.0f);
    }

    private void SelfDestruct()
    {
        Destroy(gameObject);
    }


}
