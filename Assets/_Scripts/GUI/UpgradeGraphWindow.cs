using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class UpgradeGraphWindow : EditorWindow
{
    private UpgradeGraphView graphView;
    private UpgradeNode rootNode;

    [MenuItem("Window/Upgrade Node Editor")]
    public static void Open()
    {
        UpgradeGraphWindow window = GetWindow<UpgradeGraphWindow>();
        window.titleContent = new GUIContent("Upgrade Node Editor");
    }

    private void OnEnable()
    {
        // Create the GraphView
        graphView = new UpgradeGraphView
        {
            name = "Upgrade Node Graph"
        };

        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);

        // Add controls
        var toolbar = new Toolbar();
        var nodeButton = new Button(() => AddRootNode()) { text = "Add Root Node" };
        toolbar.Add(nodeButton);

        rootVisualElement.Add(toolbar);
    }

    private void AddRootNode()
    {
        rootNode = CreateInstance<UpgradeNode>();
        rootNode.upgradeName = "Root Node";
        AssetDatabase.AddObjectToAsset(rootNode, "Assets/UpgradeNode.asset");
        AssetDatabase.SaveAssets();

        graphView.AddNode(rootNode);
    }
}

public class UpgradeGraphView : GraphView
{
    private Vector2 lastPosition = Vector2.zero;

    public UpgradeGraphView()
    {
        // Setup zoom and pan
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        // Add manipulation capabilities
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        // Add a grid background
        GridBackground grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        // Add style sheet
        styleSheets.Add(Resources.Load<StyleSheet>("Editor/UpgradeGraphStyle"));

        // Handle drag and drop
        RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
        RegisterCallback<DragPerformEvent>(OnDragPerform);
    }

    private void OnDragUpdated(DragUpdatedEvent evt)
    {
        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
    }

    private void OnDragPerform(DragPerformEvent evt)
    {
        DragAndDrop.AcceptDrag();

        foreach (Object draggedObject in DragAndDrop.objectReferences)
        {
            if (draggedObject is UpgradeNode upgradeNode)
            {
                AddNode(upgradeNode);
            }
        }
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        ports.ForEach(port =>
        {
            if (startPort != port && startPort.node != port.node)
                compatiblePorts.Add(port);
        });
        return compatiblePorts;
    }

    public void AddNode(UpgradeNode upgradeNode)
    {
        var node = new UpgradeNodeView(upgradeNode);
        node.SetPosition(new Rect(lastPosition + new Vector2(250,0), new Vector2(200, 150)));
        lastPosition += new Vector2(250, 0);
        AddElement(node);

        //node.na
        // Add children nodes
        if (upgradeNode.nextUpgrades != null)
        {
            //Debug.Log($"has {upgradeNode.nextUpgrades.Count} children");
            foreach (var childNode in upgradeNode.nextUpgrades)
            {
                AddNodeRecursive(childNode, node);
            }
        }
    }

    private void AddNodeRecursive(UpgradeNode upgradeNode, UpgradeNodeView parentNode)
    {
        var nodeView = new UpgradeNodeView(upgradeNode);
        nodeView.SetPosition(new Rect(parentNode.GetPosition().position + new Vector2(250, 0), new Vector2(200, 150)));
        AddElement(nodeView);

        var edge = parentNode.outputContainer.Q<Port>().ConnectTo(nodeView.inputContainer.Q<Port>());
        AddElement(edge);

        if (upgradeNode.nextUpgrades != null)
        {
            foreach (var childNode in upgradeNode.nextUpgrades)
            {
                AddNodeRecursive(childNode, nodeView);
            }
        }
    }
}

public class UpgradeNodeView : Node
{
    public UpgradeNode upgradeNode;

     public UpgradeNodeView(UpgradeNode nodeData)
    {
        this.upgradeNode = nodeData;
        title = nodeData.upgradeName;

        // Create UI elements for name, description, and price
        var nameLabel = new Label($"Name: {nodeData.upgradeName}");
        var descriptionLabel = new Label($"Description: {nodeData.upgradeDescription}");
        var priceLabel = new Label($"Price: {nodeData.price}");

        // Add these labels to the node
        contentContainer.Add(nameLabel);
        contentContainer.Add(descriptionLabel);
        contentContainer.Add(priceLabel);

        var inputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(float));
        inputPort.portName = "Input";
        inputContainer.Add(inputPort);

        var outputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(float));
        outputPort.portName = "Output";
        outputContainer.Add(outputPort);

        RefreshExpandedState();
        RefreshPorts();
    }
}
