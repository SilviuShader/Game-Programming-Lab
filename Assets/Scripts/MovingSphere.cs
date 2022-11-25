using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class MovingSphere : MonoBehaviour
{
    private const float     _maxDistance     = 100.0f;

    [SerializeField, Range(10.0f, 100.0f)]
    private       float     _maxSpeed        = 10.0f;
    [SerializeField, Range(10.0f, 100.0f)]
    private       float     _maxAcceleration = 10.0f;
    [SerializeField]
    private       Camera    _mainCamera      = default;
    [SerializeField]
    private       LayerMask _layerMask       = -1;
                  
    private       Vector2   _movement;
    private       Vector3   _velocity        = Vector3.zero,
                            _desiredVelocity = Vector3.zero;
    private       Rigidbody _rigidbody;

    private       bool      _manualMovement;
    private       Coroutine _movementCoroutine;

    public void Movement(InputAction.CallbackContext context)
    {
        _manualMovement = true;
        _movement = context.ReadValue<Vector2>();
    }

    public void FloorClick(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;

        Vector3 mousePosition = Mouse.current.position.ReadValue();
        var ray = _mainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, _maxDistance, _layerMask))
        {
            if (_movementCoroutine != null)
                StopCoroutine(_movementCoroutine);

            _movementCoroutine = StartCoroutine(GoToPosition(hitInfo.point));
        }
    }

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
        if (!GameController.Instance.GameRunning)
        {
            _movement = Vector2.zero;
            if (_movementCoroutine != null)
            {
                StopCoroutine(_movementCoroutine);
                _movementCoroutine = null;
            }
        }

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

    private IEnumerator GoToPosition(Vector3 targetPosition)
    {
        var initialPosition = transform.position;
        var initialVector = targetPosition - initialPosition;
        initialVector.y = 0.0f;

        var reachedTarget = false;
        _manualMovement = false;

        while (!reachedTarget && !_manualMovement &&
               !GameController.Instance.InvertControls)
        {
            var position = transform.position;
            var direction = targetPosition - position;
            direction.y = 0.0f;
            direction.Normalize();

            _movement = new Vector2(direction.x, direction.z);
            yield return new WaitForEndOfFrame();

            var currentVector = position - initialPosition;
            reachedTarget = currentVector.magnitude > initialVector.magnitude;
        }

        if (!_manualMovement)
            _movement = Vector2.zero;

        _movementCoroutine = null;
    }
}