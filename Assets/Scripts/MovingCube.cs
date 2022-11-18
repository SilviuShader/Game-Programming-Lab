using UnityEngine;
using UnityEngine.InputSystem;

public class MovingCube : MonoBehaviour
{
    private enum PlayerState
    {
        Idle,
        Rotating,
        Moving
    }

    private const float       _maxDistance     = 100.0f;
    private const float       _bias            = 0.01f;
                                               
    [SerializeField]                           
    private       Camera      _mainCamera;     
    [SerializeField]                           
    private       LayerMask   _layerMask       = -1;
    [SerializeField]                           
    private       float       _rotationSpeed   = 60.0f;
    [SerializeField]
    private       float       _movementSpeed   = 10.0f;
                                               
    private       Quaternion  _targetRotation  = Quaternion.identity;
    private       Vector3     _targetPosition  = Vector3.zero;
    private       Vector3     _initialPosition = Vector3.zero;
    private       PlayerState _playerState     = PlayerState.Idle;
    
    public void FloorClick(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;

        Vector3 mousePosition = Mouse.current.position.ReadValue();
        var ray = _mainCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out var hitInfo, _maxDistance, _layerMask))
        {
            var point = hitInfo.point;
            point.y = transform.position.y;

            var toPoint = point - transform.position;

            _targetRotation = Quaternion.LookRotation(toPoint, transform.up);
            _targetPosition = point;
            _initialPosition = transform.position;

            Debug.Log(_targetPosition + " " + _initialPosition);

            _playerState = PlayerState.Rotating;
        }
    }

    private void Update()
    {
        switch (_playerState)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Rotating:
                transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation,
                    _rotationSpeed * Time.deltaTime);

                var targetDirection = _targetRotation * Vector3.forward;
                var deltaAngle = Vector3.SignedAngle(transform.forward, targetDirection, 
                    transform.up);

                if (Mathf.Abs(deltaAngle) <= _bias)
                    _playerState = PlayerState.Moving;
                
                break;
            case PlayerState.Moving:

                var direction = (_targetPosition - _initialPosition).normalized;
                transform.position += direction * _movementSpeed * Time.deltaTime;
                
                var toCurrentPosition = transform.position - _initialPosition;
                var toTarget = _targetPosition - _initialPosition;

                if (toCurrentPosition.magnitude > toTarget.magnitude)
                    _playerState = PlayerState.Idle;

                break;
        }
    }
}
