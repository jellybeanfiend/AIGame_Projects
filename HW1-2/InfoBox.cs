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
    class InfoBox
    {
        public Rectangle bounds;
        private Player player;
        private int top;

        public Texture2D solidTexture;
        public SpriteFont font18;
        public SpriteFont font14;
        public SpriteFont font12;

        // Clickable areas
        public Rectangle[] toggleLinks;
        public bool[] toggles;
        Rectangle incRange;
        Rectangle decRange;
        Rectangle addAgent;
        List<Button> menuButtons;
        bool addMode = false;
        public bool drawNodes = true;
        //List<Button> createMap;

        public enum sideMenu { main, stats, toggleNodes, viewPath, agentPath, clickSeek };
        public enum mapActions { walls, subject, agent };
        public int menu;
        public int mapMode;

        // TODO textures
        public InfoBox(Rectangle bounds, Player player)
        {
            this.bounds = bounds;
            this.player = player;
            this.menu = (int)sideMenu.main;
            top = 50;

            menuButtons = new List<Button>();
            menuButtons.Add(new Button(new Rectangle(bounds.X + bounds.Width - 55, bounds.Y + 5, 50, 30), "Menu"));
            menuButtons.Add(new Button(new Rectangle(bounds.X + 10, bounds.Y + 35, 93, 25), "View Stats"));
            menuButtons.Add(new Button(new Rectangle(bounds.X + 10, bounds.Y + 65, 125, 25), "Toggle Nodes"));
            menuButtons.Add(new Button(new Rectangle(bounds.X + 10, bounds.Y + 95, 91, 25), "View Path"));
            menuButtons.Add(new Button(new Rectangle(bounds.X + 10, bounds.Y + 125, 108, 25), "Move Agent"));
            menuButtons.Add(new Button(new Rectangle(bounds.X + 10, bounds.Y + 155, 108, 25), "Click Seek"));


            //createMap = new List<Button>();
            //createMap.Add(new Button(new Rectangle(bounds.X + 10, top, 110, 25), "Add Walls"));
            //createMap.Add(new Button(new Rectangle(bounds.X + 10, top + 30, 110, 25), "Add Subject"));
            //createMap.Add(new Button(new Rectangle(bounds.X + 10, top + 60, 110, 25), "Add Agents"));

            toggleLinks = new Rectangle[3];
            toggleLinks[0] = new Rectangle(bounds.X + bounds.Width - 55, top + 120, 50, 20);
            toggleLinks[1] = new Rectangle(bounds.X + bounds.Width - 55, top + 210, 50, 20);
            toggleLinks[2] = new Rectangle(bounds.X + bounds.Width - 55, top + 278, 50, 20);
            toggles = new bool[4];
            incRange = new Rectangle(bounds.X + bounds.Width - 25, top, 20, 20);
            decRange = new Rectangle(bounds.X + bounds.Width - 55, top, 20, 20);
            addAgent = new Rectangle(bounds.X + bounds.Width - 85, bounds.Height - 25, 80, 20);
        }

        public void respondToInput(InputHandler input, PathGraph navMap)
        {

            // If the user clicked any of the toggle buttons, toggle their associated value
            for (int i = 0; i < toggleLinks.Length; i++)
            {
                if (input.mouseClicked(toggleLinks[i]))
                {
                    toggles[i] = !toggles[i];
                }
            }

            for (int i = 0; i < menuButtons.Count; i++)
            {
                if ((menu == (int)sideMenu.main || i == 0) && input.mouseClicked(menuButtons[i].bounds))
                {
                    resetState();
                    menu = i;
                }
            }

            //if (menu == (int)sideMenu.createMap && input.isMouseClicked())
            //{
            //    for (int i = 0; i < createMap.Count; i++ )
            //    {
            //        if (input.mouseClicked(createMap[i].bounds))
            //        {
            //            mapMode = i;
            //        }
            //    }
            //}

            // Add an enemy to the map with the coordinates of the mouse click
            if (addMode && input.mouseClicked(new Rectangle(0, 0, bounds.X, bounds.Height)))
            {
                var random = new Random();
                int randomNum = random.Next(0, (int)(2 * Math.PI));
                Program.agents.Add(new Player());
                Program.agents[Program.agents.Count - 1].Initialize(input.mouseStateCurrent.X, input.mouseStateCurrent.Y, randomNum);
                addMode = false;
            }

            // Increment/Decrement the range if the user clicked the plus or minus button
            if (input.mousePressed(incRange))
                player.range++;
            if (input.mousePressed(decRange))
                player.range--;
            // If the user clicked the "add agent" button, set the flag
            if (input.mouseClicked(addAgent))
                addMode = true;
        }

        public void resetState()
        {
            Game1.startNode = null;
            Game1.targetNode = null;
            Game1.shortestPath = null;
        }

        public void draw(SpriteBatch spriteBatch, List<Player> adjacentAgents)
        {
            int infoX = bounds.Left + 10;
            int infoY = top;

            // Draw background
            spriteBatch.Draw(solidTexture, new Rectangle(infoX - 10, 0, 250, bounds.Height), Color.Teal);

            switch (menu)
            {
                case ((int)sideMenu.main):
                    drawHeading(spriteBatch, "Menu");
                    foreach (Button button in menuButtons.Skip(1))
                            button.draw(spriteBatch, solidTexture, font14);
                    break;

                case (int)sideMenu.stats:
                    drawMenuButton(spriteBatch);
                    drawHeading(spriteBatch, "Stats");
                    drawStats(spriteBatch, adjacentAgents, infoX, infoY);
                    break;

                case (int)sideMenu.toggleNodes:
                    drawNodes = !drawNodes;
                    //drawMenuButton(spriteBatch);
                    menu = 0;
                    //drawMenuButton(spriteBatch);
                    //drawHeading(spriteBatch, "Create Map");
                    //drawCreateMapMode(spriteBatch);
                    break;

                case (int)sideMenu.viewPath:
                    drawMenuButton(spriteBatch);
                    drawHeading(spriteBatch, "View Path");
                    drawViewPath(spriteBatch);
                    break;

                case (int)sideMenu.agentPath:
                    drawMenuButton(spriteBatch);
                    drawHeading(spriteBatch, "Set Agent Path");
                    drawAgentPathMode(spriteBatch);
                    break;

                case (int)sideMenu.clickSeek:
                    drawMenuButton(spriteBatch);
                    drawHeading(spriteBatch, "Click Seek");
                    break;
            }


            // Display range and draw buttons that increment/decrement range+


            // Draw button for adding agents
            spriteBatch.Draw(solidTexture, addAgent, Color.MediumTurquoise);
            spriteBatch.DrawString(font12, "Add Agent", new Vector2(addAgent.X + 3, addAgent.Y), Color.White);
        }

        public void drawHeading(SpriteBatch spriteBatch, string text)
        {
            spriteBatch.DrawString(font18, text, new Vector2(bounds.X + 10, bounds.Y + 5), Color.White);
        }

        public void drawMenuButton(SpriteBatch spriteBatch)
        {
            menuButtons[0].draw(spriteBatch, solidTexture, font14);
        }

        public void drawStats(SpriteBatch spriteBatch, List<Player> adjacentAgents, int infoX, int infoY)
        {
            spriteBatch.DrawString(font14, "Range: " + player.range, new Vector2(infoX, infoY), Color.Black);
            spriteBatch.Draw(solidTexture, incRange, Color.MediumTurquoise);
            spriteBatch.Draw(solidTexture, decRange, Color.MediumTurquoise);
            spriteBatch.DrawString(font14, "+", new Vector2(incRange.X + 4, incRange.Y - 4), Color.White);
            spriteBatch.DrawString(font14, "-", new Vector2(decRange.X + 6, decRange.Y - 4), Color.White);

            // Draw Player Info (x, y, heading)
            spriteBatch.DrawString(font14, "Player Information:", new Vector2(infoX, infoY += 25), Color.Black);
            spriteBatch.DrawString(font14, "X: " + player.x, new Vector2(infoX, infoY += 25), Color.White);
            spriteBatch.DrawString(font14, "Y: " + player.y, new Vector2(infoX, infoY += 18), Color.White);
            spriteBatch.DrawString(font14, "Heading: " + displayDegrees(MathHelper.ToDegrees(player.heading)).ToString("0.00"), new Vector2(infoX, infoY += 18), Color.White);

            // Draw Feelers info
            spriteBatch.DrawString(font14, "Feelers:", new Vector2(infoX, infoY += 25), Color.Black);
            infoY += 5;
            for (int i = 0; i < player.feelers.Count; i++)
            {
                float length = player.getFeelerLength(player.feelers[i]);
                spriteBatch.DrawString(font14, "Feeler " + i + ": " + (length < player.range ? length + "" : ""), new Vector2(infoX, infoY += 18), (length < player.range) ? Color.Red : Color.White);
            }
            infoY += 5;

            // Draw Pie Slices info
            spriteBatch.DrawString(font14, "Pie Slices:", new Vector2(infoX, infoY += 25), Color.Black);
            infoY += 5;
            int[] activationLevels = player.getPieSlices(adjacentAgents);
            spriteBatch.DrawString(font14, "1  2  3  4", new Vector2(infoX, infoY += 18), Color.White);
            spriteBatch.DrawString(font14, activationLevels[1] + "  " + activationLevels[2] + "  " + activationLevels[3] + "  " + activationLevels[4], new Vector2(infoX, infoY += 18), Color.White);
            infoY += 5;

            // Draw Adjacent Agents info
            spriteBatch.DrawString(font14, "Adjacent Agents:", new Vector2(infoX, infoY += 25), Color.Black);
            infoY += 5;
            if (adjacentAgents.Count == 0)
                spriteBatch.DrawString(font14, "None", new Vector2(infoX, infoY += 18), Color.White);
            for (int i = 0; i < adjacentAgents.Count; i++)
            {
                spriteBatch.DrawString(font12, "Agent " + i + ": ", new Vector2(infoX, infoY += 20), Color.White);
                spriteBatch.DrawString(font12, "D: " + player.getDistanceBetween(adjacentAgents[i].position).ToString("0.00"), new Vector2(infoX + 65, infoY), Color.White);
                spriteBatch.DrawString(font12, "H: " + (MathHelper.ToDegrees(player.getRelativeAngleBetween(adjacentAgents[i]))).ToString("0.00"), new Vector2(infoX + 130, infoY), Color.White);
            }

            // Draw toggle buttons
            foreach (Rectangle rect in toggleLinks)
            {
                spriteBatch.Draw(solidTexture, rect, Color.MediumTurquoise);
                spriteBatch.DrawString(font12, "Toggle", new Vector2(rect.X, rect.Y), Color.White);
            }
        }

        public void drawViewPath(SpriteBatch spriteBatch)
        {
            if(Game1.startNode == null)
                spriteBatch.DrawString(font14, "Select a start node", new Vector2(bounds.X + 10, top), Color.White);
            else if (Game1.targetNode == null)
            {
                spriteBatch.DrawString(font14, "Start: " + Game1.startNode.position.X + ", " + Game1.startNode.position.Y, new Vector2(bounds.X + 10, top), Color.White);
                spriteBatch.DrawString(font14, "Select a target node", new Vector2(bounds.X + 10, top + 20), Color.White);
            }
            else
            {
                spriteBatch.DrawString(font14, "Start: " + Game1.startNode.position.X + ", " + Game1.startNode.position.Y, new Vector2(bounds.X + 10, top), Color.White);
                spriteBatch.DrawString(font14, "Target: " + Game1.targetNode.position.X + ", " + Game1.targetNode.position.Y, new Vector2(bounds.X + 10, top + 20), Color.White);
            }
        }

        public void drawAgentPathMode(SpriteBatch spriteBatch)
        {
            if (Game1.targetNode == null)
            {
                spriteBatch.DrawString(font14, "Select a target node", new Vector2(bounds.X + 10, top), Color.White);
            }
            else
            {
                spriteBatch.DrawString(font14, "Target: " + Game1.targetNode.bound.X + ", " + Game1.targetNode.bound.Y, new Vector2(bounds.X + 10, top), Color.White);
            }
        }

        //public void drawCreateMapMode(SpriteBatch spriteBatch)
        //{
        //    foreach (Button button in createMap)
        //    {
        //        button.draw(spriteBatch, solidTexture, font14);
        //    }
        //}

        public float displayDegrees(float num)
        {
            if (num < 0)
                return Math.Abs(num);
            else
                return 360 - num;
        }
    }
}
