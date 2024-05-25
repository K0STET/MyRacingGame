using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public Color lineColor;

    private List<Transform> points = new List<Transform>();

    private void Start()
    {
        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = lineColor;

        Transform[] pathTransforms = GetComponentsInChildren<Transform>();
        points = new List<Transform>();

        for(int i = 0; i < pathTransforms.Length; i++)
        {
            if(pathTransforms[i] != transform)
            {
                points.Add(pathTransforms[i]);
            }
        }
         
        for(int i = 0; i < points.Count; i++)
        {
            Vector3 currentPoint = points[i].position;
            Vector3 previousPoint = Vector3.zero;

            if (i > 0)
            {
                previousPoint = points[i - 1].position;
            }
            else if(i == 0 && points.Count > 1)
            {
                previousPoint = points[points.Count - 1].position;
            }

            Gizmos.DrawLine(previousPoint, currentPoint);
        }
    }

    public static object Combine(string dataPath, string v)
    {
        throw new NotImplementedException();
    }
}
