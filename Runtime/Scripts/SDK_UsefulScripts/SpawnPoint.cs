using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private readonly List<Collider> collidingObjs = new();

    public bool IsFree => collidingObjs.Count == 0;

    private void OnTriggerEnter(Collider other)
    {
        collidingObjs.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        collidingObjs.Remove(other);
    }
}

