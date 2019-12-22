using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterSelectController : MonoBehaviour
{
    public CharacterOption[] m_P1_CharacterOptions;
    public CharacterOption[] m_P2_CharacterOptions;

    public float m_SwapDelay = 0.5f; //in seconds

    public Image m_P1_SceneImage;
    public Image m_P2_SceneImage;

    private int m_P1_CharacterIndex = 0;
    private int m_P2_CharacterIndex = 0;

    private float m_P1SwapTime;
    private float m_P2SwapTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckP1Input();
        //CheckP2Input();
    }

    void CheckP1Input()
    {
        if (Time.time - m_P1SwapTime <= m_SwapDelay) { return; }

        var gamepads = Gamepad.all;

        if (gamepads.Count > 0 && gamepads[0] != null)
        {
            Vector2 stickVal = gamepads[0].leftStick.ReadValue();

            if (stickVal.y >= .9f)
            {
                ChangeCharacter(true, false);
            }
            else if (stickVal.y <= -.9f)
            {
                ChangeCharacter(true, true);
            }
        }
    }

    void CheckP2Input()
    {

    }

    private void ChangeCharacter(bool p1Change, bool moveDown)
    {
        int moveInt = moveDown ? 1 : -1;
        if (p1Change)
        {
            int newIndex = (int)Mathf.Repeat(m_P1_CharacterIndex + moveInt, m_P1_CharacterOptions.Length);

            m_P1_SceneImage.sprite = m_P1_CharacterOptions[newIndex].m_CharacterImage;

            m_P1_CharacterIndex = newIndex;
            m_P1SwapTime = Time.time;
        }
        else
        {
            int newIndex = (int)Mathf.Repeat(m_P2_CharacterIndex + moveInt, m_P2_CharacterOptions.Length);

            m_P2_SceneImage.sprite = m_P2_CharacterOptions[newIndex].m_CharacterImage;

            m_P2_CharacterIndex = newIndex;
            m_P2SwapTime = Time.time;
        }
    }
}

//TODO eventually fill this with more character specfic info like descriptions or animations or something
[System.Serializable]
public class CharacterOption
{
    public string m_Name;
    public Sprite m_CharacterImage;
}
