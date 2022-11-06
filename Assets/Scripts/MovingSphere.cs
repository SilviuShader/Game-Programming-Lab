using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class MovingSphere : MonoBehaviour
{
    [SerializeField, Range(10.0f, 100.0f)]
    private float     _maxSpeed        = 10.0f;
    [SerializeField, Range(10.0f, 100.0f)]
    private float     _maxAcceleration = 10.0f;

    private Vector2   _movement;
    private Vector3   _velocity        = Vector3.zero,
                      _desiredVelocity = Vector3.zero;
    private Rigidbody _rigidbody;

    public void Movement(InputAction.CallbackContext context) => 
        _movement = context.ReadValue<Vector2>();

    private void Awake() => 
        _rigidbody = GetComponent<Rigidbody>();

    private void OnTriggerEnter(Collider collider)
    {
        var other = collider.gameObject;
     
        if (other.CompareTag("Pickup"))
        {
            var pickup = other.GetComponent<Pickup>();
            pickup.Collect();
            Destroy(other);
        }
    }

    private void Update()
    {
        var sign = GameController.Instance.InvertControls ? -1.0f : 1.0f;
        _desiredVelocity = new Vector3(_movement.x, 0.0f, _movement.y) *
                           sign * _maxSpeed;
    }

    private void FixedUpdate()
    {
        _velocity = _rigidbody.velocity;

        var maxSpeedChange = _maxAcceleration * Time.deltaTime;

        _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x,
            maxSpeedChange);
        _velocity.z = Mathf.MoveTowards(_velocity.z, _desiredVelocity.z,
            maxSpeedChange);

        _rigidbody.velocity = _velocity;
    }
}