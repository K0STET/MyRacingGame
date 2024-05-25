using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAI : MonoBehaviour
{
    public Transform path;
    private List<Transform> points;
    private int currentNode = 0;
    public float maxSteerAngle = 40f;
    public float maxMotorTorque;
    public float brakeForce;
    public float currentSpeed;
    public float maxSpeed;
    public Vector3 CenterOfMass;
    public bool isBreaking;

    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = CenterOfMass;
        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        points = new List<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
            {
                points.Add(pathTransforms[i]);
            }
        }
    }

    
    void FixedUpdate()
    {
        ApplySteering();
        Drive();
        CheckWayPointsDistance();
        UpdateWheels();
        ApplyBrake();
    }

    void ApplySteering()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(points[currentNode].position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;

        frontLeftWheelCollider.steerAngle = newSteer;
        frontRightWheelCollider.steerAngle = newSteer;
    }

    void Drive()
    {
        currentSpeed = 2 * Mathf.PI * frontLeftWheelCollider.radius * frontLeftWheelCollider.rpm * 60 / 1000;

        if(currentSpeed < maxSpeed && !isBreaking)
        {
            frontLeftWheelCollider.motorTorque = maxMotorTorque;
            frontRightWheelCollider.motorTorque = maxMotorTorque;
        }
        else
        {
            frontLeftWheelCollider.motorTorque = 0;
            frontRightWheelCollider.motorTorque = 0;
        }
    }

    void ApplyBrake()
    {
        if (isBreaking)
        {
            rearLeftWheelCollider.brakeTorque = brakeForce;
            rearRightWheelCollider.brakeTorque = brakeForce;
        }
        else
        {
            rearLeftWheelCollider.brakeTorque = 0;
            rearRightWheelCollider.brakeTorque = 0;
        }
    }

    void CheckWayPointsDistance()
    {
        if(Vector3.Distance(transform.position,points[currentNode].position) < 0.5f)
        {
            if(currentNode == points.Count - 1)
            {
                currentNode = 0;
            }
            else
            {
                currentNode++;
            }
        }
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Brake"))
        {
            isBreaking = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Brake"))
        {
            isBreaking = false;
        }
    }
}
