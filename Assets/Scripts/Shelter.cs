using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Shelter : MonoBehaviour
{
    [SerializeField] private float distance = 0.75f;
    public List<Vector3> Positions { get; private set; }

    private int currentIndex = 0;


    private static Shelter instance;
    public static Shelter Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Shelter>();
            }
            return instance;
        }
    }

    void Awake()
    {
        Positions = new List<Vector3>();

        BoxCollider collider = GetComponent<BoxCollider>();
        Vector3 min = collider.bounds.min;
        Vector3 max = collider.bounds.max;

        for (float x = min.x; x <= max.x; x += distance)
        {
            for (float z = min.z; z <= max.z; z += distance)
            {
                Positions.Add(new Vector3(x, collider.bounds.extents.y, z));
            }
        }
    }
    public Vector3 GetPosition()
    {
        currentIndex++;
        return Positions[currentIndex];
    }
    void OnDrawGizmos()
    {

        BoxCollider collider = GetComponent<BoxCollider>();

        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);

        Vector3 min = collider.bounds.min;
        Vector3 max = collider.bounds.max;
        float interval = 0.75f;

        Gizmos.color = Color.red;

        for (float x = min.x; x <= max.x; x += interval)
        {
            for (float z = min.z; z <= max.z; z += interval)
            {
                Gizmos.DrawSphere(new Vector3(x, collider.bounds.extents.y, z), 0.1f); // Adjust the radius as needed
            }
        }

    }
}
