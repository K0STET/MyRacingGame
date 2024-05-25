using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseCar : MonoBehaviour
{
    public Transform target;
    public Transform place;
    public LayerMask whatIsPlayer;    

    public int maxSpeed;
    public float turnSpeed = 5f;
    public int brakeForce;
    private float currentSpeed;    
    public int maxMotorTorque;
    public float maxSteerAngle;
    public bool playerInSightRange;
    public bool isBreaking;
    public bool isChasing;
    public float sightRange;

    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    [Header("Sensors")]
    public float sensorLenght = 5;
    public Vector3 frontSensorPosition = new Vector3(0f,0.2f,0.5f);
    public float frontsideSensorPosition = 0.2f;
    public float frontSensorAngel = 30f;

    private bool avoiding = false;
    private float targetSteerAngle = 0;

    void Start()
    {
        isBreaking = false;
        isChasing = false;
    }

    void FixedUpdate()
    {

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        UpdateWheels();
        WheelSteering();
        LerpToSteerAngle();


        if (!isBreaking)
        {
            if (Vector3.Distance(transform.position, target.position) <= 100f)
            {
                isChasing = true;     
               
                Drive();
                Sensors();
            }
            else
            {
                isChasing = false;

                ToPlace();
            }
        }
        else
        {
            ApplyBrake();
        }

        if (playerInSightRange)
            isBreaking = true;
        else
            isBreaking = false;
    }

    public void Drive()
    {
        currentSpeed = 2 * Mathf.PI * frontLeftWheelCollider.radius * frontLeftWheelCollider.rpm * 60 / 1000;

        if (currentSpeed < maxSpeed && !isBreaking)
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
    void WheelSteering()
    {
        if (avoiding) return;
        Vector3 relativeVector = transform.InverseTransformPoint(target.position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        targetSteerAngle = newSteer;
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

    void ToPlace()
    {
        if (avoiding) return;
        Vector3 relativeVector = transform.InverseTransformPoint(place.position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;

        targetSteerAngle = newSteer;

        if (Vector3.Distance(transform.position,place.position) < 1f)
        {
            OnPlace();
        }
    }

    void ApplyBrake()
    {
        frontLeftWheelCollider.motorTorque = 10;
        frontRightWheelCollider.motorTorque = 10;
    }

    void OnPlace()
    {
        frontLeftWheelCollider.motorTorque = 0;
        frontRightWheelCollider.motorTorque = 0;
    }

    void Sensors()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        sensorStartPos += transform.forward * frontSensorPosition.z;
        sensorStartPos += transform.up * frontSensorPosition.y;
        float avoidMultiplier = 0;
        avoiding = false;       

        //front right sensor
        sensorStartPos += transform.right * frontsideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLenght))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier -= 1f;
            }            
        }        

        //front right angle sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngel, transform.up) * transform.forward, out hit, sensorLenght))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier -= 0.5f;
            }
        }       

        //front left sensor
        sensorStartPos -= transform.right * frontsideSensorPosition * 2;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLenght))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier += 1f;
            }
        }
 
        //front left angle sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngel, transform.up) * transform.forward, out hit, sensorLenght))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier += 0.5f;
            }
        }

        //front center sensor
        if (avoidMultiplier == 0)
        {
            if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLenght))
            {
                if (!hit.collider.CompareTag("Terrain"))
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    avoiding = true;
                    if(hit.normal.x < 0)
                    {
                        avoidMultiplier = -1;
                    }
                    else
                    {
                        avoidMultiplier = 1;
                    }
                }
            }
        }      

        if (avoiding)
        {
            targetSteerAngle = maxSteerAngle * avoidMultiplier;
        }
    }
    
    private void LerpToSteerAngle()
    {
        frontLeftWheelCollider.steerAngle = Mathf.Lerp(frontLeftWheelCollider.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
        frontRightWheelCollider.steerAngle = Mathf.Lerp(frontRightWheelCollider.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);

    }
}
