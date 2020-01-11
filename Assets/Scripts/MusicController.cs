using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public static MusicController m_Singleton = null;

    public AudioSource m_Music;

    private void Awake()
    {
        if (m_Singleton == null)
        {
            m_Singleton = this;
        }
    }

    public void Play()
    {
        m_Music.Play();
    }

    public void Stop()
    {
        m_Music.Stop();
    }

    public void ChangePitch(float pitch)
    {
        m_Music.pitch = pitch;
    }
}
