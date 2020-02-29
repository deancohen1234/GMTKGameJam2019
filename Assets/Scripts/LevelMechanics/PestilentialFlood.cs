using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PestilentialFlood : MonoBehaviour
{
    public LevelMechanicsContainer m_Container;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //temporary
        if (Input.GetKeyDown(KeyCode.K))
        {
            ActivateFlood();
        }
    }

    void ActivateFlood()
    {
        m_Container.GetPlayerOne().GetHealthComponent().SetDamageMultiplier(2f);
        m_Container.GetPlayerTwo().GetHealthComponent().SetDamageMultiplier(2f);
    }
}
