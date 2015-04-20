// Sevena Skeels
// Homework 2
// CAP4053

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AIClass
{
    public class Node : IComparable<Node>
    {
        private static int counter = 0;
        public int id = 0;

        public bool inFrontier;
        public Vector2 position;
        public Node parent;
        public List<Node> neighbors;
        public float pathCost;
        public float heuristicCost;
        public Rectangle bound;

        public Node(Vector2 position)
        {
            this.position = position;
            neighbors = new List<Node>(8);
            bound = new Rectangle((int)position.X - 6, (int)position.Y - 6, 12, 12); 
            clear();
        }

        public void giveId()
        {
            id = System.Threading.Interlocked.Increment(ref counter);
        }

        public void clear()
        {
            inFrontier = false;
            parent = null;
            pathCost = -1;
            heuristicCost = 0;
        }

        public bool addToFrontier()
        {
            if(!inFrontier){
                inFrontier = true;
                return true;
            }
            return false;
        }

        public void calculateHeuristic(Vector2 target)
        {
            heuristicCost = Vector2.Distance(position, target);
        }

        public void updatePathCost(Node previousNode, float distanceCost)
        {
            if (pathCost == -1 || pathCost > distanceCost)
            {
                pathCost = distanceCost;
                parent = previousNode;
            }
        }

        public float getEdgeCost(Node other)
        {
            return Vector2.Distance(other.position, position);
        }

        public float getTotalCost()
        {
            return pathCost + heuristicCost;
        }

        public int CompareTo(Node other)
        {
            return (int)(getTotalCost() - other.getTotalCost());
        }

        public void draw(SpriteBatch spriteBatch, Texture2D img, Color color)
        {
            spriteBatch.Draw(img, new Vector2((int)position.X - 6, position.Y - 6), color);
        }

        public void drawId(SpriteBatch spriteBatch, SpriteFont font)
        {
            spriteBatch.DrawString(font, id+"", new Vector2(position.X, position.Y), Color.Black);
        }

        public void drawEdge(SpriteBatch spriteBatch, Texture2D img, Color color, Node neighbor)
        {
            Vector2 direction = neighbor.position - position;
            float angle = (float)Math.Atan2(direction.Y, direction.X);
            float distance = Vector2.Distance(position, neighbor.position);
            spriteBatch.Draw(img, new Vector2(position.X, position.Y), new Rectangle((int)position.X, (int)position.Y, (int)distance, 1),
                                color, angle, new Vector2(0, 0), 1.0f, SpriteEffects.None, 1);
        }

    }
}
