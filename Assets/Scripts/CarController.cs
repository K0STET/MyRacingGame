using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [SerializeField] private Wheel[] _wheels;

    [Header("Car Settings")]
    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;
    [SerializeField] private int maxSpeed;
    [SerializeField] private float maxBackSpeed;
    [SerializeField] private GameObject CenterOfMass;
    [SerializeField] private AnimationCurve _steerCurve;

    private Rigidbody _rb;

    private float _horizontalInput;
    private float _verticalInput;
    private float _brakeInput;
    private float _movingDirection;

    private float _currentSteerAngle;
    private float _speed;

    private bool _isBreaking;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = CenterOfMass.transform.localPosition;
    }

    private void FixedUpdate()
    {
        GetInput();
        Move();
        Breaking();
        Steering();
        MaxSpeedCheck();
        HandBreak();

        _speed = _rb.velocity.magnitude * 3.6f;
    }

    private void GetInput()
    {
        _horizontalInput = Input.GetAxis("Horizontal");

        _verticalInput = Input.GetAxis("Vertical");

        _movingDirection = Vector3.Dot(transform.forward, _rb.velocity);

        _brakeInput = (_movingDirection < -0.5f && _verticalInput > 0) || (_movingDirection > 0.5f && _verticalInput < 0) ? Mathf.Abs(_verticalInput) : 0;
    }

    private void Move()
    {
        foreach (Wheel wheel in _wheels)
        {           
            wheel.WheelCollider.motorTorque = motorForce * _verticalInput;
            wheel.UpdateWheels();
        }
    }

    private void Steering()
    {
        _currentSteerAngle = _steerCurve.Evaluate(_speed) * _horizontalInput;

        float steerAngle = _currentSteerAngle;
        float slipAngle = Vector3.Angle(transform.forward,_rb.velocity-transform.forward);

        if (slipAngle < 120) steerAngle += Vector3.SignedAngle(transform.forward, _rb.velocity, Vector3.up);

        steerAngle = Mathf.Clamp(steerAngle, -maxSteerAngle,maxSteerAngle);

        foreach (Wheel wheel in _wheels)
        {
            if (wheel.IsFrontWheel) wheel.WheelCollider.steerAngle = steerAngle;
        }
    }

    private void Breaking()
    {
        foreach(Wheel wheel in _wheels)
            if (wheel.IsFrontWheel) wheel.WheelCollider.brakeTorque = breakForce * _brakeInput * (wheel.IsFrontWheel ? 0.5f : 0.9f);      
        
        if (Input.GetKey(KeyCode.Space))
        {
            _isBreaking = true;
        }
        else
        {
            _isBreaking = false;
        }
    }

    private void HandBreak()
    {
        foreach (Wheel wheel in _wheels)
        {
            if (_isBreaking)
            {
                if (wheel.IsRearWheel) wheel.WheelCollider.brakeTorque = breakForce * (wheel.IsRearWheel ? 0.9f : 0f);
            }
            else
            {
                if (wheel.IsRearWheel) wheel.WheelCollider.brakeTorque = 0f;
            }
        }
    }

    void MaxSpeedCheck()
    {
        if (_speed > maxSpeed)
        {
            _speed = maxSpeed;
           _rb.velocity = (int)(maxSpeed / 3.6f) * _rb.velocity.normalized;
        }
        if (_movingDirection < 0)
        {
            if (_verticalInput < 0)
            {
                if (_speed > maxBackSpeed)
                {
                    _speed = maxBackSpeed;
                    _rb.velocity = (int)(maxBackSpeed / 3.6f) * _rb.velocity.normalized;
                }
            }
        }
    }
}

[System.Serializable]
public struct Wheel
{
    public Transform WheelTransform;
    public WheelCollider WheelCollider;
    public bool IsFrontWheel;
    public bool IsRearWheel;

    public void UpdateWheels()
    {
        Vector3 pos;
        Quaternion rot;
        WheelCollider.GetWorldPose(out pos, out rot);
        WheelTransform.rotation = rot;
        WheelTransform.position = pos;
    }
}
