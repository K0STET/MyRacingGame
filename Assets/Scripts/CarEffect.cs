using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CarEffect : MonoBehaviour
{
    [SerializeField] private TrailRenderer[] tireMarks;
    [SerializeField] private ParticleSystem[] wheelSmoke;
    [SerializeField] private TMP_Text speedText;

    private bool tiremarksFlag;
    private bool isGrounded;

    private Rigidbody rb;

    private float _driftAngle;
    private float _movingDirection;
    public float _speed;

    public LayerMask OnGround;

    public float Drag;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.drag = Drag;
        rb.angularDrag = Drag;
    }

    
    void FixedUpdate()
    {
        CheckDrift();
        CheckGround();
        SpeedInput();
    }

    void CheckGround()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1f,OnGround);


        if (isGrounded)
        {
            rb.drag = Drag;
            rb.angularDrag = Drag;
        }
        else
        {
            rb.drag = 0.05f;
            rb.angularDrag = 0.05f;
        }
    }

    void CheckDrift()
    {
        _movingDirection  = Vector3.Dot(transform.forward, rb.velocity);
        _driftAngle  = Vector3.Angle(rb.transform.forward, (rb.velocity + rb.transform.forward).normalized);

        if (_driftAngle >= 12 && isGrounded)
        {
            if (_movingDirection > 0.01f) StartEmmiter();
        }
        else
        {
            StopEmmiter();
        }
    }

    void StartEmmiter()
    {
        if (tiremarksFlag) return;
        foreach (TrailRenderer T in tireMarks)
        {
            T.emitting = true;
        }
        foreach (ParticleSystem P in wheelSmoke)
        {
            P.Play();
        }

        tiremarksFlag = true;
    }

    void StopEmmiter()
    {
        if (!tiremarksFlag) return;
        foreach (TrailRenderer T in tireMarks)
        {
            T.emitting = false;
        }

        foreach (ParticleSystem P in wheelSmoke)
        {
            P.Stop();
        }

        tiremarksFlag = false;
    }

    void SpeedInput()
    {
        _speed = rb.velocity.magnitude * 3.6f;
        float absoluteCarSpeed = Mathf.Abs(_speed);
        speedText.text = Mathf.Round(absoluteCarSpeed).ToString("0") + " km/h";
    }
}
