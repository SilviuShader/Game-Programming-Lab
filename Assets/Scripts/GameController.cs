using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private        Pickup       _pickupPrefab       = default;
    [SerializeField]                                
    private        BoxCollider  _floor              = default;
    [SerializeField]                                
    private        float        _pickupRadius       = 0.5f;

    private static List<Pickup> InstantiatedPickups = new();

    public static void RegisterPickup(Pickup pickup)
    {
        Debug.Assert(!InstantiatedPickups.Contains(pickup),
            "Duplicate registration of pickup!", pickup);

        InstantiatedPickups.Add(pickup);
    }

    public static void UnregisterPickup(Pickup pickup)
    {
        Debug.Assert(
            InstantiatedPickups.Contains(pickup),
            "Unregistration of unknown pickup!", pickup);

        InstantiatedPickups.Remove(pickup);
    }

    private void Update()
    {
        if (InstantiatedPickups.Count < 1)
            SpawnPickup(_pickupPrefab);
    }

    private void SpawnPickup(Pickup prefab)
    {
        var pickup = Instantiate(prefab);
        var bounds = _floor.bounds;

        bounds.size -= new Vector3(_pickupRadius, 0.0f, _pickupRadius) * 2.0f;

        var position = new Vector3(
            Mathf.Lerp(bounds.min.x, bounds.max.x, Random.value),
            bounds.max.y + _pickupRadius,
            Mathf.Lerp(bounds.min.z, bounds.max.z, Random.value)
        );

        pickup.transform.position = position;
    }
}
