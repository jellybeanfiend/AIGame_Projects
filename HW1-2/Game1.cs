// Sevena Skeels
// Homework 2
// CAP4053

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace AIClass
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Game
	{

        int INFOBOXWIDTH = 240;

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
        SpriteBatch nodesBatch;
		Player player;
        Viewport viewport;
        PathGraph navMap;
        InfoBox infobox;
        InputHandler input;

        // Textures
        public Texture2D playerImage;
        public Texture2D enemyImage;
        public Texture2D solidTexture;
        public Texture2D perimeter;
        public Texture2D nodeimg;
        public Texture2D badguyOutline;
        public SpriteFont font18;
        public SpriteFont font12;
        public SpriteFont font14;
        public Texture2D selectednode;
        public Texture2D pathnode;
        public Texture2D targetnode;

        public static Node startNode;
        public static Node targetNode;
        public static List<Node> shortestPath;
        public Vector2 clickSeek;

		public Game1()
			: base()
		{
            try { graphics = new GraphicsDeviceManager(this); }
            catch { }
			Program.agents = new List<Player>();
            Program.walls = new List<BoundingBox>();
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{

            player = new Player();
            // Use Viewport to get width and height of the screen
            viewport = GraphicsDevice.Viewport;
            player.Initialize(viewport.Width/2, viewport.Height/2, (float)-Math.PI/2);

            Rectangle infoboxDimension = new Rectangle(viewport.Width - INFOBOXWIDTH, 0, INFOBOXWIDTH, viewport.Height);
            infobox = new InfoBox(infoboxDimension, player);
            input = new InputHandler();

            // Populate world

            // Add agents
            Program.agents.Add(new Player());
            Program.agents.Add(new Player());
            Program.agents.Add(new Player());
            Program.agents[0].Initialize(50, 150, 0);
            Program.agents[1].Initialize(100, 20, 90);
            Program.agents[2].Initialize(200, 300, 90);

            // Add walls
            Program.walls.Add(new BoundingBox(new Vector3(20, 70, 0), new Vector3(260, 100, 0)));
            Program.walls.Add(new BoundingBox(new Vector3(470, 10, 0), new Vector3(510, 410, 0)));
            Program.walls.Add(new BoundingBox(new Vector3(40, 340, 0), new Vector3(390, 370, 0)));

            navMap = new PathGraph(viewport.Width - 240, viewport.Height);

            
           
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);
            nodesBatch = new SpriteBatch(GraphicsDevice);
            // Load images
            playerImage = Content.Load<Texture2D>("goodguy");
            enemyImage = Content.Load<Texture2D>("badguy");
            solidTexture = new Texture2D(GraphicsDevice, 1, 1);
            perimeter = Content.Load<Texture2D>("bigcircle");
            badguyOutline = Content.Load<Texture2D>("badguyoutline");
            solidTexture.SetData(new Color[] { Color.White });
            font18 = Content.Load<SpriteFont>("SpriteFont1");
            font12 = Content.Load<SpriteFont>("SpriteFont2");
            font14 = Content.Load<SpriteFont>("SpriteFont3");
            nodeimg = Content.Load<Texture2D>("node");
            selectednode = Content.Load<Texture2D>("selectednode");
            pathnode = Content.Load<Texture2D>("pathnode");
            targetnode = Content.Load<Texture2D>("targetnode");

            // Infobox content

            infobox.solidTexture = solidTexture;
            infobox.font18 = font18;
            infobox.font14 = font14;
            infobox.font12 = font12;


		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
            /** HOMEWORK 2 **/

            input.updateState();

            infobox.respondToInput(input, navMap);
            selectPathNodes();

            if (input.isMouseClicked() && infobox.menu == (int)AIClass.InfoBox.sideMenu.clickSeek)
            {
                clickSeek = new Vector2(input.mouseStateCurrent.X, input.mouseStateCurrent.Y);
            }

            input.saveState();

            /** HOMEWORK 1 **/

			KeyboardState kState = Keyboard.GetState();
			if (kState.IsKeyDown(Keys.Right))
				player.rotate(Player.MovementDirection.right);
			if (kState.IsKeyDown(Keys.Left))
                player.rotate(Player.MovementDirection.left);
			if (kState.IsKeyDown(Keys.Up))
				player.move(Player.MovementDirection.forward);
			if (kState.IsKeyDown(Keys.Down))
				player.move(Player.MovementDirection.backward);
			if (Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

            player.updateFeelers();

            /** HOMEWORK 2**/

            foreach (Player agent in Program.agents)
            {
                if (agent.shortestPath == null)
                {
                    Console.WriteLine("AGENT BOUT TO CALL");
                    agent.setPath(agent.getRandomNode(navMap.nodes), navMap);
                }
                else
                    agent.followPath();
            }

            if (infobox.menu == (int)AIClass.InfoBox.sideMenu.agentPath && player.shortestPath != null)
            {
                player.followPath();
            }

            if (infobox.menu == (int)AIClass.InfoBox.sideMenu.clickSeek)
            {
                player.seek(clickSeek);
            }

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.White);

            List<Player> adjacentAgents = player.getAdjacentAgents();

            spriteBatch.Begin();

            

            // Draw circle around player for "agent sensors" display
            if (infobox.toggles[2])
                player.drawAgentSensorCircle(spriteBatch, perimeter);

            // Draw pie slice vectors
            if (infobox.toggles[1])
                player.drawPieSlices(spriteBatch, solidTexture, font18);

            // Draw feelers for the player
            if (infobox.toggles[0])
                player.drawFeelers(spriteBatch, solidTexture);

            if(infobox.drawNodes)
                drawNodes(spriteBatch);


            if (infobox.menu == (int)AIClass.InfoBox.sideMenu.viewPath && shortestPath != null)
            {
                drawShortestPath(spriteBatch, shortestPath);
            }

            if (infobox.menu == (int)AIClass.InfoBox.sideMenu.agentPath && shortestPath != null)
            {
                drawShortestPath(spriteBatch, shortestPath);
            }

            // Draw sprite for player
            player.drawPlayer(spriteBatch, playerImage);

            // If enemies are in range of the player, outline them in red
            foreach (Player enemy in adjacentAgents)
            {
                spriteBatch.Draw(badguyOutline, new Vector2((int)enemy.x-enemy.width/2-1, (int)enemy.y-enemy.length/2-1), Color.White);
            }

            // Draw enemies
            foreach (Player enemy in Program.agents)
            {
                spriteBatch.Draw(enemyImage, enemy.position, new Rectangle(0, 0, enemy.width, enemy.length),
                            Color.White, enemy.heading + (float)Math.PI / 2, new Vector2(enemy.width / 2, enemy.length / 2), 1.0f, SpriteEffects.None, 1);
                
            }

            // Draw walls
            foreach (BoundingBox wall in Program.walls)
            {
                spriteBatch.Draw(solidTexture, new Rectangle((int)wall.Min.X, (int)wall.Min.Y, (int)(wall.Max.X - wall.Min.X), (int)(wall.Max.Y - wall.Min.Y)), Color.Teal);
            }

            drawSelectedNodes(spriteBatch);

            // Draw a box and display stats for player location, feeler lengths, activation levels, etc.
            infobox.draw(spriteBatch, adjacentAgents);

            spriteBatch.End();

			base.Draw(gameTime);
		}

        /** HOMEWORK 2 **/

        public void drawNodes(SpriteBatch spriteBatch)
        {
            foreach (Node node in navMap.nodes)
            {
                
                foreach (Node adjacent_node in node.neighbors)
                {
                    node.drawEdge(spriteBatch, solidTexture, Color.LightGray, adjacent_node);
                }
            }
            foreach (Node n in navMap.nodes)
            {
                n.draw(spriteBatch, nodeimg, Color.LightGray);
                n.drawId(spriteBatch, font14);
            }
        }

        public void drawShortestPath(SpriteBatch spriteBatch, List<Node> shortestPath)
        {
            for (int i = 0; i < shortestPath.Count-1; i++)
            {
                shortestPath[i].drawEdge(spriteBatch, solidTexture, Color.Black, shortestPath[i+1]);
            }

            foreach(Node n in shortestPath){
                n.draw(spriteBatch, pathnode, Color.White);
            }
        }

        public void drawSelectedNodes(SpriteBatch spriteBatch)
        {
            if(startNode != null)
                startNode.draw(spriteBatch, selectednode, Color.White);
            if(targetNode != null)
                targetNode.draw(spriteBatch, targetnode, Color.White);
        }

        public void selectPathNodes()
        {
            if (input.isMouseClicked())
            {
                foreach (Node n in navMap.nodes)
                {
                    if (input.mouseClicked(n.bound))
                    {
                        if (infobox.menu == (int)AIClass.InfoBox.sideMenu.viewPath)
                            updateViewPathMode(n);
                        if (infobox.menu == (int)AIClass.InfoBox.sideMenu.agentPath)
                            updateAgentPathMode(n);
                    }
                }
            }
        }

        public void updateViewPathMode(Node n)
        {
            if (startNode == null)
                startNode = n;
            else if (targetNode == null)
            {
                targetNode = n;
                shortestPath = navMap.getShortestPath(startNode, targetNode);
            }
            else
            {
                startNode = n;
                targetNode = null;
                shortestPath = null;
            }
        }

        public void updateAgentPathMode(Node n)
        {
            targetNode = n;
            shortestPath = player.setPath(n, navMap);
        }

	}
}
