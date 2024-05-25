using UnityEngine;
using UnityEngine.SceneManagement;

public class WaterPhysics : MonoBehaviour
{
    [SerializeField] private float floatUpSpeedLimit = 1.15f;
    [SerializeField] private float floatUpSpeed = 1f;
    [SerializeField] private float MaxTime = 8;

    private Rigidbody _rb;
    private CarEffect _carEffect;

    private float _timer;

    void Start()
    {
        _rb= GetComponent<Rigidbody>();
        _carEffect= GetComponent<CarEffect>();
        _timer = 0;
    }

    private void FixedUpdate()
    {
        if (_timer >= MaxTime)
        {
            SceneManager.LoadScene(1);
            _timer = 0;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 4)
        {
            float difference = (other.transform.position.y - transform.position.y) * floatUpSpeed;
            _rb.AddForce(new Vector3(0f, Mathf.Clamp((Mathf.Abs(Physics.gravity.y) * difference), 0, Mathf.Abs(Physics.gravity.y) * floatUpSpeedLimit), 0f), ForceMode.Acceleration);
            _rb.drag = 0.99f;
            _rb.angularDrag = 0.8f;
            _timer += Time.deltaTime*1.5f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 4)
        {
            _rb.drag = _carEffect.Drag;
            _rb.angularDrag = 0.05f;
           
        }
    }
}
