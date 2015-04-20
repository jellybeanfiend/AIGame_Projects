// Sevena Skeels
// Homework 2
// CAP4053

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AIClass
{
    class InputHandler
    {

        public MouseState mouseStateCurrent;
        public MouseState mouseStatePrev;

        public InputHandler(){

        }

        public void updateState()
        {
            mouseStateCurrent = Mouse.GetState();
        }

        public void saveState()
        {
            mouseStatePrev = mouseStateCurrent;
        }

        public bool isMouseClicked(){
            return mouseStateCurrent.LeftButton == ButtonState.Pressed && mouseStatePrev.LeftButton == ButtonState.Released;
        }

        /// <summary>
        /// Determines if the user has clicked inside the given rectangle.
        /// </summary>
        /// <param name="rect">The rectangle that is checked</param>
        /// <returns>True if the user has clicked inside the rectangle</returns>
        public bool mouseClicked(Rectangle rect)
        {
            return (mouseStateCurrent.LeftButton == ButtonState.Pressed && mouseStatePrev.LeftButton == ButtonState.Released) && rect.Contains(mouseStateCurrent.X, mouseStateCurrent.Y);
        }

        /// <summary>
        /// Determines if the user currently has the mouse pressed inside the given rectangle.
        /// </summary>
        /// <param name="rect">The rectangle that is checked</param>
        /// <returns>True if the user is holding down the left mouse button inside the given rectangle.</returns>
        public bool mousePressed(Rectangle rect)
        {
            return rect.Contains(mouseStateCurrent.X, mouseStateCurrent.Y) && mouseStateCurrent.LeftButton == ButtonState.Pressed;
        }

    }
}
