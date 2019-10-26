using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : MonoBehaviour
{
    public void SetSelfDestruct(float time)
    {
        Invoke("SelfDestruct", time);
    }

    private void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
