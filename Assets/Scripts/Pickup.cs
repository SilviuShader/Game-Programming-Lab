using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] 
    private float _rotationSpeed = 100.0f;

    public void Collect() => 
        Debug.Log("Pickup collected.");

    private void OnEnable() =>
        GameController.RegisterPickup(this);

    private void OnDisable() =>
        GameController.UnregisterPickup(this);

    private void Update() =>
        transform.Rotate(0.0f, 0.0f, -_rotationSpeed * Time.deltaTime);
}
