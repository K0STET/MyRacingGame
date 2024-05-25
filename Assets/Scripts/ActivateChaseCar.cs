using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateChaseCar : MonoBehaviour
{
    public ChaseCar chaseCar;
    [SerializeField] private int MaxAllowedSpeed;
    void Start()
    {
        chaseCar.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<Rigidbody>().velocity.magnitude*3.6f >= MaxAllowedSpeed)
        {
            chaseCar.enabled = true;
        }
    }
}
