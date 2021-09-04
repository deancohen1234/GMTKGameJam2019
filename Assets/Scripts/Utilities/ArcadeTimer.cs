using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ArcadeTimer : MonoBehaviour
{
    public Text m_TimerText;
    public int m_CountdownTime = 60;

    public UnityEvent m_OnTimerComplete;

    private float m_TimerStartTime = 0;
    private bool m_TimerActive = false;

    public void StartTimer()
    {
        m_TimerStartTime = Time.time;
        m_TimerActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_TimerActive)
        {
            int timerValue = m_CountdownTime - Mathf.RoundToInt(Time.time - m_TimerStartTime);

            if (timerValue >= 0)
            {
                m_TimerText.text = timerValue.ToString();
            }
            else
            {
                //timer is complete
                m_TimerActive = false;
                m_OnTimerComplete.Invoke();
            }
        }
    }
}
