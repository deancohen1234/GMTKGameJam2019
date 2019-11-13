﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectController : MonoBehaviour
{
    [Header("Scene References")]
    public LevelSelect[] m_Levels;
    public Camera m_MainCamera;

    [Header("UI References")]
    public GameObject m_DetailsPanel;
    public Text m_Title;
    public Image m_WeaponImage;
    public Text m_WeaponDescription;
    public Text m_StageDescription;

    [Header("Camera Move Settings")]
    public float m_TransitionTotalTime = 1.5f; //in seconds

    private LevelSelect m_CurrentSelectedLevel;

    private Vector3 m_CamStartPosition;
    private Quaternion m_CamStartRotation;
    private float m_TransitionStartTime;
    private bool m_IsTransitioning;

    private void Start()
    {
        //default panel to being invisible
        if (m_DetailsPanel.activeInHierarchy) { m_DetailsPanel.SetActive(false); }
    }

    private void OnEnable()
    {
        for (int i = 0; i < m_Levels.Length; i++)
        {
            m_Levels[i].m_OnSelected += OnLevelSelected;
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < m_Levels.Length; i++)
        {
            m_Levels[i].m_OnSelected -= OnLevelSelected;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsTransitioning)
        {
            //lerp position
            float lerpAmount = Mathf.Clamp((Time.time - m_TransitionStartTime), 0, m_TransitionStartTime + m_TransitionTotalTime); //getting time from start
            float normalizedLerpAmount = DeanUtils.Map(lerpAmount, 0, m_TransitionTotalTime, 0, 1);

            //Vector3 newPos = Vector3.Lerp(m_CamStartPosition, m_CurrentSelectedLevel.GetCamTransform().position, normalizedLerpAmount);


            float totalFrames = (1.0f / Time.deltaTime) * m_TransitionTotalTime;
            float fractionOfTotalFrames = normalizedLerpAmount * totalFrames;
            float amountToMovePerFrame = fractionOfTotalFrames / totalFrames;

            Vector3 newPos = Vector3.Slerp(m_CamStartPosition, m_CurrentSelectedLevel.GetCamTransform().position, amountToMovePerFrame);
            Quaternion newRot = Quaternion.Lerp(m_CamStartRotation, m_CurrentSelectedLevel.GetCamTransform().rotation, amountToMovePerFrame);

            m_MainCamera.transform.position = newPos;
            m_MainCamera.transform.rotation = newRot;

            if (normalizedLerpAmount >= 1)
            {
                m_IsTransitioning = false;
                m_CurrentSelectedLevel = null;
            }
        }
    }

    private void OnLevelSelected(LevelSelect level)
    {
        if (!m_DetailsPanel.activeInHierarchy) { m_DetailsPanel.SetActive(true); }

        //Lerp Camera to map location
        StartTransition(level);

        //Populate fields for the level description
        SetDescriptionFields(level);
    }

    private void StartTransition(LevelSelect level)
    {
        m_CurrentSelectedLevel = level;

        m_IsTransitioning = true;
        m_TransitionStartTime = Time.time;

        m_CamStartPosition = m_MainCamera.transform.position;
        m_CamStartRotation = m_MainCamera.transform.rotation;
    }

    private void SetDescriptionFields(LevelSelect level)
    {
        m_Title.text = level.m_Title;
        m_WeaponImage.sprite = level.m_WeaponSprite;
        m_WeaponDescription.text = level.m_WeaponDescription;
        m_StageDescription.text = level.m_StageDescription;
    }
}