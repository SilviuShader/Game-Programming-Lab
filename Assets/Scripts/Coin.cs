using UnityEngine;

public class Coin : Pickup
{
    [SerializeField]
    private float _rotationSpeed = 100.0f;
    [SerializeField]
    private int   _increment     = 1;

    public override void Collect() => 
        GameController.Instance.Score += _increment;

    private void Update() =>
        transform.Rotate(0.0f, 0.0f, -_rotationSpeed * Time.deltaTime);
}
