using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class DriftManager : MonoBehaviour
{
    public Rigidbody rb;
    public GameObject driftingObject;
    public TMP_Text totalScoreText;
    public TMP_Text currentScoreText;
    public TMP_Text factorText;
    public Color normalDriftColor;
    public Color nearStopColor;
    public Color driftEndedColor;

    private float speed = 0;
    private float driftAngle;
    private float driftFactor = 1;
    private float currentScore;
    private float totalScore;

    private bool isDrifting = false;

    public float minimumSpeed = 5;
    public float minimumAngle = 4;
    public float driftingDelay = 0.2f;

    private IEnumerator stopDriftingCoroutine = null;

    void Start()
    {
        driftingObject.SetActive(false);
    }

    
    void FixedUpdate()
    {
        ManageDrift();
        ManageUI();
    }

    void ManageDrift()
    {
        speed = rb.velocity.magnitude;
        driftAngle = Vector3.Angle(rb.transform.forward, (rb.velocity + rb.transform.forward).normalized);
        if (driftAngle >= minimumAngle && speed > minimumSpeed)
        {
            if (!isDrifting || stopDriftingCoroutine!=null)
            {
                StartDrift();
            }
        }
        else
        {           
                StopDrift();
        }
        if (isDrifting)
        {
            currentScore += Mathf.Abs(driftAngle) * Time.deltaTime;
            driftFactor += Time.deltaTime;
            driftingObject.SetActive(true);
        }
    }

    void ManageUI()
    {
        totalScoreText.text = "Total Score: " + totalScore.ToString("###,###,000");
        factorText.text = driftFactor.ToString("###,###,##0.0") + "X";
        currentScoreText.text = currentScore.ToString("000");
    }

    void StartDrift()
    {
        if (!isDrifting)
        {
            driftFactor = 1;
        }
        if (stopDriftingCoroutine != null)
        {
            StopCoroutine(stopDriftingCoroutine);
            stopDriftingCoroutine = null;
        }
        currentScoreText.color = normalDriftColor;
        isDrifting = true;
    }

    void StopDrift()
    {
        stopDriftingCoroutine = StoppingDrift();
        StartCoroutine(stopDriftingCoroutine);
    }

    private IEnumerator StoppingDrift()
    {
        yield return new WaitForSeconds(0.1f);
        currentScoreText.color = nearStopColor;
        yield return new WaitForSeconds(0.2f);
        totalScore += currentScore;
        isDrifting = false;
        currentScoreText.color = driftEndedColor;
        yield return new WaitForSeconds(0.6f);
        driftingObject.SetActive(false);
        currentScore = 0;
        currentScoreText.text = currentScore.ToString();
    }
}
