using UnityEngine;

public class InversePickup : Pickup
{
    [SerializeField]
    private Vector3 _rotationSpeed = new(20.0f, 30.0f, 50.0f);

    public override void Collect() =>
        GameController.Instance.InvertControls = true;

    private void Update() =>
        transform.Rotate(_rotationSpeed * Time.deltaTime);
}
