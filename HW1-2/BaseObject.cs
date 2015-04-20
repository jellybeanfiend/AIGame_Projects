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

		public float heading
		{
			get;
			set;
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

		/// <summary>
		/// Initializes the object.
		/// </summary>
		/// <param name="x">The starting X coordinate of the object.</param>
		/// <param name="y">The starting Y coordinate of the object.</param>
		/// <param name="length">The length of the object.</param>
		/// <param name="width">The width of the object.</param>
		/// <param name="heading">The heading of the object.</param>
        public void Initialize(int x, int y, int width, int length, float heading)
		{
			this.heading = heading;
			this.x = x;
			this.y = y;
			this.length = length;
			this.width = width;
		}
		
	}
}
