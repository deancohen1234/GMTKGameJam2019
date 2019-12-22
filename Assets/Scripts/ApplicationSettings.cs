using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationSettings : MonoBehaviour
{
    public InputManager m_InputManager;

    public bool m_ArcadeMode;

    public static ApplicationSettings m_Singleton;

    // Start is called before the first frame update
    void Awake()
    {
        if (m_Singleton == null)
        {
            m_Singleton = this;
        }

        QualitySettings.vSyncCount = 0; //disable vsync
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        m_InputManager.InitializeInput(m_ArcadeMode);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}

   
