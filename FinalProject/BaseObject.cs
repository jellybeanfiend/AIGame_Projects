// Sevena Skeels
// Homework 2
// CAP4053

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AIClass
{
	public class BaseObject
	{
		public Vector2 position;
		#region properties
		public float x
		{
			get { return position.X; }
            set { position.X = value;}
		}

		public float y
		{
			get { return position.Y; }
            set { position.Y = value;}
		}

		public int length
		{
			get;
			set;
		}

        public int width
        {
            get;
            set;
        }
		#endregion

        public int maxPriority = 0;
        public int priority = 0;
        public BoundingBox bounds;

        public BaseObject(int x, int y, int width, int length)
        {
            this.x = x;
            this.y = y;
            this.length = length;
            this.width = width;
            bounds = new BoundingBox(new Vector3(x, y, 0), new Vector3(x + (width / 2), y + (length / 2), 0));
        }

        public double getDistanceBetween(Vector2 location)
        {
            return Math.Max(Math.Sqrt(Math.Pow(Math.Abs(x - location.X), 2) + Math.Pow(Math.Abs(y - location.Y), 2)), 0);
        }

	}
}
