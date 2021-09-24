using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BouncyArrow : DivineWeapon
{
    [Header("Move Properties")]
    public float m_StartingSpeed = 5f;
    public int m_StartingFreeBounces = 1;
    public float m_DragPerBounce = 2.5f;
    public float m_PickupSpeedThreshold = 1f;
    public float m_PickupDelayDuration = 0.5f; //duration in seconds after weapon is below speed threshold before it can be picked up
    public float m_RotationSpeed = 250f;

    [Header("Scaling Properties")]
    public float speedPerDisarm = 2.0f;
    public int bounceIncreaseInterval = 2; //every interval of disarms, increase bounces 

    [Header("Detection Properties")]
    public LayerMask m_HitboxMask;
    public LayerMask m_BouncingHitBoxMask;
    public float raycastDistanceModifier = 1f;
    public float m_SpherecastRadius = 0.5f;

    [Header("Damage Properties")]
    public float m_HitboxRadius = 0.5f;
    public float m_DamageCooldownDuration = 0.5f;

    [Header("Sound Properties")]
    public AudioClip[] wallBounceSounds;

    private Collider[] arrowHitColliders;
    private Collider[] arrowOverlapColliders;
    private RaycastHit[] arrowWallHits;
    private RaycastHit[] arrowPreWallHits;

    private Vector3 lastDirection;
    private float launchSpeed;
    private float pickSqrSpeedThreshold;
    private int freeBouncesRemaining; //when this is at 0, each bounce will add drag

    private int currentFreeBounceStart;
    private int disarmCount;

    private float pickupCooldownEndTime;

    private PlayerController lastHeldPlayer;
    private Rigidbody body;
    private AudioSource source;
    private WallCollisionBuffer wallCollisionBuffer;

    private bool isLaunched = false;

    private const float ARROWYHEIGHT = 0.28f;
    private const float VERTICALDOTTHRESHOLD = 0.90f; //make sure we don't get collisions from the ground

    #region Monobehavior
    private void OnEnable()
    {
        body = GetComponent<Rigidbody>();
        source = GetComponent<AudioSource>();

        Initialize();
    }

    protected override void Update()
    {
        if (!Physics.autoSimulation)
        {
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                RunPhysics();
                Physics.Simulate(Time.fixedDeltaTime);
            }
        }

        //rotate sprite
        
        if (!isLaunched)
        {
            Quaternion rotation = Quaternion.AngleAxis(0f, transform.forward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, Time.deltaTime * m_RotationSpeed);
        }
        else if (lastDirection != Vector3.zero)
        {
            float angle = Mathf.Atan2(lastDirection.x, -lastDirection.z);

            Quaternion rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, transform.forward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, Time.deltaTime * m_RotationSpeed);
        }

    }

    private void FixedUpdate()
    {
        if (Physics.autoSimulation)
        {
            RunPhysics();
        }
    }

    #endregion

    #region Weapon Overrides

    public override void OnPickup(PlayerController player)
    {
        base.OnPickup(player);

        ResetRigidbody();
    }

    public override void SetWeaponActive(bool isActive)
    {
        base.SetWeaponActive(isActive);
        
        if (isActive)
        {
            body.useGravity = true;
            body.drag = 0;
        }
    }

    public override void OnAttackStart()
    {
        base.OnAttackStart();

        Shoot();
    }

    public override void OnAttackEnd()
    {
        base.OnAttackEnd();
    }

    public override void Drop(PlayerController lastControlledPlayer)
    {
        //player is throwing/shooting weapon
        
        if (lastControlledPlayer.Equals(m_OwningPlayer))
        {
            //make sure on player drop don't randomize weapon position
            SetWeaponActive(true);

            m_OwningPlayer = null;
            m_AttackAction.SetPlayerReference(null); //set reference for actions to null
        } 
        
    }

    public override bool OnHit(PlayerController hitPlayer, PlayerController attackingPlayer)
    {
        bool playerHit = base.OnHit(hitPlayer, attackingPlayer);

        //add cooldown to stop multiple hits
        //OnSpeedBelowThreshold();

        //attacking player loses weapon, no damage
        hitPlayer.ApplyBounceBackForce(body.position);

        if (playerHit)
        {
            //player failed to disarm they now take damage
            hitPlayer.PlaySoundEffect(PlayerSound.Damaged);

            HealthComponent h = hitPlayer.GetHealthComponent();
            h.DealDamage(100);

            if (!h.IsDead())
            {
                hitPlayer.GetEffectsController().ActivateDamagedSystem();
            }

            ResetWeapon();

            body.drag = 50f;
        }

        return playerHit;
    }

    //I will come back to this later, kinda feel like poop right now (just stressed)
    //you got this... for real
    public override void OnDisarmed(PlayerController hitPlayer, PlayerController attackingPlayer)
    {
        base.OnDisarmed(hitPlayer, attackingPlayer);

        disarmCount++;
        //add speed
        launchSpeed += speedPerDisarm;

        if (disarmCount % bounceIncreaseInterval == 0)
        {
            currentFreeBounceStart++;
            freeBouncesRemaining = currentFreeBounceStart;
        }
    }

    #endregion

    private void Initialize()
    {
        if (arrowHitColliders == null || arrowHitColliders.Length > 0)
        {
            arrowHitColliders = new Collider[30];
        }

        if (arrowWallHits == null || arrowWallHits.Length > 0)
        {
            arrowWallHits = new RaycastHit[30];
        }

        if (arrowOverlapColliders == null || arrowOverlapColliders.Length > 0)
        {
            arrowOverlapColliders = new Collider[30];
        }

        if (arrowPreWallHits == null || arrowPreWallHits.Length > 0)
        {
            arrowPreWallHits = new RaycastHit[30];
        }

        pickSqrSpeedThreshold = m_PickupSpeedThreshold * m_PickupSpeedThreshold;

        ResetWeapon();
    }

    private void RunPhysics()
    {
        if (isLaunched)
        {
            if (wallCollisionBuffer.isBuffered)
            {
                EmptyAndUseWallCollisionBuffer(ref wallCollisionBuffer);
            }

            CheckArrowCollision();

            //make sure weapon isn't picked up so this doesn't occur
            if (IsBelowSpeedThreshold() && !IsPickedUp())
            {
                OnSpeedBelowThreshold();
            }
        }
        else
        {
            //only if cooldown for pickup is ended then you can activate pickup colliders
            if (Time.time >= pickupCooldownEndTime)
            {
                if (!ArePickupCollidersActive())
                {
                    SetCollidersActive(true);
                }
            }
        }

        //cache last direction
        if (body.velocity.sqrMagnitude >= 0.01f)
        {
            lastDirection = body.velocity.normalized;
        }
    }

    public void Shoot()
    {
        //Physics.autoSimulation = false;
        body.constraints = RigidbodyConstraints.FreezeRotation;

        lastHeldPlayer = m_OwningPlayer;

        //cache this now since DropWeapon will kill the OwningPlayer reference
        //Vector3 launchPosition = new Vector3(m_OwningPlayer.transform.position.x, ARROWYHEIGHT, m_OwningPlayer.transform.position.x);
        Vector3 launchPosition = new Vector3(m_OwningPlayer.m_Rigidbody.position.x, ARROWYHEIGHT, m_OwningPlayer.m_Rigidbody.position.z);

        m_OwningPlayer.DropWeapon(false); //need to drop/throw weapon to make sprite appear and allow pickup

        isLaunched = true;

        //disable pickup collider
        SetCollidersActive(false);

        body.MovePosition(launchPosition);

        body.useGravity = false;
        body.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY; //don't freeze X and Z position
        body.isKinematic = false;

        body.velocity = Vector3.ProjectOnPlane(lastHeldPlayer.GetMoveDirection() * launchSpeed, Vector3.up);
    }

    #region Speed Control

    public void ResetWeapon()
    {
        launchSpeed = m_StartingSpeed;
        currentFreeBounceStart = m_StartingFreeBounces;

        freeBouncesRemaining = currentFreeBounceStart;

        disarmCount = 0;

        ResetRigidbody();
    }

    public void ResetRigidbody()
    {
        //body.position = new Vector3(body.position.x, ARROWYHEIGHT, body.position.z);
        body.drag = 0; //only set drag on bounces
        wallCollisionBuffer.isBuffered = false;
    }

    private bool IsBelowSpeedThreshold()
    {
        return body.velocity.sqrMagnitude <= pickSqrSpeedThreshold && !wallCollisionBuffer.isBuffered;
    }

    private void OnSpeedBelowThreshold()
    {
        isLaunched = false;
        pickupCooldownEndTime = Time.time + m_PickupDelayDuration;
    }
    #endregion

    #region Arrow Collision
    private void CheckArrowCollision()
    {
        //check if we hit player
        //but only check of we are below speed threshold
        if (!IsBelowSpeedThreshold())
        {
            PlayerSphereCheck();
        }

        Vector3 direction = Vector3.ProjectOnPlane(m_Rigidbody.velocity.normalized, Vector3.up);

        //there is something right in front of us so bail, this guy's got it
        if (PreRaycastCheck(direction)) { return; }

        Debug.DrawLine(body.position, body.position + direction * m_Rigidbody.velocity.magnitude * raycastDistanceModifier * Time.fixedDeltaTime, Color.white);
        //Debug.Break();
        SphereCastCheck(direction);
    }

    #region Casting
    private void PlayerSphereCheck()
    {
        int playerHits = Physics.OverlapSphereNonAlloc(body.position, m_HitboxRadius, arrowHitColliders, m_HitboxMask);
        if (playerHits > 0)
        {
            for (int i = 0; i < playerHits; i++)
            {
                if (arrowHitColliders[i] != null)
                {
                    PlayerController player = arrowHitColliders[i].GetComponent<PlayerController>();
                    if (player != null && !player.Equals(lastHeldPlayer)) //make sure original shooter can't be hit by own arrow
                    {
                        HitPlayer(player);
                        break;
                    }
                }
            }
        }
    }

    //make sure something isn't immediately infront of arrow
    private bool PreRaycastCheck(Vector3 direction)
    {
        if (!OverlapSphereArrowCheck(body.position, null))
        {
            return false;
        }
        //go as far as it needs to find something and if not then we can log an error
        int wallHits = Physics.RaycastNonAlloc(body.position, direction, arrowPreWallHits, m_SpherecastRadius * 2f * raycastDistanceModifier, m_BouncingHitBoxMask);
        //int wallHits = Physics.SphereCastNonAlloc(body.position - direction * m_SpherecastRadius, m_SpherecastRadius, direction, arrowPreWallHits, m_SpherecastRadius * raycastDistanceModifier * 2f, m_BouncingHitBoxMask);

        if (wallHits > 0)
        {
            float shortestDist = float.MaxValue;
            int shortestDistIndex = -1;
            for (int i = 0; i < wallHits; i++)
            {
                if (arrowPreWallHits[i].collider != null && arrowPreWallHits[i].distance > 0)
                {
                    if (arrowPreWallHits[i].distance < shortestDist)
                    {
                        shortestDistIndex = i;
                        shortestDist = arrowPreWallHits[i].distance;
                    }                    
                }
            }

            if (shortestDistIndex >= 0)
            {
                BufferSurfaceCollision(arrowPreWallHits[shortestDistIndex].point, arrowPreWallHits[shortestDistIndex].normal, arrowPreWallHits[shortestDistIndex].collider, arrowPreWallHits[shortestDistIndex].collider.name);
                return true;
            }
        }

        return false;

    }

    private void SphereCastCheck(Vector3 direction)
    {
        float castDist = (m_Rigidbody.velocity.magnitude * raycastDistanceModifier * Time.fixedDeltaTime) + m_SpherecastRadius + Mathf.Epsilon;
        //check for wall collisions
        //int wallHits = Physics.RaycastNonAlloc(transform.position, direction, arrowWallHits, m_Rigidbody.velocity.magnitude * raycastDistanceModifier * Time.fixedDeltaTime, m_BouncingHitBoxMask);
        int wallHits = Physics.SphereCastNonAlloc(body.position, m_SpherecastRadius, direction, arrowWallHits, castDist, m_BouncingHitBoxMask);
        if (wallHits > 0)
        {
            int shortestDistIndex = -1;
            float shortestDist = float.MaxValue;
            for (int i = 0; i < arrowWallHits.Length; i++)
            {
                if (arrowWallHits[i].collider != null && arrowWallHits[i].distance > 0)
                {
                    if (arrowWallHits[i].distance < shortestDist)
                    {
                        shortestDistIndex = i;
                        shortestDist = arrowWallHits[i].distance;
                    }
                }
            }

            if (shortestDistIndex > -1)
            {
                Vector3 normal = Vector3.ProjectOnPlane(arrowWallHits[shortestDistIndex].normal, Vector3.up);
                Vector3 point = arrowWallHits[shortestDistIndex].point;
                point.y = body.position.y;

                point = VerifyHitPosition(point);

                Color randomColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                arrowWallHits[shortestDistIndex].collider.gameObject.GetComponent<MeshRenderer>().material.color = randomColor;
                BufferSurfaceCollision(point, normal, arrowWallHits[shortestDistIndex].collider, arrowWallHits[shortestDistIndex].collider.name);
            }
        }
    }

    private bool OverlapSphereArrowCheck(Vector3 position, Collider ignoreCollider)
    {
        if (Physics.OverlapSphereNonAlloc(position, m_SpherecastRadius * raycastDistanceModifier + Mathf.Epsilon, arrowOverlapColliders, m_BouncingHitBoxMask) > 0)
        {
            for (int i = 0; i < arrowOverlapColliders.Length; i++)
            {
                if (arrowOverlapColliders[i] != null)
                {
                    if (!arrowOverlapColliders[i].Equals(ignoreCollider)) 
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private Vector3 VerifyHitPosition(Vector3 foundPos)
    {
        RaycastHit hit;

        if (Physics.Raycast(body.position - (foundPos - body.position).normalized, (foundPos - body.position).normalized, out hit, 100f, m_BouncingHitBoxMask))
        {
            return hit.point;
        }
        else
        {
            Debug.Log("Yikes this ain't good");
            return foundPos;
        }
    }

    #endregion

    #region Buffering
    //store for 1 physics timestep, the position and normal that will happen next time step
    private void BufferSurfaceCollision(Vector3 position, Vector3 normal, Collider collider, string name)
    {
        wallCollisionBuffer.velocity = m_Rigidbody.velocity;
        wallCollisionBuffer.normal = normal;
        wallCollisionBuffer.collider = collider;
        wallCollisionBuffer.isBuffered = true;

        //body.MovePosition(position);
        body.velocity = Vector3.zero;
        body.position = position;

        //play random wall bounce sound
        int randomSoundIndex = Random.Range(0, wallBounceSounds.Length);
        source.clip = wallBounceSounds[randomSoundIndex];
        source.pitch = Random.Range(0.9f, 1.1f);
        source.Play();
    }

    private void EmptyAndUseWallCollisionBuffer(ref WallCollisionBuffer buffer)
    {
        float dot = Vector3.Dot(buffer.normal, Vector3.up);

        if (dot <= VERTICALDOTTHRESHOLD)
        {
            freeBouncesRemaining--;

            if (freeBouncesRemaining < 0)
            {
                body.drag += m_DragPerBounce;
            }

            if (buffer.normal != Vector3.zero)
            {
                body.velocity = Vector3.Reflect(buffer.velocity, buffer.normal);
            }

            //empty buffer
            buffer.isBuffered = false;
            buffer.collider = null;
        }

    }

    #endregion

    private void HitPlayer(PlayerController player)
    {
        player.AttackHit(this, lastHeldPlayer);
    }
    #endregion
}

public struct WallCollisionBuffer
{
    public bool isBuffered;
    public Vector3 velocity;
    public Vector3 normal;
    public Collider collider;
}
