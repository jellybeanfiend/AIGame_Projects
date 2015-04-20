// Sevena Skeels
// Homework 2
// CAP4053

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AIClass
{
    public class PathGraph
    {
        public HashSet<Node> nodes;
        public Dictionary<Vector2, Node> vectorToNode;
        List<Vector2> list = new List<Vector2>();
        public int width;
        public int height;

        public PathGraph(int width, int height)
        {
            this.width = width;
            this.height = height;
            nodes = new HashSet<Node>();
            vectorToNode = new Dictionary<Vector2, Node>();
            setup();
        }

        public void setup()
        {
            Vector2 startPos = new Vector2(width / 2, height / 2);
            generateNodes(getNodeFromVector(startPos), width, height);
        }

        public void Clear()
        {
            nodes.Clear();
            vectorToNode.Clear();
        }

        public bool generateNodes(Node node, int width, int height)
        {

            if (isOutOfBounds(node.position, width, height))
                return false;

            if (intersectsWalls(node.position))
                return false;

            if(list.Contains(node.position))
                return true;
            list.Add(node.position);

            node.giveId();
            nodes.Add(node);

            Vector2[] adjacent = {new Vector2(node.position.X - 50, node.position.Y),
                                  new Vector2(node.position.X + 50, node.position.Y),
                                  new Vector2(node.position.X, node.position.Y - 50),
                                  new Vector2(node.position.X, node.position.Y + 50),
                                  new Vector2(node.position.X - 50, node.position.Y + 50),
                                  new Vector2(node.position.X + 50, node.position.Y + 50),
                                  new Vector2(node.position.X - 50, node.position.Y - 50),
                                  new Vector2(node.position.X + 50, node.position.Y - 50)
                                  };

            foreach (Vector2 v in adjacent)
            {
                Node n = getNodeFromVector(v);
                if (generateNodes(n, width, height))
                {
                    if(validEdge(node, n))
                        node.neighbors.Add(n);
                }
            }

            return true;
        }

        // It's an A* search, mmmkay?
        public List<Node> getShortestPath(Node start, Node target)
        {

            // Set of nodes that have been expanded by the search
            HashSet<Node> visited = new HashSet<Node>();
            // Nodes to be expanded - sorted by path cost + heuristic cost
            Heap<Node> frontier = new Heap<Node>();

            initializeNodes();

            // Initialize search with start node
            start.updatePathCost(null, 0);
            start.calculateHeuristic(target.position);
            frontier.insert(start);

            while (!frontier.isEmpty())
            {
                // Get node that has the smallest total cost
                Node current = frontier.remove();

                if (current.position == target.position)
                    return reconstructPath(current);

                visited.Add(current);

                foreach (Node neighbor in current.neighbors)
                {
                    if (visited.Contains(neighbor))
                        continue;

                    float pathCostToNeighbor = current.pathCost + current.getEdgeCost(neighbor);
                    neighbor.updatePathCost(current, pathCostToNeighbor);

                    if (neighbor.addToFrontier())
                    {
                        neighbor.calculateHeuristic(target.position);
                        frontier.insert(neighbor);
                    }

                }
            }

            return null;
        }

        public bool validEdge(Node src, Node target)
        {

            int offset = (int)(13 * .707);
            Vector3 src1 = new Vector3(src.position.X - offset, src.position.Y - offset, 0);
            Vector3 src2 = new Vector3(src.position.X + offset, src.position.Y + offset, 0);
            Vector3 dir = Vector3.Normalize(new Vector3(target.position - src.position, 0));

            Ray leftBound = new Ray(src1, dir);
            Ray rightBound = new Ray(src2, dir);

            double distance = Vector2.Distance(target.position, src.position);

            foreach (BoundingBox wall in Program.walls)
            {
                Nullable<float> leftIntersect = leftBound.Intersects(wall);
                Nullable<float> rightIntersect = rightBound.Intersects(wall);

                if (leftIntersect <= distance || rightIntersect <= distance)
                    return false;
            }
            return true;

        }

        #region private functions
        private bool isOutOfBounds(Vector2 point, int width, int height)
        {
            return (point.X - 12 < 0 || point.X + 12 > width || point.Y - 12 < 0 || point.Y + 12 > height);
        }

        private bool intersectsWalls(Vector2 point)
        {
            BoundingBox s = new BoundingBox(new Vector3(point.X - 12, point.Y - 12, 0), new Vector3(point.X + 12, point.Y + 12, 0));

            foreach (BoundingBox wall in Program.walls)
            {
                if (s.Intersects(wall))
                {
                    return true;
                }
            }
            return false;
        }

        private void initializeNodes()
        {
            foreach (Node node in nodes)
            {
                node.clear();
            }
        }

        private List<Node> reconstructPath(Node last)
        {
            List<Node> path = new List<Node>();

            while (last != null)
            {
                path.Add(last);
                last = last.parent;
            }
            return path;
        }

        private Node getNodeFromVector(Vector2 v)
        {
            if (!vectorToNode.ContainsKey(v))
                vectorToNode[v] = new Node(v);    
            return vectorToNode[v];
        }

        #endregion
    }
}
