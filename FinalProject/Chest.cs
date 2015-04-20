using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AIClass
{
    public class Chest
    {
        public Vector2 position;
        public float x
        {
            get { return position.X; }
            set { position.X = value; }
        }

        public float y
        {
            get { return position.Y; }
            set { position.Y = value; }
        }
        public bool isOpen;
        public Texture2D img;
        String itemName;

        public Chest(float x, float y, String itemName)
        {
            this.x = x;
            this.y = y;
            this.itemName = itemName;
            isOpen = false;
        }

        public void draw(SpriteBatch spriteBatch, Texture2D img)
        {
            spriteBatch.Draw(img, position, Color.White);
        }
    }
}
