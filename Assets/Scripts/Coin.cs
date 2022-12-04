using UnityEngine;

public class Coin : Pickup
{
    public int    Increment      = 1;

    [SerializeField]
    private float _rotationSpeed = 100.0f;

    public override void Collect() => 
        GameController.Instance.Score += Increment;

    private void Update() =>
        transform.Rotate(0.0f, 0.0f, -_rotationSpeed * Time.deltaTime);
}
