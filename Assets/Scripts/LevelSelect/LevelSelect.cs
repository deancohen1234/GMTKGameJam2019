using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    [Header("General Settings")]
    public Transform m_CamTransform;
    public string m_LevelName;

    //TODO make these private but setable, and make getter function for these lads
    [Header("Level Details")]
    public Color m_PanelColor = Color.white;
    public string m_Title;
    public Sprite m_WeaponSprite;
    [TextArea]
    public string m_WeaponDescription;
    [TextArea]
    public string m_StageDescription;
    [TextArea]
    public string m_GodDescription;

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
