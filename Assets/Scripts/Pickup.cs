using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Pickup : MonoBehaviour
{
    public virtual void Collect() { }

    private void Awake() => 
        GetComponent<Collider>().isTrigger = true;

    private void OnEnable() =>
        GameController.RegisterPickup(this);

    private void OnDisable() =>
        GameController.UnregisterPickup(this);
}
