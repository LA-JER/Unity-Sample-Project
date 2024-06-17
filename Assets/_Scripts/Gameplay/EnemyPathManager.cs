using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathManager : MonoBehaviour
{
    public static EnemyPathManager Instance;

    [SerializeField] Transform map;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private Color lineColor;
    [SerializeField] private float lineWidth = 0.1f;
    [SerializeField] private List<Transform> waypoints = new List<Transform>();
    [SerializeField] private string colliderTag = "Path";
    [SerializeField] private float colliderWidth = 10;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DrawLines();
        AddColliders();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DrawLines()
    {
        if (waypoints.Count < 2)
        {
            Debugger.Log(Debugger.AlertType.Warning ,$"{name} needs at least two waypoints to create a line.");
            return;
        }
        if(lineMaterial != null)
        {
            // Iterate through the points list and create line renderers
            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                GameObject lineObject = new GameObject("Line_" + waypoints[i].name + "_" + waypoints[i+1].name);
                LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
                if (lineRenderer != null)
                {
                    // Configure the line renderer
                    lineRenderer.material = lineMaterial;
                    lineRenderer.startColor = lineColor; 
                    lineRenderer.endColor = lineColor;
                    lineRenderer.widthMultiplier = lineWidth;
                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPosition(0, waypoints[i].position);
                    lineRenderer.SetPosition(1, waypoints[i + 1].position);
                }

                if(map != null)
                {
                    lineObject.transform.SetParent(map, true);
                }
            }
        } else
        {
            Debugger.Log(Debugger.AlertType.Warning, $"{name} did not have a line meterial!");
        }
    }

    void AddColliders()
    {
        if (waypoints.Count < 2)
        {
            Debugger.Log(Debugger.AlertType.Warning, $"{name} needs at least two waypoints to create colliders.");
            return;
        }

        // Iterate through the points list and create box colliders
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            CreateBoxColliderBetween(waypoints[i], waypoints[i + 1]);
            if(i >0 &&  i < waypoints.Count - 1)
            {
                CreateCirlceColliderAt(waypoints[i]);
            }
        }
    }

    void CreateBoxColliderBetween(Transform pointA, Transform pointB)
    {
        // Create a new GameObject for the collider
        GameObject colliderObject = new GameObject("BoxCollider2D_" + pointA.name + "_" + pointB.name);
        BoxCollider2D boxCollider = colliderObject.AddComponent<BoxCollider2D>();

        // Calculate the position and size of the collider
        Vector3 midPoint = (pointA.position + pointB.position) / 2;
        float distance = Vector3.Distance(pointA.position, pointB.position);

        // Set the collider's position
        colliderObject.transform.position = midPoint;

        // Calculate the angle between the points and set the rotation
        Vector3 direction = pointB.position - pointA.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        colliderObject.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Set the size of the collider
        boxCollider.size = new Vector2(distance, colliderWidth); 
        boxCollider.isTrigger = true;
        colliderObject.tag = colliderTag;

        if (map != null)
        {
            colliderObject.transform.SetParent(map, false);
        }
    }

    void CreateCirlceColliderAt(Transform point)
    {
        GameObject colliderObject = new GameObject("CircleCollider2D_" + point.name);
        CircleCollider2D circleCollider2D = colliderObject.AddComponent<CircleCollider2D>();

        colliderObject.transform.position = point.position;

        float radius = colliderWidth / 2f;

        circleCollider2D.isTrigger = true;
        circleCollider2D.radius = radius;
        circleCollider2D.tag = colliderTag;

        if (map != null)
        {
            colliderObject.transform.SetParent(map, false);
        }
    }

    public List<Transform> GetWayPoints()
    {
        List<Transform> list = new List<Transform>();
        foreach(Transform t in waypoints)
        {
            list.Add(t);
        }

        return list;
    }
}
