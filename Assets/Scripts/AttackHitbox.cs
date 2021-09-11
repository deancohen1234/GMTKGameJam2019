﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public float m_AttackDelayTime = 0.25f;
    private PlayerController m_ParentPlayerController;

    private float m_AttackStartTime;

    private void Start()
    {
        m_ParentPlayerController = transform.parent.parent.GetComponent<PlayerController>();
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != this.gameObject)
        {
            if (other.gameObject.tag == "Player")
            {
                PlayerController pc = other.gameObject.GetComponent<PlayerController>();

                if (pc.IsInvincible() == false)
                {
                    if (Time.time - m_AttackStartTime >= m_AttackDelayTime)
                    {
                        //deal damage
                        pc.AttemptMeleeAttack(m_ParentPlayerController);

                        m_AttackStartTime = Time.time;
                    }
                }
                
            }

        }
    }
}
