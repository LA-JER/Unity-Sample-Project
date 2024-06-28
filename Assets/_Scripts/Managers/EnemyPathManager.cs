using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathManager : MonoBehaviour
{
    public static EnemyPathManager Instance;

    [SerializeField] Transform map;
    [SerializeField] private GameObject circlePrefab;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private Color lineColor;
    [SerializeField] private float lineWidth = 0.1f;
    [SerializeField] private Vector2 randomPathOffset = new Vector2(.25f, .25f);
    [SerializedDictionary("Start Node", "Spawn Weight")]
    [SerializeField] public SerializedDictionary<PathNode, int> startNodes = new SerializedDictionary<PathNode, int>();
    [SerializeField] private string colliderTag = "Path";
    [SerializeField] private float colliderWidth = 10;


    private List<Line> lines = new List<Line>();
    private List<Transform> waypoints = new List<Transform>();
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
        Initialize();
       // DrawLines();
        //AddColliders();
    }


    void Initialize()
    {
        if(startNodes == null || startNodes.Count == 0) 
        {
            Debugger.Log(Debugger.AlertType.Error, "Waypoint Manager did not have start nodes!");
            return;
        }

        foreach(var node in startNodes)
        {
            TraverseTree(node.Key, null);
        }
    }

    //Initializes all things that must be done through traversing the tree
    void TraverseTree(PathNode node, Transform lastNodeTransform)
    {
        if (node == null || node.childNodes == null) return;

        //get a single reference to each node's transform
        if (!waypoints.Contains(node.location))
        {
            waypoints.Add(node.location);
        }

        //Draw the lines and adds the colliders to designate what is the enemy path
        if(lastNodeTransform != null)
        {
            DrawLine(lastNodeTransform.position, node.location.position);
            AddCollider(lastNodeTransform.position, node.location.position);
        }

        DrawCircle(node.location.position);


        //get a list of the children 
        List<PathNode> children = new List<PathNode>();
        foreach(var child in node.childNodes)
        {
            children.Add(child.Key);
        }

        foreach(var child in children)
        {
            TraverseTree(child, node.location);
        }

    }

    private void DrawLine(Vector2 pointA, Vector2 pointB)
    {
        if (lineMaterial == null)
        {
            Debugger.Log(Debugger.AlertType.Warning, $"{name} did not have a line meterial!");
            return;
        }

        if (pointA == null || pointB == null) return;

        
        GameObject lineObject = new GameObject("Line_" + pointA + "_" + pointB);
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            // Configure the line renderer
            lineRenderer.material = lineMaterial;
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
            lineRenderer.widthMultiplier = lineWidth;
            lineRenderer.positionCount = 2;
            lineRenderer.sortingOrder = -2;
            lineRenderer.SetPosition(0, pointA);
            lineRenderer.SetPosition(1, pointB);
        }

        if (map != null)
        {
            lineObject.transform.SetParent(map, true);
        }
        
    }

    private void DrawCircle(Vector2 center)
    {
        if (center == null) return;
        GameObject circleOBJ = Instantiate(circlePrefab, map);
        circleOBJ.transform.position = center;

        circleOBJ.transform.localScale = new Vector2(lineWidth, lineWidth);
        SpriteRenderer spriteRenderer = circleOBJ.GetComponent<SpriteRenderer>();
        if(spriteRenderer != null)
        {
            spriteRenderer.color = lineColor;
        }

    }


    void AddCollider(Vector2 positionA, Vector2 positionB)
    {
        if (positionA == null || positionB == null) return;

        
        CreateBoxColliderBetween(positionA, positionB);
       
            CreateCirlceColliderAt(positionB);
        
        
    }

    void CreateBoxColliderBetween(Vector2 pointA, Vector2 pointB)
    {
        // Create a new GameObject for the collider
        GameObject colliderObject = new GameObject("BoxCollider2D_" + pointA + "_" + pointB);
        BoxCollider2D boxCollider = colliderObject.AddComponent<BoxCollider2D>();

        // Calculate the position and size of the collider
        Vector3 midPoint = (pointA + pointB) / 2;
        float distance = Vector3.Distance(pointA, pointB);

        // Set the collider's position
        colliderObject.transform.position = midPoint;

        // Calculate the angle between the points and set the rotation
        Vector3 direction = pointB - pointA;
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

    void CreateCirlceColliderAt(Vector2 point)
    {
        GameObject colliderObject = new GameObject("CircleCollider2D_" + point);
        CircleCollider2D circleCollider2D = colliderObject.AddComponent<CircleCollider2D>();

        colliderObject.transform.position = point;

        float radius = colliderWidth / 2f;

        circleCollider2D.isTrigger = true;
        circleCollider2D.radius = radius;
        circleCollider2D.tag = colliderTag;

        if (map != null)
        {
            colliderObject.transform.SetParent(map, false);
        }
    }

    public List<Vector3> Getpath(bool randomOffset = true)
    {
        float xOffset = Random.Range(-randomPathOffset.x, randomPathOffset.x);
        float yOffset = Random.Range(-randomPathOffset.y, randomPathOffset.y);
        Vector3 offset = new Vector3(xOffset, yOffset);

        //get random starting node based on weights
        PathNode nextNode = GetRandomSpawnNode();

        List < Vector3 > list = new List<Vector3>();
        while (nextNode != null){
            list.Add(nextNode.location.position + offset);
            nextNode = ChooseWeightedNode(nextNode);
        }


        return list;
    }

    PathNode ChooseWeightedNode(PathNode node)
    {
        if (node == null || node.childNodes == null ) return null;

        List<PathNode> random = RandomSelect<PathNode>.ChooseRandomObjects(node.childNodes, 1);
        if(random == null) return null;
        return random[0];
    }

    private PathNode GetRandomSpawnNode()
    {
        List<PathNode> randomStart = RandomSelect<PathNode>.ChooseRandomObjects(startNodes, 1);
        if (randomStart == null || randomStart.Count == 0)
        {
            Debugger.Log(Debugger.AlertType.Warning, "Could not choose random starting node for enemy paths!");
            return null;
        }
        return randomStart[0];
    }

    public Transform GetRandomSpawn()
    {
        return GetRandomSpawnNode().location;
    }

    public List<Transform> GetClosestWayPoints(Vector2 position)
    {
        if (waypoints == null || waypoints.Count == 0)
        {
            return null;  // Handle empty list case
        }

        //find the next transform
        Transform closestTransform = waypoints[0];
        int closestIndex = waypoints.Count;
        float closestDistanceSqr = Mathf.Infinity;

        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] == null)
            {
                continue;  // Skip null transforms in the list
            }

            // Calculate distance squared (avoids unnecessary square root)
            float distanceSqr = Vector2.SqrMagnitude(position - (Vector2)waypoints[i].position);  // Cast to Vector2

            if (waypoints[i].position.x > position.x && distanceSqr < closestDistanceSqr)
            {
                closestIndex = i;
                closestDistanceSqr = distanceSqr;
                closestTransform = waypoints[i];
            }
        }
        //Debugger.Log(Debugger.AlertType.Info, $"Next closest tranform to {position} is {closestTransform.position}");

        List<Transform> list = new List<Transform>();
        for (int i = closestIndex; i < waypoints.Count; i++)
        {
            if (waypoints[i] == null) continue;

            list.Add(waypoints[i]);   
        }


        return list;
    }
}

public class Line
{
    Vector2 positionA;
    Vector2 positionB;
}
