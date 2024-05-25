using System.Collections;
using UnityEngine;

public class DamageControl : MonoBehaviour
{
    [SerializeField]
    private float _maxMoveVertex = 1.0f;

    [SerializeField]
    private float _maxCollisionPower = 50.0f;

    [SerializeField]
    private float _maxSpeedY = 0.5f;

    [SerializeField]
    private float _destructionRange = 1f;

    [SerializeField]
    private float _impactManipulator = 0.5f;

    [SerializeField]
    private MeshFilter[] _optionalMesh;
    private MeshFilter[] _meshfilters;
    private float _sqrDestructionRange;

    private Vector3 _collisionPoint;
    private float _collisionPower;

    public void Start()
    {
        if (_optionalMesh.Length > 0)
        { _meshfilters = _optionalMesh; }
        else
        { _meshfilters = GetComponentsInChildren<MeshFilter>(); }

        _sqrDestructionRange = _destructionRange * _destructionRange;
    }

    private void OnMeshDeform(Vector3 originPosition,float force)
    {
        force = Mathf.Clamp01(force);

        for (int i = 0;  i < _meshfilters.Length; i++)
        {
            Vector3[] vertices = _meshfilters[i].mesh.vertices;

            for (int j = 0; j < vertices.Length; j++)
            {
                Vector3 scaledVertex = Vector3.Scale(vertices[j], transform.localScale);
                Vector3 vertexWorldposition = _meshfilters[i].transform.position + (_meshfilters[i].transform.rotation * scaledVertex);
                Vector3 vertexOriginToMe = vertexWorldposition - originPosition;
                Vector3 vertexToCenter = transform.position - vertexWorldposition;
                vertexToCenter.y = 0.0f;

                if (vertexOriginToMe.sqrMagnitude < _sqrDestructionRange)
                {
                    float distance = Mathf.Clamp01(vertexOriginToMe.sqrMagnitude / _sqrDestructionRange);
                    float moveDelta = force * (1.0f - distance) * _maxMoveVertex;
                    Vector3 move = Vector3.Slerp(vertexOriginToMe, vertexToCenter, _impactManipulator).normalized * moveDelta;
                    vertices[j] += Quaternion.Inverse(transform.rotation) * move;
                }
            }

            _meshfilters[i].mesh.vertices = vertices;
            _meshfilters[i].mesh.RecalculateBounds();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 collisionRelVel = collision.relativeVelocity;
        collisionRelVel.y *= _maxSpeedY;

        if (collision.contacts.Length > 0)
        {
            _collisionPoint = transform.position - collision.contacts[0].point;
            _collisionPower = collisionRelVel.magnitude * Vector3.Dot(collision.contacts[0].normal, _collisionPoint.normalized);
            

            if (_collisionPoint.magnitude > 1)
            {
                OnMeshDeform(collision.contacts[0].point, Mathf.Clamp01(_collisionPower / _maxCollisionPower));
            }
        }      
    }
}