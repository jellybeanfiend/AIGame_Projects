using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace AIClass
{
    public class Camera
    {

		protected float          _zoom; // Camera Zoom
        public Matrix             _transform; // Matrix Transform
        public Vector2          _pos; // Camera Position
        protected float         _rotation; // Camera Rotation
        private Rectangle _limits;
        private Viewport _viewport;
        public Vector2 Origin { get; set; }

        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; if (_zoom < 0.1f) _zoom = 0.1f; } // Negative zoom will flip image
        }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        // Auxiliary function to move the camera
        public void Move(Vector2 amount)
        {
            Pos += amount;
        }
        // Get set position
        public Vector2 Pos
        {
            get { return _pos; }
            set {
                _pos = value;
                // If there's a limit set and the camera is not transformed clamp position to limits
                if (Limits != null && Zoom == 1.0f && Rotation == 0.0f)
                {
                    _pos.X = MathHelper.Clamp(_pos.X, Limits.X + _viewport.Width/2, Limits.Width - _viewport.Width/2);
                    _pos.Y = MathHelper.Clamp(_pos.Y, Limits.Y + _viewport.Height / 2, Limits.Height - _viewport.Height / 2);
                }
            }
        }

        public Rectangle Limits
        {
            get { return _limits; }
            set
            {
                // Assign limit but make sure it's always bigger than the viewport
                _limits = new Rectangle
                {
                    X = value.X,
                    Y = value.Y,
                    Width = System.Math.Max(_viewport.Width, value.Width),
                    Height = System.Math.Max(_viewport.Height, value.Height)
                };
			 }
			
        }

        public Camera(Viewport viewport, Vector2 pos, Rectangle bounds)
        {
            Origin = new Vector2(viewport.Width / 2.0f, viewport.Height / 2.0f);
            _zoom = 1.0f;
            _rotation = 0.0f;
            _pos = pos;
            Limits = bounds;
            _viewport = viewport;
        }

        public void update(Vector2 playerpos)
        {

			int newx = 0;
			int newy = 0;

            if (Math.Abs(playerpos.X - Pos.X) > _viewport.Width / 60)
                newx = playerpos.X > Pos.X ? 1 : -1;

			if(Math.Abs(playerpos.Y - Pos.Y) > _viewport.Height / 60)
				newy = playerpos.Y > Pos.Y ? 1 : -1;

			if( newx != 0 || newy != 0)
				Move(new Vector2(newx, newy));
        }

        public Matrix get_transformation(float width, float height)
        {
            _transform =       // Thanks to o KB o for this solution
              Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(Origin, 0));
            return _transform;
        }


    }
}
