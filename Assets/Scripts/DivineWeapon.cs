using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivineWeapon : MonoBehaviour
{
    public Transform m_ArenaCenter;
    public float m_ArenaWidth;
    public float m_ArenaHeight;
    public Vector3 m_Offset;

    public float m_FallSpeed = 0.3f;

    [Header("Action Details")]
    public PlayerAction m_AttackAction;

    protected PlayerController m_PlayerRef;
    private float m_WeaponStartHeight;

    private bool m_IsLerping;
    private float m_StartFallTime;
    private Vector3 m_DestinationPos;
    private Vector3 m_StartPos;
    private float m_Timer;

    private void Start()
    {
        m_WeaponStartHeight = transform.position.y;

        //assign all actions for delegate when game starts
        m_AttackAction.OnActionStart += OnWeaponAttackStart;
        m_AttackAction.ActionHandler += WeaponAttack; //currently isn't set up
        m_AttackAction.OnActionEnd += OnWeaponAttackEnd;
    }

    //When closing remove all delegates
    private void OnDestroy()
    {
        m_AttackAction.OnActionStart -= OnWeaponAttackStart;
        m_AttackAction.ActionHandler -= WeaponAttack;
        m_AttackAction.OnActionEnd -= OnWeaponAttackEnd;
    }

    protected void Update()
    {
        if (m_IsLerping)
        {
            //lerp downward
            Vector3 newLerpPos = Vector3.Lerp(m_StartPos, m_DestinationPos, m_Timer);
            transform.position = newLerpPos;

            m_Timer += Time.deltaTime * m_FallSpeed;

            if (m_Timer >= 1.0f)
            {
                m_Timer = 0;
                m_IsLerping = false;
                transform.position = m_DestinationPos;

                //helps get dagger off tip of stalagmite
                GetComponent<Rigidbody>().AddForce(transform.forward * 5);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Pickup weapon
            other.gameObject.GetComponent<PlayerController>().EquipWeapon(this);
            //OnWeaponPickup(other.gameObject.GetComponent<PlayerController>());
            gameObject.SetActive(false);
        }
    }

    //on weapon pickup set player reference for this weapon's attack action
    public virtual void OnWeaponPickup(PlayerController player)
    {
        m_PlayerRef = player;
        m_AttackAction.SetPlayerReference(player);
    }

    protected virtual void OnWeaponDropped()
    {
        m_PlayerRef = null;
        m_AttackAction.SetPlayerReference(null); //set reference for actions to null
    }

    public virtual void OnWeaponAttackStart()
    {

    }

    public virtual void WeaponAttack(PlayerController player, Vector3 direction)
    {

    }

    public virtual void OnWeaponAttackEnd()
    {

    }

    //player drops weapon
    //has parameter of the player who dropped the weapon
    public virtual void Drop(PlayerController lastControlledPlayer)
    {
        RandomizeLocationFromPlayer(lastControlledPlayer.transform.position);
        OnWeaponDropped();
    }

    public virtual void OnHit(PlayerController hitPlayer, PlayerController attackingPlayer)
    {
        //attacking player loses weapon, no damage
        attackingPlayer.ApplyBounceBackForce(hitPlayer.transform.position);
        hitPlayer.ApplyBounceBackForce(attackingPlayer.transform.position);

        if (hitPlayer.IsDisarming())
        {
            hitPlayer.DisarmOppponent(attackingPlayer);
        }
        else
        {
            //player failed to disarm they now take damage

            HealthComponent h = hitPlayer.GetHealthComponent();
            h.DealDamage(100);

            if (!h.IsDead())
            {
                hitPlayer.GetEffectsController().ActivateDamagedSystem();
            }

        }
    }

    public void RandomizeLocation()
    {
        gameObject.SetActive(true);

        float randomX = Random.Range(-1.0f, 1.0f) * m_ArenaWidth;
        float randomY = Random.Range(-1.0f, 1.0f) * m_ArenaHeight;
        Vector3 newPosition = m_ArenaCenter.position + m_Offset + new Vector3(randomX, 0, randomY);
        newPosition.y = transform.position.y;

        m_DestinationPos = newPosition;
        m_StartPos = m_DestinationPos + new Vector3(0, 7.5f, 0);

        transform.position = m_StartPos;
        m_IsLerping = true;
    }

    public void RandomizeLocationFromPlayer(Vector3 playerPos)
    {
        
        gameObject.SetActive(true);

        float randomX = Random.Range(-1.0f, 1.0f) * m_ArenaWidth;
        float randomY = Random.Range(-1.0f, 1.0f) * m_ArenaHeight;
        Vector3 newPosition = m_ArenaCenter.position + m_Offset + new Vector3(randomX, 0, randomY);
        newPosition.y = transform.position.y;

        m_DestinationPos = newPosition;
        m_StartPos = playerPos + new Vector3(0, 0.5f, 0);

        transform.position = m_StartPos;
        m_IsLerping = true;
    }

    private bool IsValidPosition(Vector3 position)
    {
        bool check = Physics.CheckSphere(position + new Vector3(0, 0.5f, 0), 0.1f);

        //if true then good position, if false then try a new position
        return !check;
    }
}
