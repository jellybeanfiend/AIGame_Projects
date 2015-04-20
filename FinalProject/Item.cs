using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AIClass
{
    public class Item : BaseObject
    {

        public Texture2D img;
        public String name;
        public int maxPriority;

        public Item(int x, int y, int width, int length, int priority, String name)
            : base(x, y, width, length)
        {
            this.maxPriority = priority;
            this.priority = priority;
            this.name = name;
        }

        public void updateLocation(Vector2 pos)
        {
            position = pos;
            bounds = new BoundingBox(new Vector3(pos.X, pos.Y, 0), new Vector3(pos.X+width, pos.Y+length, 0));
        }

    }
}
