using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace KasperDev.Dialogue.Editor
{
    public class DialogueGraphView : GraphView
    {
        private string graphViewStyleSheet = "USS/GraphView/GraphViewStyleSheet"; // Name of the graph view style sheet.
        private DialogueEditorWindow editorWindow;
        private NodeSearchWindow searchWindow;

        // Amount to pan when arrow keys are pressed
        private const float PanAmount = 50f;

        public DialogueGraphView(DialogueEditorWindow editorWindow)
        {
            this.editorWindow = editorWindow;

            // Adding the ability to zoom in and out graph view.
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            // Adding style sheet to graph view.
            StyleSheet tmpStyleSheet = Resources.Load<StyleSheet>(graphViewStyleSheet);
            styleSheets.Add(tmpStyleSheet);

            // Manipulators for interaction.
            this.AddManipulator(new ContentDragger());      // The ability to drag nodes around.
            this.AddManipulator(new SelectionDragger());    // The ability to drag all selected nodes around.
            this.AddManipulator(new RectangleSelector());   // The ability to drag select a rectangle area.
            this.AddManipulator(new FreehandSelector());    // The ability to select a single node.

            // Add a visible grid to the background.
            GridBackground grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            // Set up scrolling capability.
            SetupScrollAndPan();

            // Add arrow key panning functionality.
            RegisterArrowKeyNavigation();

            AddSearchWindow();
        }

        /// <summary>
        /// Adds scrolling and panning capabilities to the graph view.
        /// </summary>
        private void SetupScrollAndPan()
        {
            // Setting the appropriate styles to ensure the content is scrollable.
            this.style.flexGrow = 1;
            this.StretchToParentSize();

            // Set overflow to Visible to ensure that it doesn't clip the content,
            // thus allowing the user to pan beyond the visible bounds.
            this.contentContainer.style.overflow = Overflow.Visible; // Ensure the overflow is visible.

            // Adjusting the content container to allow for panning and dragging beyond its bounds.
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
        }

        /// <summary>
        /// Register arrow keys for navigation.
        /// </summary>
        private void RegisterArrowKeyNavigation()
        {
            this.RegisterCallback<KeyDownEvent>(evt =>
            {
                switch (evt.keyCode)
                {
                    case KeyCode.LeftArrow:
                        // Pan left
                        this.viewTransform.position += new Vector3(PanAmount, 0, 0);
                        evt.StopPropagation();
                        break;
                    case KeyCode.RightArrow:
                        // Pan right
                        this.viewTransform.position -= new Vector3(PanAmount, 0, 0);
                        evt.StopPropagation();
                        break;
                    case KeyCode.UpArrow:
                        // Pan up
                        this.viewTransform.position += new Vector3(0, PanAmount, 0);
                        evt.StopPropagation();
                        break;
                    case KeyCode.DownArrow:
                        // Pan down
                        this.viewTransform.position -= new Vector3(0, PanAmount, 0);
                        evt.StopPropagation();
                        break;
                }
            });
        }

        /// <summary>
        /// Add a search window to graph view.
        /// </summary>
        private void AddSearchWindow()
        {
            searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            searchWindow.Configure(editorWindow, this);
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
        }

        // This is a graph view method that we override.
        // This is where we tell the graph view which nodes can connect to each other.
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>(); // All the ports that can be connected to.
            Port startPortView = startPort; // Start port.

            ports.ForEach((port) =>
            {
                Port portView = port;

                // First we tell that it cannot connect to itself.
                // Then we tell it cannot connect to a port on the same node.
                // Lastly we tell it that an input node cannot connect to another input node and an output node cannot connect to an output node.
                if (startPortView != portView && startPortView.node != portView.node && startPortView.direction != port.direction && startPortView.portColor == portView.portColor)
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts; // return all the acceptable ports.
        }

        /// <summary>
        /// Reload the current selected language.
        /// Normally used when changing language.
        /// </summary>
        public void ReloadLanguage()
        {
            List<BaseNode> allNodes = nodes.ToList().Where(node => node is BaseNode).Cast<BaseNode>().ToList();
            foreach (BaseNode node in allNodes)
            {
                node.ReloadLanguage();
            }
        }

        // Make Node's -----------------------------------------------------------------------------------

        /// <summary>
        /// Make new Start Node and set its position.
        /// </summary>
        public StartNode CreateStartNode(Vector2 position)
        {
            return new StartNode(position, editorWindow, this);
        }

        /// <summary>
        /// Make new End Node and set its position.
        /// </summary>
        public EndNode CreateEndNode(Vector2 position)
        {
            return new EndNode(position, editorWindow, this);
        }

        /// <summary>
        /// Make new Event Node and set its position.
        /// </summary>
        public EventNode CreateEventNode(Vector2 position)
        {
            return new EventNode(position, editorWindow, this);
        }

        /// <summary>
        /// Make new Dialogue Node and set its position.
        /// </summary>
        public DialogueNode CreateDialogueNode(Vector2 position)
        {
            return new DialogueNode(position, editorWindow, this);
        }

        /// <summary>
        /// Make new Branch Node and set its position.
        /// </summary>
        public BranchNode CreateBranchNode(Vector2 position)
        {
            return new BranchNode(position, editorWindow, this);
        }

        /// <summary>
        /// Make new Choice Node and set its position.
        /// </summary>
        public ChoiceNode CreateChoiceNode(Vector2 position)
        {
            return new ChoiceNode(position, editorWindow, this);
        }
    }
}
