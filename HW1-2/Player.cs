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
	public class Player : BaseObject
	{
		public enum MovementDirection
		{
			forward,
			backward,
            right,
            left
		}

        public List<Ray> feelers;
        public BoundingSphere nextLocation;
        public int range = 50;
        public Vector2 velocity;
        public Stack<Node> shortestPath;

		/// <summary>
		/// Initializes the Player object. Sets length and width to 1.
		/// </summary>
		/// <param name="x">The starting X coordinate of the object.</param>
		/// <param name="y">The starting Y coordinate of the object.</param>
		/// <param name="heading">The heading of the object.</param>
		public void Initialize(int x, int y, float heading)
		{
			Initialize(x, y, 24, 24, heading);

            // Create feelers for the player
            Vector3 playerpos = new Vector3(x, y, 0);
            velocity = new Vector2((float)Math.Cos(heading), (float)Math.Sin(heading));
            feelers = new List<Ray>();
            feelers.Add(new Ray(playerpos, new Vector3((float)Math.Cos(heading - Math.PI / 6), (float)Math.Sin(heading - Math.PI / 6), 0)));
            feelers.Add(new Ray(playerpos, new Vector3((float)Math.Cos(heading), (float)Math.Sin(heading), 0)));
            feelers.Add(new Ray(playerpos, new Vector3((float)Math.Cos(heading + Math.PI / 6), (float)Math.Sin(heading + Math.PI / 6), 0)));
        }

        /** ASSIGNMENT 1 **/

        /// <summary>
        /// Update each feeler to match the coordinates and heading of the player.
        /// </summary>
        // Note: there's probably a better way to do this, but for now I'm making new objects each time! Yikes!
        public void updateFeelers()
        {
            feelers[0] = new Ray(new Vector3(position, 0), new Vector3((float)Math.Cos(heading - Math.PI / 6), (float)Math.Sin(heading - Math.PI / 6), 0));
            feelers[1] = new Ray(new Vector3(position, 0), new Vector3((float)Math.Cos(heading), (float)Math.Sin(heading), 0));
            feelers[2] = new Ray(new Vector3(position, 0), new Vector3((float)Math.Cos(heading + Math.PI / 6), (float)Math.Sin(heading + Math.PI / 6), 0));
        }

        /// <summary>
        /// Determines the length of the given feeler so that it can be drawn.
        /// The length is the distance between the player and the closest wall that intersects with said feeler,
        /// or a default max length if the feeler does not intersect with any walls.
        /// </summary>
        /// <param name="feeler">The feeler to check.</param>
        /// <returns>The distance between the player and the wall that intersects with the feeler, or a default max length if no walls intersect.</returns>
        public float getFeelerLength(Ray feeler)
        {
            float min = range;
            foreach (BoundingBox wall in Program.walls)
            {
                if (feeler.Intersects(wall) != null && feeler.Intersects(wall)-(width/2) <= min)
                {
                    min = (float)feeler.Intersects(wall) - (width / 2);
                }
            }
            return min;
        }

        /// <summary>
        /// Calculates the next location of the player and ensures that it does not collide with any walls.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="gameTime">Its game time!</param>
        /// <param name="movementDirection">Key pressed by user</param>
		public void move(MovementDirection movementDirection)
		{

            Vector2 dirVec = (movementDirection == MovementDirection.forward) ? getVectorFrom(heading) : Vector2.Negate(getVectorFrom(heading));

            // Create a BoundingSphere that represents where the player will move
            nextLocation = new BoundingSphere(new Vector3((dirVec + position), 0), 12);

            // If the new location of the player collides with any walls, don't update the location
            foreach (BoundingBox wall in Program.walls)
            {
                if (nextLocation.Intersects(wall))
                    return;
            }

            position += dirVec;
            
		}

		private Vector2 getVectorFrom(float heading)
		{
			return Vector2.Normalize(new Vector2((float)Math.Cos(heading), (float)Math.Sin(heading)));
		}

        public void rotate(MovementDirection movementDirection)
		{
            heading = (movementDirection == MovementDirection.right) ? MathHelper.WrapAngle(heading + 0.05f) : MathHelper.WrapAngle(heading - 0.05f);
		}

		public Vector2 getFacingVector()
		{
			return getVectorFrom(heading);
		}

		public Vector2 getNormalizedPosition()
		{
			return Vector2.Normalize(position);
		}

		public float getRelativeAngleBetween(Player player)
		{
			float dotProduct = Vector2.Dot(getFacingVector(), Vector2.Normalize(new Vector2( player.x - x, player.y - y)));
            return (float)Math.Acos(dotProduct);
		}

        /// <summary>
        /// Examines each adjacent agent and determines which pie slice it belongs in.
        /// </summary>
        /// <param name="adjacentAgents">List of all agents within a range of the player</param>
        /// <returns>array of ints, the index corresponds to the slice and stores the number of agents in that slice.</returns>
        public int[] getPieSlices(List<Player> adjacentAgents)
        {
            Vector2 leftToRight = new Vector2(range * (float)Math.Cos(heading - (3 * Math.PI / 4)), range * (float)Math.Sin(heading - (3 * Math.PI / 4)));
            Vector2 rightToLeft = new Vector2(range * (float)Math.Cos(heading + (3 * Math.PI / 4)), range * (float)Math.Sin(heading + (3 * Math.PI / 4)));

            int[] activationLevels = new int[5];

            foreach (Player agent in adjacentAgents)
            {
                Vector2 agentVector = position - agent.position;
                float isLeftQuadrant = Vector2.Dot(agentVector, leftToRight);
                float isRightQuadrant = Vector2.Dot(agentVector, rightToLeft);

                if ((isLeftQuadrant < 0) && (isRightQuadrant < 0)) // 3rd quadrant
                {
                    activationLevels[3]++;
                }
                else if ((isLeftQuadrant < 0) && (isRightQuadrant > 0)) // 4th quadrant
                {
                    activationLevels[4]++;
                }
                else if ((isLeftQuadrant > 0) && (isRightQuadrant < 0)) // 2nd quadrant
                {
                    activationLevels[2]++;
                }
                else
                {
                    activationLevels[1]++; // 1st quadrant
                }
            }

            return activationLevels;
            
        }

		/// <summary>
		/// Gets the distance between this agent and another agent.
		/// </summary>
		/// <param name="p">The agent to check against.</param>
		/// <returns>The distance between the two agents.</returns>
		public double getDistanceBetween(Vector2 location)
		{
            return Math.Max(Math.Sqrt(Math.Pow(Math.Abs(x - location.X), 2) + Math.Pow(Math.Abs(y - location.Y), 2)) - width, 0);
		}

		/// <summary>
		/// Gets a list of all agents within the specified range.
		/// </summary>
		/// <param name="n">The range.</param>
		/// <returns>A List containing all agents within the specified range. If there are no agents in range, returns an empty List.</returns>
		public List<Player> getAdjacentAgents()
		{
            List<Player> adjacentAgents = new List<Player>();

			foreach (Player p in Program.agents) {
				if (getDistanceBetween(p.position) <= range)
                {
                    adjacentAgents.Add(p);
                }
			}

            return adjacentAgents;
		}

        /** HOMEWORK 2 **/

        public bool seek(Vector2 destination)
        {
            float dist = Vector2.Distance(destination, position);
            if (dist > 1)
            {
                Vector2 desired = Vector2.Normalize(destination - position);
                heading = (float)Math.Atan2(desired.Y, desired.X);
                move(MovementDirection.forward);
                return false;
            }
            return true;
        }

        public Node getRandomNode(HashSet<Node> nodes)
        {
            Random rnd = new Random();
            int r = rnd.Next(nodes.Count);
            List<Node> list = nodes.ToList();
            return list[r];
        }

        public bool isTargetObstructed(Vector2 target, double distance)
        {
            Vector2 desired = target - position;
            Ray lineOfSight = new Ray(new Vector3(position, 0), new Vector3((desired), 0));
            foreach (BoundingBox wall in Program.walls)
            {
                if(lineOfSight.Intersects(wall) < distance)
                    return true;
            }
            return false;
        }

        public List<Node> setPath(Node target, PathGraph navMap)
        {
            Node closest = getClosestNode(getAdjacentNodes(navMap.nodes));
            List<Node> tempShortest = navMap.getShortestPath(closest, target);
            shortestPath = new Stack<Node>();
            foreach (Node n in tempShortest)
            {
                shortestPath.Push(n);
            }
            return tempShortest;
        }

        public void followPath()
        {
            if (shortestPath.Count == 0)
            {
                shortestPath = null;
                return;
            }
            if (seek(shortestPath.Peek().position))
                shortestPath.Pop();
        }

        public Node getClosestNode(Heap<Node> list)
        {
            return list.peek();
        }

        public Heap<Node> getAdjacentNodes(HashSet<Node> nodes)
        {
            Heap<Node> orderedByDistance = new Heap<Node>();
            foreach (Node n in nodes)
            {
                n.heuristicCost = (float)getDistanceBetween(n.position);
                n.pathCost = 0;

                //if (!isTargetObstructed(n.position, n.heuristicCost))
                    orderedByDistance.insert(n);
            }

            return orderedByDistance;
        }

        //public Dictionary<Node, double> getAdjacentNodes(HashSet<Node> nodes)
        //{
        //    Dictionary<Node, double> adjacentNodes = new Dictionary<Node, double>();

        //    if (nodes == null || nodes.Count == 0)
        //        Console.WriteLine("NODES IS NULL");

        //    foreach (Node n in nodes)
        //    {
        //        double distance = getDistanceBetween(n.position);
        //        if (distance <= range && !isTargetObstructed(n.position, distance))
        //        {
        //            adjacentNodes.Add(n, distance);
        //        }
        //    }

        //    if (adjacentNodes.Count == 0)
        //    {
        //        Console.WriteLine("NO ADJACENT NODES FOUND");
        //        Console.WriteLine("AT POSITION" + x + y);
        //    }


        //    return adjacentNodes;
        //}

        /** HOMEWORK 1 DRAW FUNCS **/
    

        // Draw player specific items
        public void drawPlayer(SpriteBatch spriteBatch, Texture2D image)
        {
            spriteBatch.Draw(image,position, new Rectangle(0,0,width, length),
                             Color.White, heading + (float)Math.PI / 2, new Vector2(width/2,length/2), 1.0f, SpriteEffects.None, 1);
        }

        public void drawPieSlices(SpriteBatch spriteBatch, Texture2D solidTexture, SpriteFont font)
        {

            Vector2 frontLeft = new Vector2((float)Math.Cos(heading - (Math.PI / 4)), (float)Math.Sin(heading - (Math.PI / 4)));
            Vector2 frontRight = new Vector2((float)Math.Cos(heading + (Math.PI / 4)), (float)Math.Sin(heading + (Math.PI / 4)));
            Vector2 backLeft = new Vector2((float)Math.Cos(heading - (3 * Math.PI / 4)), (float)Math.Sin(heading - (3 * Math.PI / 4)));
            Vector2 backRight = new Vector2((float)Math.Cos(heading + (3 * Math.PI / 4)), (float)Math.Sin(heading + (3 * Math.PI / 4)));

            spriteBatch.Draw(solidTexture, frontLeft + position, new Rectangle(0, 0, range+(width/2), 2), Color.Black, (float)Math.Atan2(frontLeft.Y, frontLeft.X), new Vector2(0, 0), 1.0f, SpriteEffects.None, 1);
            spriteBatch.Draw(solidTexture, frontRight + position, new Rectangle(0, 0, range + (width / 2), 2), Color.Black, (float)Math.Atan2(frontRight.Y, frontRight.X), new Vector2(0, 0), 1.0f, SpriteEffects.None, 1);
            spriteBatch.Draw(solidTexture, backLeft + position, new Rectangle(0, 0, range + (width / 2), 2), Color.Black, (float)Math.Atan2(backLeft.Y, backLeft.X), new Vector2(0, 0), 1.0f, SpriteEffects.None, 1);
            spriteBatch.Draw(solidTexture, backRight + position, new Rectangle(0, 0, range + (width / 2), 2), Color.Black, (float)Math.Atan2(backRight.Y, backRight.X), new Vector2(0, 0), 1.0f, SpriteEffects.None, 1);

            Vector2 fontsize = font.MeasureString("1");

            spriteBatch.DrawString(font, "1", position, Color.Black, (float)(heading + (Math.PI / 2)), new Vector2(fontsize.X / 2, 3 * fontsize.Y / 2), 1.0f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "2", position, Color.Black, (float)(heading + (Math.PI / 2)), new Vector2(-range + fontsize.Y, fontsize.Y / 2), 1.0f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "3", position, Color.Black, (float)(heading + (Math.PI / 2)), new Vector2(fontsize.X / 2, -range + fontsize.Y), 1.0f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "4", position, Color.Black, (float)(heading + (Math.PI / 2)), new Vector2(range - fontsize.Y / 2, fontsize.Y / 2), 1.0f, SpriteEffects.None, 1);
            
        }

        public void drawFeelers(SpriteBatch spriteBatch, Texture2D image)
        {
            foreach (Ray feeler in feelers)
            {
                Color c = Color.LimeGreen;
                float feelerLength = getFeelerLength(feeler);
                if (feelerLength < range)
                    c = Color.Red;

                spriteBatch.Draw(image, new Vector2(feeler.Position.X, feeler.Position.Y), new Rectangle(0, 0, (int)feelerLength+width/2, 2),
                            c, (float)Math.Atan2(feeler.Direction.Y, feeler.Direction.X), new Vector2(0,0), 1.0f, SpriteEffects.None, 1);
            }
        }

        public void drawAgentSensorCircle(SpriteBatch spriteBatch, Texture2D perimeter)
        {
            spriteBatch.Draw(perimeter, new Vector2(x - range - width/2, y - range - width/2), null, Color.White, 0, new Vector2(0, 0), (float)(2*range+width) / 400, SpriteEffects.None, 1);
        }

	}
}
