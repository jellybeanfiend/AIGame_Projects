// Sevena Skeels
// Homework 2
// CAP4053

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AIClass
{
	public class Player : Agent
	{

        public List<Ray> feelers;
        public MovementDirection currentdirection;

        public Player(int x, int y, int width, int length, float heading)
            : base(x, y, width, length, heading)
        {
            Vector3 playerpos = new Vector3(x, y, 0);
            priority = 20;
            feelers = new List<Ray>();
            feelers.Add(new Ray(playerpos, new Vector3((float)Math.Cos(heading - Math.PI / 6), (float)Math.Sin(heading - Math.PI / 6), 0)));
            feelers.Add(new Ray(playerpos, new Vector3((float)Math.Cos(heading), (float)Math.Sin(heading), 0)));
            feelers.Add(new Ray(playerpos, new Vector3((float)Math.Cos(heading + Math.PI / 6), (float)Math.Sin(heading + Math.PI / 6), 0)));
            pace = 1.2f;
            currentdirection = MovementDirection.forward;
        }

        public void update(KeyboardState currentKState, KeyboardState prevKState)
        {
            bool moving = false;

            if (currentKState.IsKeyDown(Keys.Right))
            {
                heading = 0;
                moving = true;
                move();
 
            }

            if (currentKState.IsKeyDown(Keys.Left))
            {
                heading = (float)Math.PI;
                moving = true;
                move();

            }

            if (currentKState.IsKeyDown(Keys.Up))
            {
                heading = 3*(float)Math.PI /2;
                moving = true;
                currentdirection = MovementDirection.forward;
                move();
            }
            if (currentKState.IsKeyDown(Keys.Down))
            {
                heading = (float)Math.PI /2;
                moving = true;
                currentdirection = MovementDirection.backward;
                move();
            }

            if (moving)
            {
                sprite.Update();

                foreach (BoundingBox grass in Game1.grass)
                {
                    if (grass.Intersects(bounds) && Game1.rnd.Next(1, 1000) > 993)
                        Game1.inventory.addRandomItem();
                }

                foreach (Chest chest in Game1.chests)
                    if (Vector2.DistanceSquared(chest.position, position) < 3000 && !chest.isOpen)
                    {
                        Game1.foundchests--;
                        chest.isOpen = true;
                        Game1.justOpened = chest;
                        Game1.counter = 50;
                        break;
                    }
            }


            // TODO: Add check to see if any enemies got you!
            foreach (Item i in Game1.distractions)
                if (i.bounds.Intersects(bounds) && i.priority == 0)
                {
                    Game1.inventory.items.Add(i);
                    Game1.distractions.Remove(i);
                    break;
                }
            foreach (Agent enemy in Game1.agents)
            {
                if (enemy.bounds.Intersects(bounds))
                    Game1.state = Game1.State.lost;
            }
        }

        #region Clutter

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
                foreach (BoundingBox wall in Game1.walls)
                {
                    if (feeler.Intersects(wall) != null && feeler.Intersects(wall) - (width / 2) <= min)
                    {
                        min = (float)feeler.Intersects(wall) - (width / 2);
                    }
                }
                return min;
            }

            public int[] getPieSlices(List<Agent> adjacentAgents)
            {
                Vector2 leftToRight = new Vector2(range * (float)Math.Cos(heading - (3 * Math.PI / 4)), range * (float)Math.Sin(heading - (3 * Math.PI / 4)));
                Vector2 rightToLeft = new Vector2(range * (float)Math.Cos(heading + (3 * Math.PI / 4)), range * (float)Math.Sin(heading + (3 * Math.PI / 4)));

                int[] activationLevels = new int[5];

                foreach (Agent agent in adjacentAgents)
                {
                    Vector2 agentVector = position - agent.position;
                    float isLeftQuadrant = Vector2.Dot(agentVector, leftToRight);
                    float isRightQuadrant = Vector2.Dot(agentVector, rightToLeft);

                    if ((isLeftQuadrant < 0) && (isRightQuadrant < 0)) // 3rd quadrant
                    {
                        Console.WriteLine("3");
                        activationLevels[3]++;
                    }
                    else if ((isLeftQuadrant < 0) && (isRightQuadrant > 0)) // 4th quadrant
                    {
                        Console.WriteLine("4");
                        activationLevels[4]++;
                    }
                    else if ((isLeftQuadrant > 0) && (isRightQuadrant < 0)) // 2nd quadrant
                    {
                        Console.WriteLine("2");
                        activationLevels[2]++;
                    }
                    else
                    {
                        Console.WriteLine("1 - IN VIEW");
                        activationLevels[1]++; // 1st quadrant
                    }
                }

                return activationLevels;

            }

            public void drawPieSlices(SpriteBatch spriteBatch, Texture2D solidTexture, SpriteFont font)
            {

                Vector2 frontLeft = new Vector2((float)Math.Cos(heading - (Math.PI / 4)), (float)Math.Sin(heading - (Math.PI / 4)));
                Vector2 frontRight = new Vector2((float)Math.Cos(heading + (Math.PI / 4)), (float)Math.Sin(heading + (Math.PI / 4)));
                Vector2 backLeft = new Vector2((float)Math.Cos(heading - (3 * Math.PI / 4)), (float)Math.Sin(heading - (3 * Math.PI / 4)));
                Vector2 backRight = new Vector2((float)Math.Cos(heading + (3 * Math.PI / 4)), (float)Math.Sin(heading + (3 * Math.PI / 4)));

                spriteBatch.Draw(solidTexture, frontLeft + position, new Rectangle(0, 0, range + (width / 2), 2), Color.Black, (float)Math.Atan2(frontLeft.Y, frontLeft.X), new Vector2(0, 0), 1.0f, SpriteEffects.None, 1);
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

                    spriteBatch.Draw(image, new Vector2(feeler.Position.X, feeler.Position.Y), new Rectangle(0, 0, (int)feelerLength + width / 2, 2),
                                c, (float)Math.Atan2(feeler.Direction.Y, feeler.Direction.X), new Vector2(0, 0), 1.0f, SpriteEffects.None, 1);
                }
            }

        #endregion
    }
}
