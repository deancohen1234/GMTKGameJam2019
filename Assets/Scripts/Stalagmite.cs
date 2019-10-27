using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stalagmite : MonoBehaviour
{
    public GameObject m_RockObject;

    public void SetTimeToSelfDestruct(float time)
    {
        Invoke("SelfDestruct", time);
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
