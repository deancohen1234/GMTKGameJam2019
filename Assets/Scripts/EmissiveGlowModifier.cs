using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissiveGlowModifier : MonoBehaviour
{
    public Material m_EmissiveMat;
    public float m_IntensityModifier = 2.0f;
    public float m_NoisePanSpeed = 0.5f;

    public bool m_Activate;

    // Update is called once per frame
    void Update()
    {
        if (m_Activate)
        {
            float sample = Mathf.PerlinNoise(Time.time * m_NoisePanSpeed, 0);

            Color currentColor = m_EmissiveMat.GetColor("_EmissionColor");
            Vector4 normalizedColor = currentColor;
            normalizedColor.Normalize();
            float newIntensity = sample * m_IntensityModifier;

            m_EmissiveMat.SetColor("_EmissionColor", normalizedColor * newIntensity);
        }

    }
}
