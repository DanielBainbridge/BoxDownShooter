using Gun;
using Managers;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;
using static Enemy.EnemyBase;
using static Gun.GunModule;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Enum setups for further use in variables
    /// </summary>
    public enum PlayerState
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
    /// Variables for player controller, divided into sections noted by headers
    /// </summary>
    /// 
    [Header("Control")]
    private PlayerInput C_playerInput;
    [Rename("Controller Dead Zone Percentage"), Range(0, 1), SerializeField] private float f_controllerDeadZone = 0.15f;

    // Movement Variables, distances are in m/s, rotations are in revolutions/s
    [Header("Movement")]
    [Rename("Max Speed")] public float f_maxSpeed = 15;
    [Rename("Max Acceleration")] public float f_maxAcceleration = 40;
    private float f_currentAccelerationStep = 0;
    [Rename("Acceleration Curve")] public AnimationCurve C_accelerationCurve;
    float f_playerSize = 0.4f;


    private Vector2 S_movementVec2Direction;
    private Vector2 S_movementVec2DirectionLastFrame;
    private Vector3 S_movementInputDirection
    {
        get { return new Vector3(S_movementVec2Direction.x, 0, S_movementVec2Direction.y); }
    }

    public Vector3 GetRotationDirection()
    {
        return new Vector3(S_rotationVec2Direction.x, 0, S_rotationVec2Direction.y);
    }


    [Header("Rotation")]
    [Rename("Rotation Time"), Range(0, 0.5f)] public float f_rotationTime = 0.1f;

    private Vector2 S_previousRotationVec2Direction;
    private float f_previousYRotationAngle
    {
        get { return ExtraMaths.Map(0, 360, -180, 180, (-Mathf.Atan2(S_previousRotationVec2Direction.y, S_previousRotationVec2Direction.x) * Mathf.Rad2Deg) + 90); }
    }
    private Vector2 S_rotationVec2Direction;
    private float f_desiredRotationAngle
    {
        get { return (-Mathf.Atan2(S_rotationVec2Direction.y, S_rotationVec2Direction.x) * Mathf.Rad2Deg) + 90; }
    }

    [Header("Dodge")]
    [Rename("Dodge Startup")] public float f_dodgeStartDelay = 0.12f;
    [Rename("Dodge Length")] public float f_dodgeLength = 2.5f;
    [Rename("Dodge Time")] public float f_dodgeTime = 1;
    [Rename("Dodge Animation Curve")] public AnimationCurve C_dodgeCurve;

    // Setup for dodge refil for balancing later down the line
    //int i_dodgeMax = 3;
    //int i_dodgeCount; // to be set on start

    [Header("Fake Physics")]
    float f_stepHeight = 0.2f;
    float f_standRadius = 0.8f;
    float f_dampStrength = 0.4f;
    float f_coyoteTime = 0.8f;
    [Rename("Collision Bounce Percentage"), Range(0, 1)] public float f_collisionBounciness = 0.45f;

    [Header("Game Variables")]
    [Rename("Interact Range")] public float f_interactRange = 3.0f;
    [Rename("Health")] float f_health = 100;
    [Rename("Spawn Location")] Vector3 S_spawnLocation; // player set to this on start and before loading into new scene
    [Rename("Default Player Material")] public Material C_defaultMaterial;
    [Rename("Dodge Player Material")] public Material C_dodgeMaterial;

    // public Gun playerGun owned gun goes here.
    [Rename("Player Gun"), SerializeField] public Gun.Gun C_playerGun = null;

    [HideInInspector] public PlayerState e_playerState = PlayerState.Normal;


    // run time
    private Vector3 S_acceleration;
    private Vector3 S_velocity = Vector3.zero;
    private float f_rotationalAcceleration;
    private float f_rotationalVelocity;
    private ControlManager C_controlManagerReference = null;
    private float f_healthCurrent; // to be set on start
    private int i_livesCurrent; // to be set on start
    private bool b_isDead = false;
    private int i_bulletLayerMask;
    private bool b_fireCancelWhileDodging;
    [Range(0,1)]private float f_slowMultiplier = 0;

    ///<summary>
    /// Player methods, the method name should be self explanitory if not there is reference 
    ///</summary>

    private void OnEnable()
    {
        // reference control manager
        C_controlManagerReference = FindObjectOfType<ControlManager>();

        // action map control setup
        C_playerInput = GetComponent<PlayerInput>();
        C_playerInput.SwitchCurrentActionMap("PlayerControl");
        InputActionMap actionMap = C_playerInput.currentActionMap;
        actionMap.Enable();
        actionMap.FindAction("Movement").performed += Move;
        actionMap.FindAction("Movement").canceled += StopMove;
        actionMap.FindAction("Rotate").performed += RotationSet;
        actionMap.FindAction("Dodge").performed += Dodge;
        actionMap.FindAction("Interact").performed += Interact;
        actionMap.FindAction("Fire").performed += Fire;
        actionMap.FindAction("Fire").canceled += CancelFire;
        actionMap.FindAction("Reload").performed += Reload;
        actionMap.FindAction("Pause").performed += Pause;
        i_bulletLayerMask = ~(LayerMask.GetMask("Bullet") + LayerMask.GetMask("Ignore Raycast"));
    }


    private void Update()
    {
        if (e_playerState != PlayerState.NoControl || e_playerState != PlayerState.Dodge)
        {
            MovePlayer();
        }
        if (e_playerState != PlayerState.NoControl)
        {
            RotatePlayerToTarget();
        }
        C_controlManagerReference.ChangeInputDevice(C_playerInput.currentControlScheme);
    }
    private void LateUpdate()
    {
        S_movementVec2DirectionLastFrame = S_movementVec2Direction;
        S_previousRotationVec2Direction = S_rotationVec2Direction;
    }


    // input callbacks
    private void Move(InputAction.CallbackContext context)
    {
        Vector2 inputValue = context.ReadValue<Vector2>();
        if (inputValue.magnitude > f_controllerDeadZone)
            S_movementVec2Direction = Vector2.ClampMagnitude(inputValue, 1f);
    }
    private void StopMove(InputAction.CallbackContext context)
    {
        S_movementVec2Direction = context.ReadValue<Vector2>();
        f_currentAccelerationStep = 0;
    }
    private void RotationSet(InputAction.CallbackContext context)
    {
        Vector2 inputValue = context.ReadValue<Vector2>();
        //if control scheme mouse/keyb because player will need to rotate to mouse pos
        if (C_controlManagerReference.GetCurrentControllerType() == ControlManager.CurrentControllerType.KeyboardMouse)
        {
            Vector3 dir = new Vector3(inputValue.x, inputValue.y, 0) - Camera.main.WorldToScreenPoint(transform.position);
            S_rotationVec2Direction = Vector2.ClampMagnitude(new Vector2(dir.x, dir.y) / Screen.height, 1.0f);
        }
        else
        {
            if (inputValue.magnitude > f_controllerDeadZone)
                S_rotationVec2Direction = Vector2.ClampMagnitude(inputValue, 1.0f);
        }
    }
    private void Fire(InputAction.CallbackContext context)
    {
        if (e_playerState != PlayerState.Dodge && e_playerState != PlayerState.NoAttack && e_playerState != PlayerState.NoControl)
            C_playerGun.StartFire();
    }
    private void CancelFire(InputAction.CallbackContext context)
    {
        C_playerGun.CancelFire();
        if(e_playerState == PlayerState.Dodge)
        {
            b_fireCancelWhileDodging = true;
        }
    }
    private void Dodge(InputAction.CallbackContext context)
    {
        if (e_playerState != PlayerState.Dodge && e_playerState != PlayerState.NoAttack && e_playerState != PlayerState.NoControl)
        {
            StartCoroutine(DoDodge());
        }
        //set invincible, don't let player control direction
        //move certain amount quickly
        //set state to normal
    }
    private void Interact(InputAction.CallbackContext context)
    {
        //check for interactables in radius, if none early out
        //find distance of all in radius
        //interact with shortest range
        float closestDistance = float.MaxValue;
        int closestCollisionReference = 0;
        Collider[] collisions = Physics.OverlapSphere(transform.position, f_interactRange);
        if (collisions.Length == 0)
        {
            return;
        }
        for (int i = 0; i < collisions.Length; i++)
        {
            float distance = (collisions[i].transform.position - transform.position).magnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCollisionReference = i;
            }
        }
        if (collisions[closestCollisionReference].transform.tag == "Gun Module")
        {
            GunModule gunModuleToSwap = (GunModule)Resources.Load(GunModuleSpawner.GetGunModuleResourcesPath(collisions[closestCollisionReference].name));

            C_playerGun.SwapGunPiece(gunModuleToSwap);
            Destroy(collisions[closestCollisionReference].gameObject);
        }
    }
    private void Reload(InputAction.CallbackContext context)
    {
        // reload clip of bullets to max
        C_playerGun.Reload();
    }
    private void Pause(InputAction.CallbackContext context)
    {
        //bring up a menu
        //swap action map
    }

    // utility methods

    private void MovePlayer()
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

        //find our desired velocity and our maximum speed change
        float effectiveSpeed = Mathf.Clamp((f_maxSpeed - C_playerGun.aC_moduleArray[1].f_movementPenalty), 0, f_maxSpeed);

        Vector3 desiredVelocity = S_movementInputDirection * effectiveSpeed * C_accelerationCurve.Evaluate(f_currentAccelerationStep);
        float maxSpeedChange = f_maxAcceleration * Time.deltaTime;

        //move smoothly towards our desired velocity from our current veolicty
        S_velocity.x = Mathf.MoveTowards(S_velocity.x, desiredVelocity.x, maxSpeedChange);
        S_velocity.z = Mathf.MoveTowards(S_velocity.z, desiredVelocity.z, maxSpeedChange);
        CheckCollisions();
    }

    private void RotatePlayerToTarget()
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


    //TO DO proper dodge collision handling
    private IEnumerator DoDodge()
    {
        yield return new WaitForSeconds(f_dodgeStartDelay);
        bool firingAtStartOfDodge = C_playerGun.b_isFiring;
        if (firingAtStartOfDodge)
        {
            C_playerGun.CancelFire();
        }
        

        e_playerState = PlayerState.Dodge;
        GetComponent<Renderer>().material = C_dodgeMaterial;
        
        Vector3 startPosition = transform.localPosition;
        float dodgeDistance = f_dodgeLength;
        float dodgeTime = f_dodgeTime;
        float timeSinceStart = 0;

        RaycastHit hit;
        if (Physics.SphereCast(transform.localPosition, f_playerSize, S_movementInputDirection, out hit, f_dodgeLength, i_bulletLayerMask))
        {
            dodgeDistance = hit.distance - f_playerSize;
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

        NormaliseState();
        GetComponent<Renderer>().material = C_defaultMaterial;
        if(!b_fireCancelWhileDodging && firingAtStartOfDodge)
        {
            C_playerGun.StartFire();
        }
        b_fireCancelWhileDodging = false;
    }

    private void CheckCollisions()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.localPosition, f_playerSize, Vector3.right, out hit, f_playerSize, i_bulletLayerMask) && S_velocity.x > 0)
        {
            S_velocity.x = -S_velocity.x * f_collisionBounciness;
        }
        else if (Physics.SphereCast(transform.localPosition, f_playerSize, -Vector3.right, out hit, f_playerSize, i_bulletLayerMask) && S_velocity.x < 0)
        {
            S_velocity.x = -S_velocity.x * f_collisionBounciness;
        }
        if (Physics.SphereCast(transform.localPosition, f_playerSize, Vector3.forward, out hit, f_playerSize, i_bulletLayerMask) && S_velocity.z > 0)
        {
            S_velocity.z = -S_velocity.z * f_collisionBounciness;
        }
        else if (Physics.SphereCast(transform.localPosition, f_playerSize, -Vector3.forward, out hit, f_playerSize, i_bulletLayerMask) && S_velocity.z < 0)
        {
            S_velocity.z = -S_velocity.z * f_collisionBounciness;
        }
    }

    public void AddVelocityToPlayer(Vector3 velocityToAdd)
    {
        S_velocity += velocityToAdd;
    }

    private void Die()
    {

    }
    private void Respawn()
    {

    }
    public void DamagePlayer(float damage)
    {
        f_healthCurrent -= damage;
        if (f_healthCurrent <= 0)
        {
            Die();
        }
    }
    public void ApplyBulletElement(GunModule.BulletEffectInfo bulletEffectInfo, float damage)
    {
        switch (bulletEffectInfo.e_bulletEffect)
        {
            case BulletEffect.None:
                break;
            case BulletEffect.DamageOverTime:

                break;
            case BulletEffect.Slow:
                if (e_playerState != PlayerState.Frozen)
                    if(f_slowMultiplier > 0)
                    {
                        f_slowMultiplier -= bulletEffectInfo.f_slowPercent;
                        f_slowMultiplier = Mathf.Clamp(f_slowMultiplier, 0, 1);
                    }
                break;
            case BulletEffect.Chain:

                break;
            case BulletEffect.Vampire:

                break;
        }
    }


    private void HealPlayer()
    {

    }
    private void SetPlayerPosition()
    {

    }
    private void ItemPickup()
    {

    }
    private void ItemDrop()
    {

    }
    private void NormaliseState()
    {
        e_playerState = PlayerState.Normal;
    }
    private void ChangeStateForSeconds()
    {

    }

    // coroutines

    private IEnumerator ChangeState()
    {
        yield return null;
    }

    private void OnDrawGizmos()
    {

    }
}