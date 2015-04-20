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
    public class Agent : BaseObject
    {
        
        public float heading
        {
            get;
            set;
        }

        public enum State
        {
            wander, seek, investigate
        }

        public enum MovementDirection
        {
            forward,
            backward,
            right,
            left
        }

        public BoundingBox nextLocation;
        public int range;
        public double rangeSquared;
        public List<Vector2> currentPathList;
        public Stack<Vector2> currentPath;
        public State state = State.wander;
        public Random rnd;
        public BaseObject currentlySeeking = null;
        public Ray leftBound;
        public Ray rightBound;
        public int countdown = 200;
        public AnimatedSprite sprite;
        public float pace;

        public Agent(int x, int y, int width, int length, float heading)
            : base(x, y, width, length)
        {
            this.heading = heading;
            rnd = new Random();
            range = 200;
            rangeSquared = Math.Pow(range, 2);
            pace = 1.4f;
            bounds = new BoundingBox(new Vector3(x, y+length+100, 0), new Vector3(x + width, y + length+200, 0));
        }

        public void move()
        {

            Vector2 dirVec = pace * getVectorFrom(heading);
            //Console.WriteLine(dirVec);

            sprite.Update();

            // Create a BoundingSphere that represents where the player will move
            //nextLocation = new BoundingSphere(new Vector3(dirVec + position, 0), 12);
            nextLocation = new BoundingBox(new Vector3((dirVec + position).X - (width / 2), (dirVec + position).Y - (length/3), 0), new Vector3((dirVec + position).X + (width / 2), (dirVec + position).Y + (length/2), 0));

            // If the new location of the player collides with any walls, don't update the location
            foreach (BoundingBox wall in Game1.walls)
            {
                if (nextLocation.Intersects(wall)){
                    if(currentPath != null){
                        Node n = Game1.navMap.getRandomNode();
                        setPath(n, n.position);
                    }
                    return;
                }
            }
            bounds = nextLocation;

            position += dirVec;
        }

        public void rotate(MovementDirection movementDirection)
        {
            heading = (movementDirection == MovementDirection.right) ? MathHelper.WrapAngle(heading + 0.05f) : MathHelper.WrapAngle(heading - 0.05f);
        }

        public bool seek(Vector2 destination)
        {
            if (Vector2.Distance(destination, position) > 1)
            {
                Vector2 desired = Vector2.Normalize(destination - position);
                heading = (float)Math.Atan2(desired.Y, desired.X);
                move();
                return false;
            }
            return true;
        }

        public bool detect(Vector2 target)
        {
            Vector2 leftToRight = new Vector2(range * (float)Math.Cos(heading - (2 * Math.PI / 3)), range * (float)Math.Sin(heading - (2 * Math.PI / 3)));
            Vector2 rightToLeft = new Vector2(range * (float)Math.Cos(heading + (2 * Math.PI / 3)), range * (float)Math.Sin(heading + (2 * Math.PI / 3)));

            Vector2 agentVector = position - target;
            float isLeftQuadrant = Vector2.Dot(agentVector, leftToRight);
            float isRightQuadrant = Vector2.Dot(agentVector, rightToLeft);

            if ((isLeftQuadrant > 0) && (isRightQuadrant > 0))
            {
                if (!targetObstructed(target))
                    return true;
            }
            return false;
        }

        public void locateDistraction()
        {
            // find closest item in range
            float minDistance = float.MaxValue;
            Item item = null;
            foreach (Item i in Game1.distractions)
            {
                float distance = Vector2.DistanceSquared(i.position, position);
                if (distance < rangeSquared)
                {
                    if (i.priority > 0 && distance < minDistance)
                    {
                        minDistance = distance;
                        item = i;
                    }
                }
            }

            if (currentlySeeking != null && currentlySeeking is Item)
            {
                if (Vector2.DistanceSquared(position, currentlySeeking.position) > minDistance)
                {
                    currentlySeeking = item;
                    setPath(Game1.navMap.getClosestNode(item.position), item.position);
                }
            }
            else if (item != null)
            {
                currentlySeeking = item;
                state = State.seek;
                setPath(Game1.navMap.getClosestNode(item.position), item.position);
            }
            // Player is in range
            else if (Vector2.DistanceSquared(Game1.player.position, position) < rangeSquared)
            {
                currentlySeeking = Game1.player;
                state = State.seek;

                setPath(Game1.navMap.getClosestNode(currentlySeeking.position), currentlySeeking.position);
            }
            // Just wander on
            followPath();
        }

        public void update()
        {

            if (state == State.seek)
            {
                if (currentlySeeking.priority == 0)
                {
                    currentlySeeking = null;
                    state = State.wander;
                }
                // found item/player
                else if (currentPath == null || currentPath.Count == 0)
                {
                    currentlySeeking.priority = 0;
                    if(currentlySeeking is Item)
                        Game1.distractions.Remove((Item)currentlySeeking);
                    state = State.investigate;
                    heading = (float)Math.PI;
                }
                else
                    locateDistraction();

            }
            else if (state == State.investigate)
            {
                float num = Game1.rnd.Next(0, 100);
                if (countdown > 0)
                {
                    
                    rotate((countdown > 100) ? MovementDirection.right : MovementDirection.left);
                    countdown--;
                }
                else
                {
                    currentlySeeking = null;
                    state = State.wander;
                    countdown = 200;
                }
                //else
                //{
                //    //followPath();
                //    //Console.WriteLine(num);
                //    if (num > 96)
                //    {
                //        currentlySeeking = null;
                //        state = State.wander;
                //        countdown = 200;
                //    }
                //}
                
            }
            else
                locateDistraction();
        }


        public bool targetObstructed(Vector2 target)
        {
            Vector3 vectorBetween = new Vector3(target - position, 0);
            Vector3 rotatedvector = 12 * Vector3.Normalize(new Vector3(-1 * vectorBetween.Y, vectorBetween.X, 0));
            Vector3 pos = new Vector3(position, 0);

            leftBound = new Ray(rotatedvector + pos, Vector3.Normalize(vectorBetween));
            rightBound = new Ray((-1 * rotatedvector) + pos, Vector3.Normalize(vectorBetween));

            double distance = Vector2.Distance(target, position);

            foreach (BoundingBox wall in Game1.walls)
            {
                Nullable<float> leftIntersect = leftBound.Intersects(wall);
                Nullable<float> rightIntersect = rightBound.Intersects(wall);

                if (leftIntersect <= distance || rightIntersect <= distance)
                {
                    return true;
                }
            }
            return false;

        }

        public List<Node> showObstructed()
        {
            List<Node> obstructed = new List<Node>();
            foreach (Node n in Game1.navMap.nodes)
            {
                if (targetObstructed(n.position))
                    obstructed.Add(n);
            }
            return obstructed;
        }

        public void setPath(Node target, Vector2 targetLoc)
        {
            if (targetLoc != null && !targetObstructed(targetLoc))
            {
                currentPathList = new List<Vector2>();
                currentPathList.Add(targetLoc);
                currentPath = new Stack<Vector2>();
                currentPath.Push(targetLoc);

            }
            else
            {
                if (currentPathList != null && Vector2.DistanceSquared(targetLoc, currentPathList[0]) < 400)
                {
                    return;
                }
                Node closest = Game1.navMap.getClosestNode(position);
                currentPathList = Game1.navMap.getShortestPath(closest, target);
                this.currentPath = new Stack<Vector2>();
                foreach (Vector2 n in currentPathList)
                    {
                        currentPath.Push(n);
                    }
            }
        }

        public void followPath()
        {
            if (currentPath == null || currentPath.Count == 0)
            {
                Node newnode;
                if (currentlySeeking != null)
                    newnode = Game1.navMap.getRandomNodeInRange(currentlySeeking.position, 30);
                else
                    newnode = Game1.navMap.getRandomNode();
                setPath(newnode, newnode.position);
            }
            if (seek(currentPath.Peek()))
            {
                currentPath.Pop();
            }
        }

        // Draw player specific items
        public void draw(SpriteBatch spriteBatch, Texture2D image)
        {
            double direction = (MathHelper.WrapAngle((float)(heading + (Math.PI / 4) + Math.PI)) + Math.PI) / (Math.PI / 2);

            int dir = Math.Abs(((int)direction) - 3);
            sprite.Draw(spriteBatch, position, dir);
        }

        #region Clutter

            public List<Agent> getAdjacentAgents()
            {
                List<Agent> adjacentAgents = new List<Agent>();

                foreach (Agent p in Game1.agents)
                {
                    if (getDistanceBetween(p.position) - (width / 2) <= range)
                    {
                        adjacentAgents.Add(p);
                    }
                }

                return adjacentAgents;
            }

            private Vector2 getVectorFrom(float heading)
            {
                return Vector2.Normalize(new Vector2((float)Math.Cos(heading), (float)Math.Sin(heading)));
            }

            public Vector2 getFacingVector()
            {
                return getVectorFrom(heading);
            }

            public Vector2 getNormalizedPosition()
            {
                return Vector2.Normalize(position);
            }

            public float getRelativeAngleBetween(Vector2 target)
            {
                float dotProduct = Vector2.Dot(getFacingVector(), Vector2.Normalize(new Vector2(target.X - x, target.Y - y)));
                return (float)Math.Acos(dotProduct);
            }

            public void drawAgentSensorCircle(SpriteBatch spriteBatch, Texture2D perimeter)
            {
                //spriteBatch.Draw(perimeter, position, new Rectangle(0, 0, 2*range, 2*range),
                             //Color.White, 0, new Vector2(2*range ,2*range), (float)range/400, SpriteEffects.None, 1);
                spriteBatch.Draw(perimeter, new Vector2(x - range - width / 2, y - range - width / 2), null, Color.White, 0, new Vector2(0, 0), (float)(2 * range + width) / 400, SpriteEffects.None, 1);
            }
        #endregion
    }
}
