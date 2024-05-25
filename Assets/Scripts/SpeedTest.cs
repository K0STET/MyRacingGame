using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedTest : MonoBehaviour
{
    private System.DateTime startTime;
    private bool count = false;
    private float speed;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        speed = (rb.velocity.magnitude * 3.6f);
        float absoluteCarSpeed = Mathf.Abs(speed);

        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("������� ������");
            startTime = System.DateTime.UtcNow;
            count = true;

            
        }

        if(count && absoluteCarSpeed >= 100)
        {
            System.TimeSpan ts = System.DateTime.UtcNow - startTime;
            Debug.Log("�� 100 ��/� ��: " + ts.Seconds.ToString() + ","+ ts.Milliseconds.ToString("###,00") + " ������");
            Debug.Log("�� 100 ��/� ��: " + ts.ToString(@"s\,fff"));
            count = false;
        }
    }
}
