using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : MonoBehaviour
{
    [SerializeField] private Vector3 sittingOffset;
    [SerializeField] private Vector3 mugOffset;
    [SerializeField] private Vector3 tipOffset;
    [SerializeField] private Mug mugPrefab;

    public float BookingTime { get; private set; }
    public bool IsAcquired { get; private set; }

    public bool HasMug => spawnedMug != null;
    private Mug spawnedMug;

    public Vector3 MugSpawnPoint
    {
        get
        {
            return transform.TransformPoint(mugOffset);
        }
    }
    public Vector3 CostumerSitPosition
    {
        get
        {
            return transform.TransformPoint(sittingOffset);
        }
    }
    public Vector3 TipCoinsPosition
    {
        get
        {
            return transform.TransformPoint(tipOffset);
        }
    }
    public void Acquire()
    {
        IsAcquired = true;
        BookingTime = Time.time;
    }
    public void Release()
    {
        IsAcquired = false;
    }
    public void ServeMug()
    {
        Destroy(spawnedMug.gameObject);
        spawnedMug = null;
    }
    public void PlaceMug(Player player)
    {
        spawnedMug = Instantiate(mugPrefab, MugSpawnPoint, Quaternion.identity, transform);
        player.Mug.Serve();
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(MugSpawnPoint, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(CostumerSitPosition, 0.1f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(TipCoinsPosition, 0.1f);
    }
}
