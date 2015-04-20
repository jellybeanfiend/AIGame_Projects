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

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
        SpriteBatch nodesBatch;
		public static Player player;
        public static Viewport viewport;
        public static PathGraph navMap;
        InfoBox infobox;
        InputHandler input;
        public static Random rnd;

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
        public Texture2D background;
        public Texture2D tree;
        public Texture2D chestimg;
        public Texture2D openchest;

        public static Node startNode;
        public static Node targetNode;
        public static List<Node> shortestPath;
        public Vector2 clickSeek;

        public static List<Agent> agents;
        public static List<BoundingBox> walls;
        public static List<Item> distractions;
        public static Camera cam;
        public static Inventory inventory;
        KeyboardState currentKState;
        KeyboardState prevKState;
        private AnimatedSprite animatedSprite;
        public List<Vector2> trees;
        public static List<BoundingBox> grass;
        public static List<Item> possibleItems;
        public static List<Chest> chests;
        public static int foundchests;
        public static State state;
        public static int counter;
        public string message = "The forest is haunted! You have the know-how to perform an exorcism, \n but first you need to collect 3 essential items located in chests throughout the map. \n\nWatch out for ghosts and if they get too close, drop fruit to keep them away. \n\nIf you need more fruit, search the bushes! Scroll through your inventory with tab and press 'E' to drop fruit.";
        public static Chest justOpened;

        public enum State
        {
            start,
            active,
            lost,
            won
        }

		public Game1()
			: base()
		{
            try { graphics = new GraphicsDeviceManager(this); }
            catch { }
            agents = new List<Agent>();
            walls = new List<BoundingBox>();
            
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
            state = State.start;
            counter = 500;
            
            rnd = new Random();
            // Use Viewport to get width and height of the screen
            viewport = GraphicsDevice.Viewport;

            input = new InputHandler();
            chests = new List<Chest>();
            possibleItems = new List<Item>();
            possibleItems.Add(new Item(0, 0, 25, 25, 30, "strawberry"));
            possibleItems.Add(new Item(0, 0, 25, 25, 30, "cherry"));
            possibleItems.Add(new Item(0, 0, 25, 25, 30, "pineapple"));

            cam = new Camera(viewport, new Vector2(500, 500), new Rectangle(0, 0, 1000, 1000));

            player = new Player(1000,1000, 20, 40, (float)-Math.PI / 2);

            for (int i = 0; i < 3; i++)
                agents.Add(new Agent(0, 0, 23, 28, 0));

            inventory = new Inventory(viewport);

            

            #region grass
            grass = new List<BoundingBox>();
            grass.Add(new BoundingBox(new Vector3(39,284,0), new Vector3(208, 360, 0)));
            grass.Add(new BoundingBox(new Vector3(16,589,0), new Vector3(107, 807, 0)));
            grass.Add(new BoundingBox(new Vector3(154,487,0), new Vector3(452, 567, 0)));
            grass.Add(new BoundingBox(new Vector3(303,909,0), new Vector3(544, 979, 0)));
            grass.Add(new BoundingBox(new Vector3(607,704,0), new Vector3(799, 781, 0)));
            grass.Add(new BoundingBox(new Vector3(860,834,0), new Vector3(954, 913, 0)));
            #endregion


            #region tree vectors
            trees = new List<Vector2>();
            trees.Add(new Vector2(480, 140));
            trees.Add(new Vector2(540, 140));
            trees.Add(new Vector2(600, 140));

            trees.Add(new Vector2(450, 165));
            trees.Add(new Vector2(510, 165));
            trees.Add(new Vector2(570, 165));
            trees.Add(new Vector2(630, 165));

            trees.Add(new Vector2(480, 190));
            trees.Add(new Vector2(540, 190));
            trees.Add(new Vector2(600, 190));

            trees.Add(new Vector2(600, 490));
            trees.Add(new Vector2(660, 490));
            trees.Add(new Vector2(600, 410));
            trees.Add(new Vector2(660, 410));

            trees.Add(new Vector2(690, 435));
            trees.Add(new Vector2(690, 485));
            trees.Add(new Vector2(690, 515));

            trees.Add(new Vector2(570, 435));
            trees.Add(new Vector2(570, 485));
            trees.Add(new Vector2(570, 515));

            trees.Add(new Vector2(630, 435));
            trees.Add(new Vector2(630, 485));
            trees.Add(new Vector2(630, 515));

            trees.Add(new Vector2(200, 205));

            trees.Add(new Vector2(300, 570));
            trees.Add(new Vector2(360, 570));

            trees.Add(new Vector2(620, 800));
            trees.Add(new Vector2(730, 800));
            trees.Add(new Vector2(675, 820));
            #endregion

            // Populate world

            // Add bounds
            #region Bounds
            walls.Add(new BoundingBox(new Vector3(0, 43, 0), new Vector3(303, 215, 0)));
            walls.Add(new BoundingBox(new Vector3(0, 0, 0), new Vector3(173, 43, 0)));
            walls.Add(new BoundingBox(new Vector3(833, 0, 0), new Vector3(1000, 114, 0)));
            walls.Add(new BoundingBox(new Vector3(736, 0, 0), new Vector3(837, 50, 0)));
            walls.Add(new BoundingBox(new Vector3(609, 0, 0), new Vector3(741, 27, 0)));
            walls.Add(new BoundingBox(new Vector3(857, 125, 0), new Vector3(967, 245, 0)));
            walls.Add(new BoundingBox(new Vector3(889, 218, 0), new Vector3(919, 349, 0)));
            walls.Add(new BoundingBox(new Vector3(889, 453, 0), new Vector3(919, 520, 0)));
            walls.Add(new BoundingBox(new Vector3(831, 512, 0), new Vector3(1000, 596, 0)));
            walls.Add(new BoundingBox(new Vector3(117, 829, 0), new Vector3(256, 945, 0)));
            // Trees
            walls.Add(new BoundingBox(new Vector3(512, 307, 0), new Vector3(657, 320, 0)));
            walls.Add(new BoundingBox(new Vector3(482, 279, 0), new Vector3(685, 297, 0)));
            walls.Add(new BoundingBox(new Vector3(600, 514, 0), new Vector3(745, 649, 0)));
            walls.Add(new BoundingBox(new Vector3(419, 667, 0), new Vector3(482, 708, 0)));
            walls.Add(new BoundingBox(new Vector3(232, 324, 0), new Vector3(258, 338, 0)));
            walls.Add( new BoundingBox(new Vector3(332,689,0), new Vector3(358,703,0)));
            walls.Add(new BoundingBox(new Vector3(392,689,0), new Vector3(418,703,0)));
            walls.Add(new BoundingBox(new Vector3(652,919,0), new Vector3(678,933,0)));
            walls.Add(new BoundingBox(new Vector3(762,919,0), new Vector3(788,933,0)));
            walls.Add(new BoundingBox(new Vector3(707,939,0), new Vector3(733,953,953)));

            walls.Add(new BoundingBox(new Vector3(933,260,0), new Vector3(965,284,0)));
            walls.Add(new BoundingBox(new Vector3(30, 883, 0), new Vector3(62, 907, 0)));
            walls.Add(new BoundingBox(new Vector3(310, 15, 0), new Vector3(342, 39, 0)));

            chests.Add(new Chest(933, 260, "Necklace of Vitality"));
            chests.Add(new Chest(30, 883, "Hat of Wisdom"));
            chests.Add(new Chest(310, 15, "Scroll of Truth"));
#endregion


            

            navMap = new PathGraph(1000,1000);

            setVariables();



			base.Initialize();
		}

        public void setVariables()
        {
            foundchests = 3;
            cam.Pos = new Vector2(500, 500);
            player.position = new Vector2((int)cam.Pos.X, (int)cam.Pos.Y);

            distractions = new List<Item>();

            // Add agents
            for (int i = 0; i < 3; i++)
            {
                Node n = navMap.getValidNode(player.position, 100);
                agents[i].position = new Vector2((int)n.position.X, (int)n.position.Y);
            }

            for (int i = 0; i < chests.Count; i++)
                chests[i].isOpen = false;
            if (possibleItems[0].img != null)
            {
                inventory.items.Clear();
                inventory.addRandomItem();
                inventory.addRandomItem();
            }

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
            playerImage = Content.Load<Texture2D>("princess");
            enemyImage = Content.Load<Texture2D>("ghost");
            player.sprite = new AnimatedSprite(playerImage, 4,9);
            foreach (Agent enemy in agents)
            {
                enemy.sprite = new AnimatedSprite(enemyImage, 4,3);
            }
            solidTexture = new Texture2D(GraphicsDevice, 1, 1);
            perimeter = Content.Load<Texture2D>("bigcircle");
            badguyOutline = Content.Load<Texture2D>("badguyoutline");
            solidTexture.SetData(new Color[] { Color.White });
            font18 = Content.Load<SpriteFont>("SpriteFont1");
            font12 = Content.Load<SpriteFont>("SpriteFont2");
            font14 = Content.Load<SpriteFont>("SpriteFont3");
            nodeimg = Content.Load<Texture2D>("node");
            //selectednode = Content.Load<Texture2D>("selectednode");
            pathnode = Content.Load<Texture2D>("pathnode");
            targetnode = Content.Load<Texture2D>("targetnode");
            background = Content.Load<Texture2D>("bg");
            chestimg = Content.Load<Texture2D>("chest");
            openchest = Content.Load<Texture2D>("openchest");
            tree = Content.Load<Texture2D>("tree");
            Texture2D strawberry = Content.Load<Texture2D>("I_C_Strawberry");
            Texture2D cherry = Content.Load<Texture2D>("I_C_Cherry");
            Texture2D pineapple = Content.Load<Texture2D>("I_C_Pineapple");
            Texture2D scroll = Content.Load<Texture2D>("I_Scroll");
            Texture2D hat = Content.Load<Texture2D>("C_Hat01");
            Texture2D necklace = Content.Load<Texture2D>("Ac_Necklace01");
            possibleItems[0].img = strawberry;
            possibleItems[1].img = cherry;
            possibleItems[2].img = pineapple;
            chests[0].img = hat;
            chests[1].img = scroll;
            chests[2].img = necklace;
            // Infobox content

            Texture2D texture = Content.Load<Texture2D>("princess");
            animatedSprite = new AnimatedSprite(texture, 4,9);

            inventory.addRandomItem();
            inventory.addRandomItem();

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
            currentKState = Keyboard.GetState();

            if (state == State.start)
            {
                if(currentKState.IsKeyDown(Keys.Enter))
                    state = State.active;
                if (counter < 0)
                {
                    counter = 0;
                    state = State.active;
                }
                else
                    counter--;
            }
            if (state == State.won || state == State.lost)
            {
                if (currentKState.IsKeyDown(Keys.Enter))
                {
                    setVariables();
                    state = State.active;
                }
                    
            }
            if(state == State.active)
            {
                player.update(currentKState, prevKState);
                inventory.update(currentKState, prevKState);

                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                cam.update(player.position);

                foreach (Agent agent in agents)
                {
                    agent.update();
                }

                prevKState = currentKState;

                if (justOpened != null)
                {
                    counter--;
                    if (counter == 0)
                        justOpened = null;
                }

                if (foundchests == 0)
                    state = State.won;
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

            List<Agent> adjacentAgents = player.getAdjacentAgents();

            //spriteBatch.Begin();
            spriteBatch.Begin(SpriteSortMode.Deferred,
                        BlendState.AlphaBlend,
                        null,
                        null,
                        null,
                        null,
                        cam.get_transformation(viewport.Width, viewport.Height));

            // Background
            spriteBatch.Draw(background, new Rectangle(0, 0, 1000, 1000), Color.White);

            

            foreach (Chest chest in chests)
            {
                if (chest.isOpen)
                    chest.draw(spriteBatch, openchest);
                else
                    chest.draw(spriteBatch, chestimg);
            }

            //foreach (BoundingBox wall in walls)
            //{
            //    spriteBatch.Draw(solidTexture, new Rectangle((int)wall.Min.X, (int)wall.Min.Y, (int)(wall.Max.X - wall.Min.X), (int)(wall.Max.Y - wall.Min.Y)), Color.Teal);
            //}

            // Draw Items
            foreach (Item i in distractions)
            {
                spriteBatch.Draw(i.img, new Rectangle((int)i.x, (int)i.y, 32, 32), Color.White);
            }

            // Draw player

            player.draw(spriteBatch, playerImage);

            // Draw enemies
            foreach (Agent enemy in agents)
            {
                enemy.draw(spriteBatch, enemyImage);
            }


            // Draw Trees
            foreach (Vector2 treePosition in trees)
            {
                spriteBatch.Draw(tree, treePosition, Color.White); 
            }

            

            if (justOpened != null)
            {
                spriteBatch.Draw(justOpened.img, justOpened.position - new Vector2(0, 20), Color.White);
            }

            // Draw inventory
            inventory.draw(spriteBatch, solidTexture, font14, cam.Pos);

            spriteBatch.DrawString(font14, "Items left to find: " + foundchests, new Vector2(cam.Pos.X - (viewport.Width / 2), cam.Pos.Y - (viewport.Height / 2)), Color.White);

            if (state == State.start)
            {
                //spriteBatch.Draw(solidTexture, new Rectangle((int)cam.Pos.X - (viewport.Width / 2), (int)cam.Pos.Y - (viewport.Height / 2), (int)viewport.Width, (int)viewport.Height), Color.Black * 0.8f);
                displayMessage(spriteBatch, message);
                spriteBatch.DrawString(font14, "Press Enter to continue", new Vector2(cam.Pos.X + (viewport.Width / 2) - 200, cam.Pos.Y + (viewport.Height / 2) - 30), Color.White);
            }
            if(state == State.lost)
                displayMessage(spriteBatch, "You've been caught! Time to join the spirit world.\n Press Enter to play again!");
            if (state == State.won)
                displayMessage(spriteBatch, "You did it! You found all the items and performed the exercism. \nNow you can keep all that delicious fruit to yourself.\n Press Enter to play again!");
            spriteBatch.End();

			base.Draw(gameTime);
		}

        public void displayMessage(SpriteBatch spriteBatch, string s)
        {
            spriteBatch.Draw(solidTexture, new Rectangle((int)cam.Pos.X - (viewport.Width / 2), (int)cam.Pos.Y, (int)viewport.Width, (int)viewport.Height), Color.Black * 0.9f);
            spriteBatch.DrawString(font14, s, new Vector2((int)cam.Pos.X - (viewport.Width / 2), cam.Pos.Y), Color.White);

        }
	}
}
