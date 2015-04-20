using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AIClass
{
    public class Inventory
    {
        public int selected = 0;
        public Viewport viewport;
        public int itemsize = 40;
        public int maxItems = 4;
        public int width;
        public List<Item> items;

        public enum Action
        {
            next
        }

        public Inventory(Viewport viewport)
        {
            this.viewport = viewport;
            items = new List<Item>();
            width = (itemsize * maxItems) + (maxItems + 1) * 5;
        }

        public void update(KeyboardState currentKState, KeyboardState prevKState)
        {
            if (prevKState.IsKeyUp(Keys.Tab) && currentKState.IsKeyDown(Keys.Tab))
                nextItem();
            if (prevKState.IsKeyUp(Keys.Space) && currentKState.IsKeyDown(Keys.Space))
                useItem();
        }

        public void nextItem()
        {
            selected = (selected+1) % maxItems;
            Console.WriteLine(selected);
        }

        public void addRandomItem()
        {
            if (items.Count < maxItems)
            {
                int rnd = Game1.rnd.Next(0, Game1.possibleItems.Count);
                Item i = new Item(0,0,32,32,30,Game1.possibleItems[rnd].name);
                i.img = Game1.possibleItems[rnd].img;
                items.Add(i);
            }
            
        }

        public void useItem()
        {
            if (selected < items.Count)
            {
                items[selected].updateLocation(Game1.player.position);
                items[selected].priority = items[selected].maxPriority;
                Console.WriteLine(items[selected]);
                Game1.distractions.Add(items[selected]);
                items.RemoveAt(selected);
                selected = Math.Max(selected - 1, 0);
            }
            
        }

        public void draw(SpriteBatch spriteBatch, Texture2D solidTexture, SpriteFont font, Vector2 cam)
        {
            Color transparent_black = Color.White * 0.5f;

            int x = (int)Game1.cam.Pos.X - width / 2;
            int y = (int)cam.Y + viewport.Height / 3 + 20;
            spriteBatch.Draw(solidTexture, new Rectangle(x, y - 5, width, itemsize + 10), transparent_black);
            spriteBatch.Draw(solidTexture, new Rectangle(selected * (itemsize + 5) + x, y - 5, itemsize + 10, itemsize + 10), Color.LightSeaGreen);

            for (int i = 0; i < maxItems; i++)
            {
                spriteBatch.Draw(solidTexture, new Rectangle(5 + i * (itemsize + 5) + x, y, itemsize, itemsize), Color.White);
                if (i < items.Count)
                {
                    spriteBatch.Draw(items[i].img, new Vector2(8 + i * (itemsize + 5) + x, y+5), Color.White); 
                }
                
            }
            if (selected < items.Count)
            {
                Vector2 size = font.MeasureString(items[selected].name);
                float left = x + ((width / 2) - (size.X / 2));
                spriteBatch.DrawString(font, items[selected].name, new Vector2(left, y - size.Y), Color.Black);
            }
            

        }
    }
}
