using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    public Transform m_CamTransform;
    public string m_LevelName;

    public Action<LevelSelect> m_OnSelected; //on select event that sends this object back through
    
    public Transform GetCamTransform()
    {
        return m_CamTransform;
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(m_LevelName);
    }

    public void Select()
    {
        m_OnSelected?.DynamicInvoke(this);
    }
}
