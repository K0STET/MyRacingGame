using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarSoundsManager : MonoBehaviour
{
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;

    private float currentSpeed;

    private Rigidbody _rb;
    private AudioSource _carAudio;
    private CarController _carController;

    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;

    [SerializeField] private AudioSource _startCarAudio;
    [SerializeField] private AudioSource _stopCarAudio;

    [SerializeField] private float startTime;
    [SerializeField] private float stopTime;

    private float pitchFromCar;

    public bool isDrive;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _carAudio = GetComponent<AudioSource>();
        _carController = GetComponent<CarController>();
        _carController.enabled = false;
    }

    private void FixedUpdate()
    {
        StartCar();
        EngineSound();
    }

    private void EngineSound()
    {
        currentSpeed = _rb.velocity.magnitude * 3.6f;
        pitchFromCar = _rb.velocity.magnitude / 50f;

        if (currentSpeed < minSpeed)
            _carAudio.pitch = minPitch;
        if (currentSpeed > minSpeed && currentSpeed < maxSpeed)
            _carAudio.pitch = minPitch + pitchFromCar;
        if (currentSpeed > maxSpeed)
            _carAudio.pitch = maxPitch;
    }

    private void StartCar()
    {
        if (Input.GetKeyDown(KeyCode.F) && isDrive == false)
        {
            StartCoroutine(StartEngineSound());
            isDrive = true;
        }
        else if (Input.GetKeyDown(KeyCode.F) && isDrive == true)
        {
            StartCoroutine(StopEngineSound());
            isDrive = false;
        }
    }

    private IEnumerator StartEngineSound()
    {
        _startCarAudio.PlayOneShot(_startCarAudio.clip);
        yield return new WaitForSeconds(startTime);
        _carAudio.Play();
        _carController.enabled = true;
    }

    private IEnumerator StopEngineSound()
    {
        _carController.enabled = false;
        _stopCarAudio.PlayOneShot(_stopCarAudio.clip);
        yield return new WaitForSeconds(stopTime);
        _carAudio.Pause();      
    }
}
