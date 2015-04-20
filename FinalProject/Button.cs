using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AIClass
{
    class Button
    {

        public Rectangle bounds;
        public bool toggle;
        public string message;

        public Button(Rectangle bounds, string message)
        {
            this.bounds = bounds;
            this.toggle = false;
            this.message = message;
        }

        public void click()
        {
            toggle = !toggle;
        }

        public bool wasClicked(Point clickLocation)
        {
            return bounds.Contains(clickLocation);
        }

        public void draw(SpriteBatch spriteBatch, Texture2D background, SpriteFont font)
        {
            Vector2 size = font.MeasureString(message);
            float left = bounds.X + ((bounds.Width/2) - (size.X/2));
            float top = bounds.Y + ((bounds.Height / 2) - (size.Y / 2));
            spriteBatch.Draw(background, bounds, Color.MediumTurquoise);
            spriteBatch.DrawString(font, message, new Vector2(left, top), Color.White);
        }
    }
}
