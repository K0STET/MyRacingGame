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
            Debug.Log("Начинаю отсчёт");
            startTime = System.DateTime.UtcNow;
            count = true;

            
        }

        if(count && absoluteCarSpeed >= 100)
        {
            System.TimeSpan ts = System.DateTime.UtcNow - startTime;
            Debug.Log("До 100 км/ч за: " + ts.Seconds.ToString() + ","+ ts.Milliseconds.ToString("###,00") + " секунд");
            Debug.Log("До 100 км/ч за: " + ts.ToString(@"s\,fff"));
            count = false;
        }
    }
}
