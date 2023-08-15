using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using Managers;
using Gun;
using UnityEngine.InputSystem;
using static PlayerController;

public class Combatant : MonoBehaviour
{
    public enum CombatState
    {
        Normal,
        Invincible,
        Dodge,
        NoControl,
        NoAttack,
        Frozen,
        Burn,
        Count
    }

    /// <summary>
    /// Variables
    /// </summary>

    [Header("Movement")]
    [Rename("Max Speed")] public float f_maxSpeed = 5.5f;
    [Rename("Max Acceleration")] public float f_maxAcceleration = 40;
    [Rename("Acceleration Curve")] public AnimationCurve C_accelerationCurve;
    [Space(4)]

    [Header("Rotation")]
    [Rename("Rotation Time"), Range(0, 0.5f)] public float f_rotationTime = 0.08f;
    [Space(4)]

    [Header("Dodge")]
    [Rename("Dodge Startup")] public float f_dodgeStartDelay = 0.12f;
    [Rename("Dodge Length")] public float f_dodgeLength = 2.5f;
    [Rename("Dodge Time")] public float f_dodgeTime = 0.3f;
    [Rename("Dodge Animation Curve")] public AnimationCurve C_dodgeCurve;
    [Rename("Dodge Count")]public int i_maxDodges = 3;
    [Rename("Dodge Recovery Time")]public float f_dodgeRecoveryTime;

    [Space(4)]

    [Header("Fake Physics")]
    [Rename("Size")] public float f_size = 0.35f;
    float f_stepHeight = 0.2f;
    float f_dampStrength = 0.4f;
    float f_coyoteTime = 0.8f;
    [Rename("Collision Bounce Percentage"), Range(0, 1)] public float f_collisionBounciness = 0.285f;
    [Space(4)]

    [Header("Game Variables")]
    [Rename("Max Health")] public float f_maxHealth = 100;
    [Rename("Invincibility On Hit Time")] public float f_invincibleTime = 0.15f;
    [Rename("Owned Gun")] public Gun.Gun C_ownedGun = null;
    [Space(4)]

    [Header("Materials")]
    [Rename("Default Material")] public Material C_defaultMaterial;
    [Rename("Dodge Material")] public Material C_dodgeMaterial;




    /// <summary>
    /// Run time variables
    /// </summary>
    #region RuntimeVariables
    protected float f_currentHealth;
    public CombatState e_combatState = CombatState.Normal;

    protected Vector3 S_acceleration;
    protected Vector3 S_velocity = Vector3.zero;
    protected float f_rotationalAcceleration;
    protected float f_rotationalVelocity;
    public bool b_isDead = false;
    protected int i_bulletLayerMask;
    protected bool b_fireCancelWhileDodging;
    [Range(0, 1)] protected float f_slowMultiplier = 1;
    protected float f_currentAccelerationStep = 0;
    protected int i_currentDodgeCount;




    #region RuntimeMovement
    protected Vector2 S_movementVec2Direction;
    protected Vector2 S_movementVec2DirectionLastFrame;
    protected Vector3 S_movementInputDirection
    {
        get { return new Vector3(S_movementVec2Direction.x, 0, S_movementVec2Direction.y); }
    }
    #endregion

    #region RuntimeRotation
    protected Vector2 S_previousRotationVec2Direction;
    protected float f_previousYRotationAngle
    {
        get { return ExtraMaths.Map(0, 360, -180, 180, (-Mathf.Atan2(S_previousRotationVec2Direction.y, S_previousRotationVec2Direction.x) * Mathf.Rad2Deg) + 90); }
    }
    protected Vector2 S_rotationVec2Direction;
    protected float f_desiredRotationAngle
    {
        get { return (-Mathf.Atan2(S_rotationVec2Direction.y, S_rotationVec2Direction.x) * Mathf.Rad2Deg) + 90; }
    }
    #endregion    
    #endregion



    #region UnityOverrides

    protected void Start()
    {
        i_bulletLayerMask = ~(LayerMask.GetMask("Bullet") + LayerMask.GetMask("Ignore Raycast"));
        f_currentHealth = f_maxHealth;
    }

    protected void Update()
    {
        if (e_combatState != CombatState.NoControl || e_combatState != CombatState.Dodge)
        {
            Move();
        }
        if (e_combatState != CombatState.NoControl)
        {
            RotateToTarget();
        }
    }

    protected void LateUpdate()
    {
        S_movementVec2DirectionLastFrame = S_movementVec2Direction;
        S_previousRotationVec2Direction = S_rotationVec2Direction;
    }

    #endregion


    #region Functions  
    
    /// <summary>
    /// Movement Related Functions
    /// </summary>

    protected virtual void Move()
    {
        // move
        transform.localPosition += S_velocity * Time.deltaTime;

        // if we are moving then increase the acceleration step
        if (S_movementInputDirection != Vector3.zero)
        {
            if (f_currentAccelerationStep < 1)
            {
                f_currentAccelerationStep += Time.deltaTime;
            }
        }

        float effectiveSpeed;
        //find our desired velocity and our maximum speed change
        if (C_ownedGun != null)
        {
            effectiveSpeed = Mathf.Clamp((f_maxSpeed - C_ownedGun.aC_moduleArray[1].f_movementPenalty), 0, f_maxSpeed);
        }
        else
        {
            effectiveSpeed = Mathf.Clamp((f_maxSpeed), 0, f_maxSpeed);
        }

        Vector3 desiredVelocity = S_movementInputDirection * effectiveSpeed * C_accelerationCurve.Evaluate(f_currentAccelerationStep);
        float maxSpeedChange = f_maxAcceleration * Time.deltaTime;

        //move smoothly towards our desired velocity from our current veolicty
        S_velocity.x = Mathf.MoveTowards(S_velocity.x, desiredVelocity.x, maxSpeedChange);
        S_velocity.z = Mathf.MoveTowards(S_velocity.z, desiredVelocity.z, maxSpeedChange);
        CheckCollisions();
    }

    protected void ChangeMovementDirection(Vector2 input)
    {
        if (e_combatState != CombatState.Dodge && e_combatState != CombatState.NoAttack && e_combatState != CombatState.NoControl)
        {
            S_movementVec2Direction = Vector2.ClampMagnitude(input, 1f);
        }
    }

    protected void StopMovementDirection()
    {
        S_movementVec2Direction = Vector2.zero;
        f_currentAccelerationStep = 0;
    }

    protected void Dodge()
    {
        StartCoroutine(DodgeRoutine());
    }


    public void ZeroVelocity()
    {
        S_velocity = Vector3.zero;
    }

    public void AddVelocity(Vector3 velToAdd)
    {
        if(e_combatState != CombatState.Frozen)
        {
            S_velocity += velToAdd;
        }
    }


    protected void RotateToTarget()
    {
        // Rotate
        transform.rotation = Quaternion.Euler(0,
            Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, f_desiredRotationAngle, ref f_rotationalVelocity, f_rotationTime),
            0);

        // based on difference between current rotation and last rotation add to rotation acceleration
        float rotationAngleDifference = Vector3.Angle(transform.forward, Quaternion.Euler(0, f_desiredRotationAngle, 0) * Vector3.forward);

        //dot product, positve/negative
        if (Vector3.Dot(transform.forward, Quaternion.Euler(0, f_desiredRotationAngle, 0) * Vector3.right) < 0)
        {
            rotationAngleDifference = -rotationAngleDifference;
        }

        // if we are close enough to our rotationn stop otherwise add velocity to our rotational velocity
        if (rotationAngleDifference <= 0.02 && rotationAngleDifference >= -0.02)
        {
            f_rotationalAcceleration = 0;
            f_rotationalVelocity = 0;
        }
        else
        {
            f_rotationalAcceleration = rotationAngleDifference * Time.deltaTime;
            f_rotationalVelocity += f_rotationalAcceleration * Time.deltaTime;
        }
    }

    protected void SetRotationDirection(Vector2 input)
    {
        S_rotationVec2Direction = Vector2.ClampMagnitude(input, 1f);
    }

    public Vector3 GetRotationDirection()
    {
        return new Vector3(S_rotationVec2Direction.x, 0, S_rotationVec2Direction.y);
    }

    private void CheckCollisions()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.localPosition, f_size, Vector3.right, out hit, f_size, i_bulletLayerMask) && S_velocity.x > 0)
        {
            S_velocity.x = -S_velocity.x * f_collisionBounciness;
        }
        else if (Physics.SphereCast(transform.localPosition, f_size, -Vector3.right, out hit, f_size, i_bulletLayerMask) && S_velocity.x < 0)
        {
            S_velocity.x = -S_velocity.x * f_collisionBounciness;
        }
        if (Physics.SphereCast(transform.localPosition, f_size, Vector3.forward, out hit, f_size, i_bulletLayerMask) && S_velocity.z > 0)
        {
            S_velocity.z = -S_velocity.z * f_collisionBounciness;
        }
        else if (Physics.SphereCast(transform.localPosition, f_size, -Vector3.forward, out hit, f_size, i_bulletLayerMask) && S_velocity.z < 0)
        {
            S_velocity.z = -S_velocity.z * f_collisionBounciness;
        }
    }


    /// <summary>
    /// Damage Related Functions
    /// </summary>

    public void Damage(float damage)
    {
        f_currentHealth -= damage;
        StartCoroutine(ChangeStateForSeconds(CombatState.Invincible, f_invincibleTime));
        if (f_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float heal)
    {
        f_currentHealth += heal;
        f_currentHealth = Mathf.Clamp(f_currentHealth, 0, f_maxHealth);
    }

    public virtual void Die()
    {
        gameObject.SetActive(false);
        b_isDead = true;
        ChangeState(CombatState.Normal);
        //Invoke("Respawn", 5);
    }

    public void Respawn()
    {
        gameObject.SetActive(true);
        f_currentHealth = f_maxHealth;
        b_isDead = false;
    }

    /// <summary>
    /// Shooting Related Functions
    /// </summary>

    public void FireGun()
    {
        if (e_combatState != CombatState.Dodge && e_combatState != CombatState.NoAttack && e_combatState != CombatState.NoControl)
            C_ownedGun.StartFire();
    }

    public void CancelGun()
    {
        C_ownedGun.CancelFire();
        if (e_combatState == CombatState.Dodge)
        {
            b_fireCancelWhileDodging = true;
        }
    }

    public void ReloadGun()
    {
        C_ownedGun.Reload();
    }

    public void ApplyBulletElement(GunModule.BulletEffectInfo bulletEffectInfo, float damage)
    {
        switch (bulletEffectInfo.e_bulletEffect)
        {
            case GunModule.BulletEffect.None:
                break;
            case GunModule.BulletEffect.DamageOverTime:

                break;
            case GunModule.BulletEffect.Slow:
                if (e_combatState != CombatState.Frozen)
                    if (f_slowMultiplier > 0)
                    {
                        f_slowMultiplier -= bulletEffectInfo.f_slowPercent;
                        f_slowMultiplier = Mathf.Clamp(f_slowMultiplier, 0, 1);
                    }
                break;
            case GunModule.BulletEffect.Chain:

                break;
            case GunModule.BulletEffect.Vampire:

                break;
        }
    }

    public void ChangeState(CombatState state)
    {
        e_combatState = state;
    }


    #endregion

    #region Coroutines

    private IEnumerator ChangeStateForSeconds(CombatState state, float seconds)
    {
        ChangeState(state);
        yield return new WaitForSeconds(seconds);
        ChangeState(CombatState.Normal);
    }

    //set invincible, don't let player control direction
    //move certain amount quickly
    //set state to normal
    private IEnumerator DodgeRoutine()
    {
        yield return new WaitForSeconds(f_dodgeStartDelay);
        bool firingAtStartOfDodge = C_ownedGun.b_isFiring;
        if (firingAtStartOfDodge)
        {
            C_ownedGun.CancelFire();
        }


        ChangeState(CombatState.Dodge);
        GetComponent<Renderer>().material = C_dodgeMaterial;

        Vector3 startPosition = transform.localPosition;
        float dodgeDistance = f_dodgeLength;
        float dodgeTime = f_dodgeTime;
        float timeSinceStart = 0;

        RaycastHit hit;
        if (Physics.SphereCast(transform.localPosition, f_size, S_movementInputDirection, out hit, f_dodgeLength, i_bulletLayerMask))
        {
            dodgeDistance = hit.distance - f_size;
            float dodgePercentage = dodgeDistance / f_dodgeLength;
            dodgeTime = f_dodgeTime * dodgePercentage;

        }

        Vector3 goalPosition = transform.position + (S_movementInputDirection.normalized * dodgeDistance);


        while (dodgeTime > timeSinceStart)
        {
            transform.position = Vector3.Lerp(startPosition, goalPosition, C_dodgeCurve.Evaluate(timeSinceStart / dodgeTime));
            yield return 0;
            timeSinceStart += Time.deltaTime;
        }

        transform.rotation = transform.rotation * Quaternion.Euler(0, 0, 0);

        ChangeState(CombatState.Normal);
        GetComponent<Renderer>().material = C_defaultMaterial;
        if (!b_fireCancelWhileDodging && firingAtStartOfDodge)
        {
            C_ownedGun.StartFire();
        }
        b_fireCancelWhileDodging = false;
    }

    protected IEnumerator SpeedUpAfterTime(float effectTime, float increaseAmount)
    {
        yield return new WaitForSeconds(effectTime);
        if (e_combatState != CombatState.Frozen)
        {
            f_slowMultiplier += increaseAmount;
        }
        else
        {
            f_slowMultiplier = 0;
        }
    }

    protected IEnumerator ResetAfterFrozen(float effectTime)
    {
        yield return new WaitForSeconds(effectTime);
        S_velocity = Vector3.zero;
        f_slowMultiplier = 1;
        ChangeState(CombatState.Normal);
    }

    #endregion

}
