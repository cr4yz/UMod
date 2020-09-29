using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Movement : TimeStepMonoBehaviour
{

    [Header("Movement Configuration")]
    public float MoveSpeed = 6.9f;
    public float Gravity = 20.3199f;
    public float Acceleration = 5f;
    public float AirAcceleration = 1500f;
    public float StopSpeed = 1.9f;
    public float AirCap = .76f;
    public float Friction = 4f;
    public float JumpPower = 7.15f;

    [Header("Mouse Configuration")]
    public float MouseSensitivity = 1f;

    [Header("Other")]
    public Camera Camera;
    public Collider Collider;

    [HideInInspector]
    public Vector3 MoveVector;

    private MovementButtons _buttonsDown;
    private Vector3 _previousOrigin;

    private Vector3 _origin;
    public Vector3 Origin
    {
        get { return _origin; }
        set { 
            _origin = value;
            transform.position = value;
        }
    }
    private Vector3 _angles;
    public Vector3 Angles
    {
        get { return _angles; }
        set
        {
            _angles = value;
            Camera.transform.eulerAngles = value;
        }
    }
    public bool Grounded { get; private set; }
    public bool Surfing { get; private set; }
    public bool JustJumped { get; private set; }

    private void Start()
    {
        if (!Collider)
        {
            Collider = GetComponent<Collider>();
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Origin = transform.position;
        Angles = Camera.transform.eulerAngles;
    }

    protected override void OnFrame()
    {
        TryMouseLook(GetMouseDelta());
        _buttonsDown = GetButtons();

        Camera.transform.rotation = Quaternion.Euler(Angles);
        transform.position = Vector3.Lerp(_previousOrigin, Origin, base.Alpha);
    }

    protected override void OnTick()
    {
        CheckForGravity();
        CheckForGround();
        CheckForJump();

        var inputVector = GetInputVector(_buttonsDown);
        var wishDir = inputVector.normalized;
        var wishSpeed = inputVector.magnitude;

        if (Grounded)
        {
            MoveVector = GroundAccelerate(MoveVector, wishDir, wishSpeed, Acceleration, base.FixedDeltaTime, 1f);
            MoveVector = AddFriction(MoveVector, StopSpeed, Friction, base.FixedDeltaTime);
        }
        else
        {
            MoveVector = AirAccelerate(MoveVector, wishDir, wishSpeed, AirAcceleration, AirCap, base.FixedDeltaTime);
        }

        //MoveVector = ClampVelocity(MoveVector, MoveSpeed);
        _previousOrigin = Origin;
        Origin += MoveVector * base.FixedDeltaTime;

        ResolveCollisions();
    }

    private Vector2 GetMouseDelta()
    {
        return new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
    }

    private MovementButtons GetButtons()
    {
        var result = MovementButtons.None;

        var sideMove = Input.GetAxisRaw("Horizontal");
        var forwardMove = Input.GetAxisRaw("Vertical");

        if(sideMove > 0)
        {
            result |= MovementButtons.Right;
        }
        else if(sideMove < 0)
        {
            result |= MovementButtons.Left;
        }

        if(forwardMove > 0)
        {
            result |= MovementButtons.Forward;
        }
        else if(forwardMove < 0)
        {
            result |= MovementButtons.Back;
        }

        if(Input.GetAxisRaw("Jump") > 0)
        {
            result |= MovementButtons.Jump;
        }

        return result;
    }

    private Vector3 GetInputVector(MovementButtons buttons)
    {
        var sideMove = _buttonsDown.HasFlag(MovementButtons.Right)
            ? MoveSpeed
            : _buttonsDown.HasFlag(MovementButtons.Left) 
            ? -MoveSpeed 
            : 0;
        var fwdMove = _buttonsDown.HasFlag(MovementButtons.Forward)
            ? MoveSpeed
            : _buttonsDown.HasFlag(MovementButtons.Back)
            ? -MoveSpeed
            : 0;
        var inputVector = new Vector3(sideMove, 0, fwdMove);
        inputVector = Camera.transform.TransformDirection(inputVector);
        inputVector.y = 0;
        return inputVector;
    }

    private void TryMouseLook(Vector2 mouseDelta)
    {
        if(Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }
        var rotationVector = new Vector3(-mouseDelta.y, mouseDelta.x, 0);
        Angles += rotationVector * MouseSensitivity;
    }

    private void CheckForGravity()
    {
        if (!Grounded)
        {
            MoveVector.y -= Gravity * base.FixedDeltaTime;
        }
    }

    private void CheckForJump()
    {
        JustJumped = false;

        if (Grounded && _buttonsDown.HasFlag(MovementButtons.Jump))
        {
            MoveVector.y += JumpPower;
            Grounded = false;
            JustJumped = true;
        }
    }

    private void CheckForGround()
    {
        Surfing = false;

        if (JustJumped)
        {
            return;
        }

        var hits = Physics.BoxCastAll(center: Collider.bounds.center + new Vector3(0, .1f, 0),
            halfExtents: Collider.bounds.extents * .99f,
            direction: Vector3.down,
            orientation: Quaternion.identity,
            maxDistance: .11f + Collider.bounds.extents.y * .005f,
            layerMask: 1 << 0);

        var validHits = hits.ToList().FindAll(x => x.normal.y >= 0.7f && x.point != Vector3.zero).OrderBy(x => x.distance);
        Grounded = validHits.Count() > 0;

        if (Grounded)
        {
            if (validHits.Count() > 0)
            {
                var best = validHits.First();
                if (best.normal.y < 1)
                {
                    MoveVector = ClipVelocity(MoveVector, best.normal, 1.0f);
                }
                else
                {
                    MoveVector.y = 0;
                }
            }
        }
        else
        {
            var surfHits = hits.ToList().FindAll(x => x.normal.y < 0.7f && x.point != Vector3.zero).OrderBy(x => x.distance);
            if (surfHits.Count() > 0)
            {
                Origin += surfHits.First().normal * 0.02f;
                MoveVector = ClipVelocity(MoveVector, surfHits.First().normal, 1.0f);
                Surfing = true;
            }
        }
    }

    private void ResolveCollisions()
    {
        var overlaps = Physics.OverlapBox(Collider.bounds.center, Collider.bounds.extents, Quaternion.identity, 1 << 0);

        foreach (var other in overlaps)
        {
            if(other == Collider)
            {
                continue;
            }
            if(Physics.ComputePenetration(Collider, Origin, Quaternion.identity, other, other.transform.position, other.transform.rotation, out Vector3 direction, out float distance))
            {
                if (Vector3.Dot(direction, MoveVector.normalized) > 0)
                {
                    continue;
                }

                Vector3 penetrationVector = direction * distance;

                if (!Surfing)
                {
                    Origin += penetrationVector;
                    MoveVector -= Vector3.Project(MoveVector, -direction);
                }
                else
                {
                    MoveVector = ClipVelocity(MoveVector, direction, 1.0f);
                }
            }
        }
    }

    private Vector3 GroundAccelerate(Vector3 input, Vector3 wishDir, float wishSpeed, float accel, float deltaTime, float surfaceFriction)
    {
        var currentSpeed = Vector3.Dot(input, wishDir);
        var addSpeed = wishSpeed - currentSpeed;

        if (addSpeed <= 0)
        {
            return input;
        }

        var accelspeed = Mathf.Min(accel * deltaTime * wishSpeed * surfaceFriction, addSpeed);
        input += accelspeed * wishDir;
        return input;
    }

    private Vector3 AirAccelerate(Vector3 input, Vector3 wishDir, float wishSpeed, float accel, float airCap, float deltaTime)
    {
        var wishSpd = Mathf.Min(wishSpeed, airCap);
        var currentSpeed = Vector3.Dot(input, wishDir);
        var addSpeed = wishSpd - currentSpeed;

        if (addSpeed <= 0)
        {
            return input;
        }

        var accelspeed = Mathf.Min(addSpeed, accel * wishSpeed * deltaTime);
        input += accelspeed * wishDir;
        return input;
    }

    private Vector3 AddFriction(Vector3 input, float stopSpeed, float friction, float deltaTime)
    {
        var speed = input.magnitude;

        if (speed < 0.0001905f)
        {
            return input;
        }

        var drop = 0f;
        var control = (speed < stopSpeed) ? stopSpeed : speed;
        drop += control * friction * deltaTime;
        var newspeed = Mathf.Max(speed - drop, 0);

        if (newspeed != speed)
        {
            newspeed /= speed;
            input *= newspeed;
        }
        return input;
    }

    private Vector3 ClipVelocity(Vector3 input, Vector3 normal, float overbounce)
    {
        var backoff = Vector3.Dot(input, normal) * overbounce;

        for (int i = 0; i < 3; i++)
        {
            var change = normal[i] * backoff;
            input[i] = input[i] - change;
        }

        var adjust = Vector3.Dot(input, normal);
        if (adjust < 0.0f)
        {
            input -= (normal * adjust);
        }
        return input;
    }

}
